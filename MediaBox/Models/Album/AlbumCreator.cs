using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	internal class AlbumCreator : ModelBase {
		private readonly AlbumContainer _albumContainer;

		/// <summary>
		/// 作成/編集するアルバム
		/// </summary>
		public ReactiveProperty<RegisteredAlbum> Album {
			get;
		} = new ReactiveProperty<RegisteredAlbum>();

		public AlbumCreator() {
			this._albumContainer = Get.Instance<AlbumContainer>();
		}

		/// <summary>
		/// アルバム新規作成
		/// </summary>
		public void CreateAlbum() {
			this.Album.Value = Get.Instance<RegisteredAlbum>();
			this.Album.Value.Create();
			this._albumContainer.AlbumList.Add(this.Album.Value);
		}

		/// <summary>
		/// アルバム編集
		/// </summary>
		/// <param name="album">編集するアルバム</param>
		public void EditAlbum(RegisteredAlbum album) {
			this.Album.Value = album;
		}

		/// <summary>
		/// ファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		public void AddFile(MediaFile mediaFile) {
			this.Album.Value.AddFile(mediaFile);
		}

		/// <summary>
		/// 監視ディレクトリ追加
		/// </summary>
		/// <param name="path"></param>
		public void AddDirectory(string path) {
			this.Album.Value.MonitoringDirectories.Add(path);
		}
	}
}
