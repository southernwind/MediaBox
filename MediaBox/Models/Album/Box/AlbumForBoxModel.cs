using System;
using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.DataBase;
namespace SandBeige.MediaBox.Models.Album.Box {
	public class AlbumForBoxModel : ModelBase, IAlbumForBoxModel {
		private readonly IAlbumLoader _albumLoader;
		private readonly IMediaBoxDbContext _rdb;

		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
		}

		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		public IReactiveProperty<int> Count {
			get;
		} = new ReactivePropertySlim<int>();

		public IReactiveProperty<int?> AlbumBoxId {
			get;
		} = new ReactivePropertySlim<int?>();

		public IAlbumObject AlbumObject {
			get;
		}

		public AlbumForBoxModel(IAlbumLoaderFactory albumLoaderFactory, IMediaBoxDbContext rdb, int albumId, IAlbumObject albumObject) {
			this._rdb = rdb;
			this.AlbumId = albumId;
			this.AlbumObject = albumObject;
			this.AlbumBoxId.Subscribe(_ => {
				this.RaisePropertyChanged(nameof(this.AlbumBoxId));
			});
			this._albumLoader = albumLoaderFactory.CreateWithoutSortAndFilter(albumObject);

			this._albumLoader.OnAlbumDefinitionUpdated.Subscribe(_ => this.Update());
		}

		public void Update() {
			lock (this._rdb) {
				var record = this._rdb.Albums.Where(x => x.AlbumId == this.AlbumId).Select(x => new { x.Title, x.AlbumBoxId }).First();
				this.Title.Value = record.Title;
				this.Count.Value = this._albumLoader.GetBeforeFilteringCount();
				this.AlbumBoxId.Value = record.AlbumBoxId!;
			}
		}
	}

	public static class AlbumForBoxModelExtensions {
		public static AlbumForBoxModel ToAlbumModelForAlbumBox(this IAlbumObject registeredAlbumObject, int albumId, IMediaBoxDbContext rdb, IAlbumLoaderFactory albumLoaderFactory) {
			var album = new AlbumForBoxModel(albumLoaderFactory, rdb, albumId, registeredAlbumObject);
			album.Update();
			return album;
		}
	}
}
