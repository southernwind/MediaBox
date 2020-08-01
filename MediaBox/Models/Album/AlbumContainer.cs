using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムコンテナ
	/// </summary>
	/// <remarks>
	/// DIコンテナによってSingletonとして扱われる。
	/// <see cref="RegisteredAlbum"/>を生成可能なすべてのアルバムIDを保持する。
	/// </remarks>
	public class AlbumContainer : ModelBase, IAlbumContainer {
		private readonly Subject<int> _albumUpdatedSubject = new Subject<int>();
		private readonly IMediaBoxDbContext _rdb;
		public IObservable<int> AlbumUpdated {
			get {
				return this._albumUpdatedSubject.AsObservable();
			}
		}
		/// <summary>
		/// アルバムリスト
		/// </summary>
		public ReactiveCollection<int> AlbumList {
			get;
		} = new ReactiveCollection<int>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumContainer(IMediaBoxDbContext rdb) {
			this._rdb = rdb;
			lock (this._rdb) {
				// アルバムリスト初期読み込み
				this.AlbumList.AddRange(this._rdb.Albums.Select(x => x.AlbumId));
			}
		}

		/// <summary>
		/// アルバム追加
		/// </summary>
		/// <param name="albumId">追加対象アルバムID</param>
		public void AddAlbum(int albumId) {
			this.AlbumList.Add(albumId);
		}

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="albumObject">削除対象アルバム</param>
		public void RemoveAlbum(IAlbumObject albumObject) {
			if (albumObject is not RegisteredAlbumObject rao) {
				throw new ArgumentException();
			}

			lock (this._rdb) {
				this._rdb.Remove(this._rdb.Albums.Single(x => x.AlbumId == rao.AlbumId));
				this._rdb.SaveChanges();
			}
			this.AlbumList.Remove(rao.AlbumId);
		}

		/// <summary>
		/// アルバム更新通知発行
		/// </summary>
		/// <param name="albumId">アルバムID</param>
		public void OnAlbumUpdated(int albumId) {
			this._albumUpdatedSubject.OnNext(albumId);
		}
	}
}
