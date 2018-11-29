using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	class AlbumCreator : ModelBase {
		private readonly AlbumContainer _albumContainer;

		/// <summary>
		/// 作成するアルバム
		/// </summary>
		public ReactiveProperty<RegisteredAlbum> Album {
			get;
		} = new ReactiveProperty<RegisteredAlbum>();

		public AlbumCreator() {
			this._albumContainer = Get.Instance<AlbumContainer>();
			this.Album.Value = Get.Instance<RegisteredAlbum>();
			this.Album.Value.Title.Value = "無題";

			
		}

		public void AddFromCandidate(MediaFile mediaFile) {
			this.Album.Value.AddFile(mediaFile);
		}
	}
}
