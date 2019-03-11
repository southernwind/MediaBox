using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Livet.Messaging.IO;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムクリエイターViewModel
	/// </summary>
	internal class AlbumEditorViewModel : ViewModelBase {

		/// <summary>
		/// メディア選択用アルバムセレクター
		/// </summary>
		public AlbumSelectorViewModel AlbumSelectorViewModel {
			get;
		}

		/// <summary>
		/// 選択中未追加メディア
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileViewModel>> SelectedNotAddedMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileViewModel>>(Array.Empty<IMediaFileViewModel>());

		/// <summary>
		/// 選択中追加済みメディア
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileViewModel>> SelectedAddedMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileViewModel>>(Array.Empty<IMediaFileViewModel>());

		/// <summary>
		/// パス
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// 監視ディレクトリ
		/// </summary>
		public ReadOnlyReactiveCollection<string> MonitoringDirectories {
			get;
		}

		/// <summary>
		/// 選択中監視ディレクトリ
		/// </summary>
		public IReactiveProperty<string> SelectedMonitoringDirectory {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// パス
		/// </summary>
		public IReactiveProperty<string> AlbumPath {
			get;
		}

		/// <summary>
		/// ファイルリスト
		/// </summary>
		public ReadOnlyReactiveCollection<IMediaFileViewModel> Items {
			get;
		}

		/// <summary>
		/// アルバムに監視ディレクトリを追加する
		/// </summary>
		public ReactiveCommand<FolderSelectionMessage> AddMonitoringDirectoryCommand {
			get;
		} = new ReactiveCommand<FolderSelectionMessage>();

		/// <summary>
		/// アルバムから監視ディレクトリを削除する
		/// </summary>
		public ReactiveCommand RemoveMonitoringDirectoryCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// アルバム新規作成コマンド
		/// </summary>
		public ReactiveCommand CreateAlbumCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 保存コマンド
		/// </summary>
		public ReactiveCommand SaveCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 読み込みコマンド
		/// </summary>
		public ReactiveCommand LoadCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// アルバム編集コマンド
		/// </summary>
		public ReactiveCommand<AlbumViewModel> EditAlbumCommand {
			get;
		} = new ReactiveCommand<AlbumViewModel>();

		/// <summary>
		/// 候補メディアをアルバムに追加するコマンド
		/// </summary>
		public ReactiveCommand AddFilesCommand {
			get;
		}

		/// <summary>
		/// 候補メディアをアルバムに追加するコマンド
		/// </summary>
		public ReactiveCommand RemoveFilesCommand {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumEditorViewModel() {
			var model = Get.Instance<AlbumEditor>().AddTo(this.CompositeDisposable);
			this.ModelForToString = model;
			this.AlbumSelectorViewModel = Get.Instance<AlbumSelectorViewModel>();

			this.Title = model.Title.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.AlbumPath = model.AlbumPath.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.MonitoringDirectories = model.MonitoringDirectories.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this.Items = model.Items.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create, disposeElement: false).AddTo(this.CompositeDisposable);

			this.AddFilesCommand = this.SelectedNotAddedMediaFiles.Select(x => x.Any()).ToReactiveCommand();
			this.AddFilesCommand.Subscribe(_ => {
				model.AddFiles(this.SelectedNotAddedMediaFiles.Value.Select(x => x.Model));
				this.SelectedNotAddedMediaFiles.Value = Array.Empty<IMediaFileViewModel>();
			}).AddTo(this.CompositeDisposable);

			this.AlbumSelectorViewModel.CurrentAlbum.Subscribe(x => {
				this.SelectedNotAddedMediaFiles.Value = Array.Empty<IMediaFileViewModel>();
			});

			this.RemoveFilesCommand = this.SelectedAddedMediaFiles.Select(x => x.Any()).ToReactiveCommand();
			this.RemoveFilesCommand.Subscribe(_ => {
				model.RemoveFiles(this.SelectedAddedMediaFiles.Value.Select(x => x.Model));
				this.SelectedAddedMediaFiles.Value = Array.Empty<IMediaFileViewModel>();
			}).AddTo(this.CompositeDisposable);

			this.AddMonitoringDirectoryCommand
				.Subscribe(x => {
					if (x.Response == null) {
						return;
					}
					model.AddDirectory(x.Response);
				}).AddTo(this.CompositeDisposable);

			this.RemoveMonitoringDirectoryCommand.Subscribe(_ => model.RemoveDirectory(this.SelectedMonitoringDirectory.Value)).AddTo(this.CompositeDisposable);

			this.CreateAlbumCommand.Subscribe(model.CreateAlbum).AddTo(this.CompositeDisposable);

			this.EditAlbumCommand.Subscribe(x => {
				if (x.Model is RegisteredAlbum ra) {
					model.EditAlbum(ra);
					model.Load();
				}
			}).AddTo(this.CompositeDisposable);

			this.SaveCommand.Subscribe(model.Save).AddTo(this.CompositeDisposable);

			this.LoadCommand.Subscribe(model.Load).AddTo(this.CompositeDisposable);
		}
	}
}
