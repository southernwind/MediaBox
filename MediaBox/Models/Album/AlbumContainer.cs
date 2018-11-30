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
	/// <summary>
	/// アルバムコンテナ
	/// </summary>
	class AlbumContainer :ModelBase{
		/// <summary>
		/// アルバム一覧
		/// </summary>
		public ReactiveCollection<Album> AlbumList {
			get;
		} = new ReactiveCollection<Album>();

		/// <summary>
		/// カレントアルバム
		/// </summary>
		public ReactiveProperty<Album> CurrentAlbum {
			get;
		} = new ReactiveProperty<Album>();

		/// <summary>
		/// 一時アルバムフォルダパス
		/// </summary>
		public ReactiveProperty<string> TemporaryAlbumPath {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumContainer() {
		}

		public AlbumContainer Initialize() {
			this.AlbumList.AddRangeOnScheduler(this.DataBase.Albums.Select(x => x.AlbumId).ToList().Select(x => Get.Instance<RegisteredAlbum>().Initialize(x)));
			return this;
		}

		/// <summary>
		/// 引数のアルバムをカレントする
		/// </summary>
		/// <param name="album"></param>
		public void SetAlbumToCurrent(Album album) {
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// 一時アルバムをカレントにする
		/// </summary>
		public void SetTemporaryAlbumToCurrent() {
			var album = Get.Instance<FolderAlbum>().Initialize(this.TemporaryAlbumPath.Value);
			this.CurrentAlbum.Value = album;
		}
	}
}
