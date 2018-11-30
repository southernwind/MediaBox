using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Album {
	class AlbumCreatorViewModel : ViewModelBase {

		private AlbumCreator _model;

		/// <summary>
		/// メディア選択用アルバムコンテナー
		/// </summary>
		public AlbumContainerViewModel AlbumContainerViewModel {
			get;
		}

		/// <summary>
		/// 作成するアルバム
		/// </summary>
		public ReadOnlyReactiveProperty<AlbumViewModel> AlubmViewModel {
			get;
		}

		/// <summary>
		/// アルバムに追加する候補メディア
		/// </summary>
		public ReactiveProperty<MediaFileViewModel> CandidateMediaFile {
			get;
		} = new ReactiveProperty<MediaFileViewModel>();

		/// <summary>
		/// アルバムに監視ディレクトリを追加する
		/// </summary>
		public ReactiveCommand<string> AddMonitoringDirectoryCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// 候補メディアをアルバムに追加するコマンド
		/// </summary>
		public ReactiveCommand AddFromCandidateCommand {
			get;
		} = new ReactiveCommand();

		public AlbumCreatorViewModel() {
			this._model = Get.Instance<AlbumCreator>();
			this.AlbumContainerViewModel = Get.Instance<AlbumContainerViewModel>();
			this.AlubmViewModel = this._model.Album.Select(x => Get.Instance<AlbumViewModel>().Initialize(x)).ToReadOnlyReactiveProperty();

			this.AddFromCandidateCommand.Subscribe(_ => {
				this._model.AddFromCandidate(this.CandidateMediaFile.Value.Model);
				this.CandidateMediaFile.Value = null;
			});

			this.AddMonitoringDirectoryCommand.Subscribe(x => {
				this._model.AddDirectory(x);
			});
		}
	}
}
