using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.DataBase {
	public class Database{
		private readonly LiteDatabase _db;
		public LiteCollection<MediaFile> GetMediaFilesCollection() {
			return this._db.GetCollection<MediaFile>("mediaFiles");
		}

		public LiteCollection<Position> GetPositionsCollection() {
			return this._db.GetCollection<Position>("positions");
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="connectionString">接続文字列</param>
		public Database(string connectionString) {
			this._db = new LiteDatabase(connectionString);
		}
	}
}
