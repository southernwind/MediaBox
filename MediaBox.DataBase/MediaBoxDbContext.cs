using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.DataBase {
	public class MediaBoxDbContext : DbContext {
		private readonly DbConnection _dbConnection;

		/// <summary>
		/// メディアファイルテーブル
		/// </summary>
		public DbSet<MediaFile> MediaFiles {
			get;
			set;
		}

		public MediaBoxDbContext(DbConnection dbConnection) {
			this._dbConnection = dbConnection;
		}

		/// <summary>
		/// テーブル設定
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<MediaFile>().HasKey(mf => mf.MediaFileId);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			switch (this._dbConnection) {
				case SqliteConnection conn:
					optionsBuilder.UseSqlite(conn);
					break;
			}
		}
	}
}
