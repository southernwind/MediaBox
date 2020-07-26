
using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.Box {
	public class AlbumForBoxModel : ModelBase {
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		public IReactiveProperty<int> Count {
			get;
		} = new ReactivePropertySlim<int>();

		public IReactiveProperty<int?> AlbumBoxId {
			get;
		} = new ReactivePropertySlim<int?>();

		public AlbumForBoxModel(string title, int count, int? albumBoxId) {
			this.Title.Value = title;
			this.Count.Value = count;
			this.AlbumBoxId.Value = albumBoxId;
		}
	}

	public static class AlbumForBoxModelExtensions {
		public static AlbumForBoxModel ToAlbumModelForAlbumBox(this RegisteredAlbumObject registeredAlbumObject, MediaBoxDbContext rdb) {
			lock (rdb) {
				var record = rdb.Albums.Where(x => x.AlbumId == registeredAlbumObject.AlbumId).Select(x => new { x.Title, x.AlbumBoxId, x.AlbumMediaFiles.Count }).First();
				return new AlbumForBoxModel(record.Title, record.Count, record.AlbumBoxId);
			}
		}
	}
}
