using System;
using System.Collections.Generic;
using System.Reactive.Linq;

using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album;

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
		/// アルバムボックスID
		/// </summary>
		public IReactiveProperty<int?> AlbumBoxId {
			get;
		}

		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReadOnlyReactiveProperty<string[]> AlbumBoxTitle {
			get;
		}

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
		/// アルバムボックス変更コマンド
		/// </summary>
		public ReactiveCommand AlbumBoxChangeCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumEditorViewModel(AlbumEditor albumEditor) {
			this.ModelForToString = albumEditor.AddTo(this.CompositeDisposable);
			this.AlbumSelectorViewModel = new AlbumSelectorViewModel(albumEditor.AlbumSelector);

			this.AlbumBoxId = albumEditor.AlbumBoxId.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.AlbumBoxTitle = albumEditor.AlbumBoxTitle.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Title = albumEditor.Title.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.MonitoringDirectories = albumEditor.MonitoringDirectories.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.AlbumSelectorViewModel.CurrentAlbum.Subscribe(x => {
				this.SelectedNotAddedMediaFiles.Value = Array.Empty<IMediaFileViewModel>();
			});

			this.AddMonitoringDirectoryCommand
				.Subscribe(x => {
					if (x.Response == null) {
						return;
					}
					albumEditor.AddDirectory(x.Response);
				}).AddTo(this.CompositeDisposable);

			this.RemoveMonitoringDirectoryCommand.Subscribe(_ => albumEditor.RemoveDirectory(this.SelectedMonitoringDirectory.Value)).AddTo(this.CompositeDisposable);

			this.CreateAlbumCommand.Subscribe(albumEditor.CreateAlbum).AddTo(this.CompositeDisposable);

			this.EditAlbumCommand.Subscribe(x => {
				if (x.Model is RegisteredAlbum ra) {
					albumEditor.EditAlbum(ra);
					albumEditor.Load();
				}
			}).AddTo(this.CompositeDisposable);

			this.AlbumBoxChangeCommand.Subscribe(_ => {
				using var vm = new AlbumBoxSelectorViewModel();
				var message = new TransitionMessage(typeof(Views.Album.AlbumBoxSelectorWindow), vm, TransitionMode.Modal);
				this.Messenger.Raise(message);
				if (vm.Completed) {
					this.AlbumBoxId.Value = vm.AlbumBoxId.Value;
				}
			});

			this.SaveCommand.Subscribe(x => {
				albumEditor.Save();
				this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
			}).AddTo(this.CompositeDisposable);

			this.LoadCommand.Subscribe(albumEditor.Load).AddTo(this.CompositeDisposable);
		}
	}
}
