using System;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container {
	public interface IAlbumContainer {
		public IObservable<int> AlbumUpdated {
			get;
		}
		/// <summary>
		/// アルバムリスト
		/// </summary>
		public ReactiveCollection<int> AlbumList {
			get;
		}

		/// <summary>
		/// アルバム追加
		/// </summary>
		/// <param name="albumId">追加対象アルバムID</param>
		public void AddAlbum(int albumId);

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバムID</param>
		public void RemoveAlbum(IAlbumObject album);

		/// <summary>
		/// アルバム更新通知発行
		/// </summary>
		/// <param name="albumId">アルバムID</param>
		public void OnAlbumUpdated(int albumId);
	}
}
