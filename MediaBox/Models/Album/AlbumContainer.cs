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
			// 登録フォルダ+DB読み込みアルバム
			var album = Get.Instance<RegisteredAlbum>().Initialize();
			this.AlbumList.Add(album);
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
