using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Gesture;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.ViewModels.Map {
	/// <summary>
	/// GPS選択ウィンドウViewModel
	/// </summary>
	public class GpsSelectorWindowViewModel : DialogViewModelBase {
		public static string ParameterNameTargetFiles = nameof(ParameterNameTargetFiles);
		/// <summary>
		/// モデル
		/// </summary>
		private readonly IGpsSelector _model;

		/// <summary>
		/// 操作受信
		/// </summary>
		public IGestureReceiver GestureReceiver {
			get {
				return this._model.GestureReceiver;
			}
			set {
			}
		}

		/// <summary>
		/// 座標
		/// </summary>
		public IReadOnlyReactiveProperty<GpsLocation> Location {
			get;
		}

		/// <summary>
		/// 処理対象ファイル
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileViewModel>> TargetFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileViewModel>>(Array.Empty<IMediaFileViewModel>());

		/// <summary>
		/// GPS設定対象候補リスト
		/// </summary>
		public ReadOnlyReactiveCollection<IMediaFileViewModel> CandidateMediaFiles {
			get;
		}

		/// <summary>
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapViewModel> Map {
			get;
		}

		/// <summary>
		/// ズームレベル
		/// </summary>
		public IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		/// <summary>
		/// ウィンドウタイトル
		/// </summary>
		public override string Title {
			get {
				return "GPSの情報の再設定";
			}
			set {
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public GpsSelectorWindowViewModel(IGpsSelector model, ViewModelFactory viewModelFactory) {
			this._model = model;
			this.Location = this._model.Location.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.ZoomLevel = this._model.ZoomLevel.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.CandidateMediaFiles =
				this._model
					.CandidateMediaFiles
					.ToReadOnlyReactiveCollection(
						this._model.CandidateMediaFiles.ToCollectionChanged<IMediaFileModel>(),
						viewModelFactory.Create,
						disposeElement: false
					).AddTo(this.CompositeDisposable);
			this.Map =
				this._model
					.Map
					.Select(viewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			// 処理対象ファイル ViewModel⇔Model間双方向同期
			this.TargetFiles.TwoWaySynchronize(
				this._model.TargetFiles,
				x => x.Select(vm => vm.Model),
				x => x.Select(viewModelFactory.Create))
				.AddTo(this.CompositeDisposable);

			// モデル破棄時にこのインスタンスも破棄
			this.AddTo(this._model.CompositeDisposable);
		}

		/// <summary>
		/// GPS設定対象ファイル注入用
		/// </summary>
		/// <param name="mediaFileViewModels">対象ファイルリスト</param>
		public void SetCandidateMediaFiles(IEnumerable<IMediaFileViewModel> mediaFileViewModels) {
			this._model.CandidateMediaFiles.AddRange(mediaFileViewModels.Select(x => x.Model));
		}

		public override void OnDialogOpened(IDialogParameters parameters) {
			// 設定対象ファイルの取得
			var mediaFileViewModels = parameters.GetValue<IEnumerable<IMediaFileViewModel>>(ParameterNameTargetFiles);
			this._model.CandidateMediaFiles.AddRange(mediaFileViewModels.Select(x => x.Model));
		}
	}
}
