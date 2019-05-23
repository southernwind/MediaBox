using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Album {

	/// <summary>
	/// アルバムViewModel
	/// </summary>
	internal class AlbumViewModel : MediaFileCollectionViewModel<AlbumModel> {
		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public IReadOnlyReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// カレントインデックス番号
		/// </summary>
		public IReactiveProperty<int> CurrentIndex {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// フィルタリング前件数
		/// </summary>
		public IReadOnlyReactiveProperty<int> BeforeFilteringCount {
			get;
		}

		/// <summary>
		/// 選択中メディアファイル
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileViewModel>> SelectedMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileViewModel>>(Array.Empty<IMediaFileViewModel>());

		/// <summary>
		/// カレントメディアファイル
		/// </summary>
		public IReadOnlyReactiveProperty<IMediaFileViewModel> CurrentItem {
			get;
		}

		/// <summary>
		/// 複数メディアファイルまとめて情報表示用ViewModel
		/// </summary>
		public IReadOnlyReactiveProperty<MediaFileInformationViewModel> MediaFileInformation {
			get;
		}

		/// <summary>
		/// 表示モード
		/// </summary>
		public IReactiveProperty<DisplayMode> DisplayMode {
			get;
		}

		/// <summary>
		/// 表示モードに対応した型
		/// </summary>
		public IReadOnlyReactiveProperty<DisplayBase> DisplayViewModel {
			get;
		}

		/// <summary>
		/// 表示モード変更コマンド
		/// </summary>
		public ReactiveCommand<DisplayMode> ChangeDisplayModeCommand {
			get;
		} = new ReactiveCommand<DisplayMode>();

		/// <summary>
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapViewModel> Map {
			get;
		}

		/// <summary>
		/// ファイル追加コマンド
		/// </summary>
		public ReactiveCommand<IEnumerable<IMediaFileViewModel>> AddMediaFileCommand {
			get;
		} = new ReactiveCommand<IEnumerable<IMediaFileViewModel>>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public AlbumViewModel(AlbumModel model) : base(model) {
			this.Title = this.Model.Title.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.BeforeFilteringCount = this.Model.BeforeFilteringCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.Map = this.Model.Map.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.MediaFileInformation =
				this.Model
					.MediaFileInformation
					.Select(this.ViewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			this.DisplayMode = this.Model.DisplayMode.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			// DataTemplateのDataTypeでテンプレートを切り替えるための苦肉の策
			// どのクラスもこのアルバムViewModel(this)のインスタンスを持っていて、型だけが違う
			// これらの3つのクラスをDataTemplateのDataTypeにバインドすることでViewを切り替えている
			this.DisplayViewModel = this.DisplayMode.Select<DisplayMode, DisplayBase>(x => {
				switch (x) {
					default:
						return new DisplayLibrary(this);
					case Composition.Enum.DisplayMode.Detail:
						return new DisplayDetail(this);
					case Composition.Enum.DisplayMode.Map:
						return new DisplayMap(this);
				}
			}).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.CurrentItem = this.Model.CurrentMediaFile.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			// VM⇔Model間双方向同期
			this.SelectedMediaFiles.TwoWaySynchronize(
				this.Model.CurrentMediaFiles,
				x => x.Select(vm => vm.Model).ToArray(),
				x => x.Select(this.ViewModelFactory.Create).ToArray());

			// 表示モード変更コマンド
			this.ChangeDisplayModeCommand
				.Subscribe(this.Model.ChangeDisplayMode)
				.AddTo(this.CompositeDisposable);

			// ファイル追加コマンド
			this.AddMediaFileCommand.Subscribe(x => {
				if (this.Model is RegisteredAlbum ra) {
					ra.AddFiles(x.Select(vm => vm.Model));
				}
			});

			// ソート順やフィルタリングを行うためのコレクションビューの取得
			var itemsCollectionView = CollectionViewSource.GetDefaultView(this.Items);

			// 先読みロード
			this.CurrentIndex
				.CombineLatest(
					this.DisplayMode,
					(currentIndex, displayMode) => (currentIndex, displayMode))
				// TODO : 時間で制御はあまりやりたくないな　何か考える
				.Throttle(TimeSpan.FromMilliseconds(100))
				.Subscribe(x => {
					if (x.currentIndex == -1 || x.displayMode != Composition.Enum.DisplayMode.Detail) {
						// 全アンロード
						this.Model.Prefetch(Array.Empty<IMediaFileModel>());
						return;
					}
					if (!(itemsCollectionView is CollectionView cv)) {
						return;
					}

					var minIndex = Math.Max(0, x.currentIndex - 2);
					var count = Math.Min(x.currentIndex + 2, cv.Count - 1) - minIndex + 1;
					try {
						// 読み込みたい順に並べる
						var vms =
							Enumerable
								.Range(minIndex, count)
								.OrderBy(i => i >= x.currentIndex ? 0 : 1)
								.ThenBy(i => Math.Abs(i - x.currentIndex))
								.Select(i => (IMediaFileViewModel)cv.GetItemAt(i))
								.ToArray();
						this.Model.Prefetch(vms.Select(vm => vm.Model));
					} catch (Exception ex) {
						// なにかの例外が発生する。再現待ち。
						this.Logging.Log(ex);
						this.Logging.Log(minIndex);
						this.Logging.Log(count);
						this.Logging.Log(cv);
						this.Logging.Log(cv.Count);
						throw;
					}
				});
		}
	}

	#region 苦肉の策
	internal abstract class DisplayBase {
		public AlbumViewModel AlbumViewModel {
			get;
		}
		public DisplayBase(AlbumViewModel albumViewModel) {
			this.AlbumViewModel = albumViewModel;
		}
	}
	internal class DisplayLibrary : DisplayBase {
		public DisplayLibrary(AlbumViewModel albumViewModel) : base(albumViewModel) {
		}
	}
	internal class DisplayDetail : DisplayBase {
		public DisplayDetail(AlbumViewModel albumViewModel) : base(albumViewModel) {
		}
	}
	internal class DisplayMap : DisplayBase {
		public DisplayMap(AlbumViewModel albumViewModel) : base(albumViewModel) {
		}
	}
	#endregion
}
