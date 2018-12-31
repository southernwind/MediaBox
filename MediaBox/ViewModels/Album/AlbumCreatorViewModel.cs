using System;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Album {
	internal class AlbumCreatorViewModel : ViewModelBase {

		/// <summary>
		/// メディア選択用アルバムコンテナー
		/// </summary>
		public AlbumContainerViewModel AlbumContainerViewModel {
			get;
		}

		/// <summary>
		/// 作成するアルバム
		/// </summary>
		public ReadOnlyReactiveProperty<AlbumViewModel> AlbumViewModel {
			get;
		}

		/// <summary>
		/// 選択中未追加メディア
		/// </summary>
		public ReactiveCollection<MediaFileViewModel> SelectedNotAddedMediaFiles {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();

		/// <summary>
		/// 選択中追加済みメディア
		/// </summary>
		public ReactiveCollection<MediaFileViewModel> SelectedAddedMediaFiles {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();

		/// <summary>
		/// アルバムに監視ディレクトリを追加する
		/// </summary>
		public ReactiveCommand<string> AddMonitoringDirectoryCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// アルバム新規作成コマンド
		/// </summary>
		public ReactiveCommand CreateAlbumCommand {
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
		} = new ReactiveCommand();

		/// <summary>
		/// 候補メディアをアルバムに追加するコマンド
		/// </summary>
		public ReactiveCommand RemoveFilesCommand {
			get;
		} = new ReactiveCommand();

		public AlbumCreatorViewModel() {
			var model = Get.Instance<AlbumCreator>().AddTo(this.CompositeDisposable);
			this.AlbumContainerViewModel = Get.Instance<AlbumContainerViewModel>();
			this.AlbumViewModel =
				model
					.Album
					.Select(x => x == null ? null : this.ViewModelFactory.Create(x))
					.ToReadOnlyReactiveProperty()
					.AddTo(this.CompositeDisposable);

			this.AddFilesCommand.Subscribe(_ => {
				model.AddFiles(this.SelectedNotAddedMediaFiles.Select(x => x.Model));
			}).AddTo(this.CompositeDisposable);

			this.RemoveFilesCommand.Subscribe(_ => {
				model.RemoveFiles(this.SelectedAddedMediaFiles.Select(x => x.Model));
			}).AddTo(this.CompositeDisposable);

			this.AddMonitoringDirectoryCommand.Subscribe(x => {
				model.AddDirectory(x);
			}).AddTo(this.CompositeDisposable);

			this.CreateAlbumCommand.Subscribe(x => {
				model.CreateAlbum();
			}).AddTo(this.CompositeDisposable);

			this.EditAlbumCommand.Subscribe(x => {
				if (x.Model is RegisteredAlbum ra) {
					model.EditAlbum(ra);
				}
			}).AddTo(this.CompositeDisposable);
		}
	}
}
