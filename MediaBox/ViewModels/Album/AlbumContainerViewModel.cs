using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムコンテナViewModel
	/// </summary>
	class AlbumContainerViewModel:ViewModelBase {
		/// <summary>
		/// モデル
		/// </summary>
		private AlbumContainer _model;

		/// <summary>
		/// アルバム一覧
		/// </summary>
		public ReadOnlyReactiveCollection<AlbumViewModel> AlbumList {
			get;
		}

		/// <summary>
		/// カレントアルバム
		/// </summary>
		public ReadOnlyReactiveProperty<AlbumViewModel> CurrentAlbum {
			get;
		}

		/// <summary>
		/// 一時アルバムフォルダパス
		/// </summary>
		public ReactiveProperty<string> TemporaryAlbumPath {
			get;
		}

		/// <summary>
		/// 一時アルバムをカレントにするコマンド
		/// </summary>
		public ReactiveCommand SetTemporaryAlbumToCurrent {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumContainerViewModel() {
			this._model = Get.Instance<AlbumContainer>();

			this.AlbumList = this._model.AlbumList.ToReadOnlyReactiveCollection(x => Get.Instance<AlbumViewModel>().Initialize(x));

			this.CurrentAlbum =
				this._model
					.CurrentAlbum
					.Where(x => x != null)
					.Select(x => this.AlbumList.FirstOrDefault(a => a.Model == x) ?? Get.Instance<AlbumViewModel>().Initialize(x))
					.ToReadOnlyReactiveProperty();


			this.TemporaryAlbumPath = this._model.TemporaryAlbumPath.ToReactivePropertyAsSynchronized(x => x.Value);

			this.SetTemporaryAlbumToCurrent.Subscribe(this._model.SetTemporaryAlbumToCurrent);
		}
	}
}
