
using LiteDB;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.DataBase {
	public class DocumentDb : IDocumentDb {
		private readonly LiteDatabase _db;
		public ILiteCollection<MediaFile> GetMediaFilesCollection() {
			return this._db.GetCollection<MediaFile>("mediaFiles");
		}

		public ILiteCollection<Position> GetPositionsCollection() {
			return this._db.GetCollection<Position>("positions");
		}

		/// <summary>
		/// テスト用全コレクション削除
		/// </summary>
		public void DropAllCollectionForTest() {
			this._db.DropCollection("mediaFiles");
			this._db.DropCollection("positions");
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="connectionString">接続文字列</param>
		public DocumentDb(string connectionString) {
			this._db = new LiteDatabase(connectionString);
		}

		public void Dispose() {
			this._db.Dispose();
		}
	}
}
