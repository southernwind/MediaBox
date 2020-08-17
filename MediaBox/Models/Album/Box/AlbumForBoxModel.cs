using System;
using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.DataBase;
namespace SandBeige.MediaBox.Models.Album.Box {
	public class AlbumForBoxModel : ModelBase, IAlbumForBoxModel {
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

		public AlbumForBoxModel(IAlbumObject albumObject, string title, int count, int? albumBoxId) {
			this.Title.Value = title;
			this.Count.Value = count;
			this.AlbumBoxId.Value = albumBoxId;
			this.AlbumObject = albumObject;
			this.AlbumBoxId.Subscribe(_ => {
				this.RaisePropertyChanged(nameof(this.AlbumBoxId));
			});
		}
	}

	public static class AlbumForBoxModelExtensions {
		public static AlbumForBoxModel ToAlbumModelForAlbumBox(this IAlbumObject registeredAlbumObject, int albumId, IMediaBoxDbContext rdb) {
			lock (rdb) {
				var record = rdb.Albums.Where(x => x.AlbumId == albumId).Select(x => new { x.Title, x.AlbumBoxId, x.AlbumMediaFiles.Count }).First();
				return new AlbumForBoxModel(registeredAlbumObject, record.Title, record.Count, record.AlbumBoxId);
			}
		}
	}
}
