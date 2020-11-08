using System.Linq;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.Composition.Interfaces.Services.AlbumServices;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Services.AlbumServices {
	public class AlbumEditorService : IAlbumEditorService {
		private readonly IMediaBoxDbContext _rdb;
		private readonly IAlbumContainer _albumContainer;

		public AlbumEditorService(IMediaBoxDbContext rdb, IAlbumContainer albumContainer) {
			this._rdb = rdb;
			this._albumContainer = albumContainer;
		}

		public void RemoveFiles(int targetAlbumId, long[] mediaFileIds) {
			lock (this._rdb) {
				using var tran = this._rdb.Database.BeginTransaction();

				var deleteRows = this._rdb.AlbumMediaFiles.Where(x => x.AlbumId == targetAlbumId && mediaFileIds.Contains(x.MediaFileId));
				this._rdb.AlbumMediaFiles.RemoveRange(deleteRows);

				this._rdb.SaveChanges();
				tran.Commit();
			}
			this._albumContainer.OnAlbumUpdated(targetAlbumId);
		}

		public void AddFiles(int targetAlbumId, long[] mediaFileIds) {
			lock (this._rdb) {
				using var tran = this._rdb.Database.BeginTransaction();

				var addRows = mediaFileIds.Except(this._rdb.AlbumMediaFiles.Select(x => x.MediaFileId)).Select(x => new AlbumMediaFile {
					AlbumId = targetAlbumId,
					MediaFileId = x
				});
				this._rdb.AlbumMediaFiles.RemoveRange(addRows);

				this._rdb.SaveChanges();
				tran.Commit();
			}
			this._albumContainer.OnAlbumUpdated(targetAlbumId);
		}
	}
}
