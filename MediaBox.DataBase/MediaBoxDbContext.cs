using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using SandBeige.MediaBox.DataBase.Tables;
using Microsoft.Extensions.Logging;

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

		/// <summary>
		/// タグテーブル
		/// </summary>
		public DbSet<Tag> Tags {
			get;
			set;
		}

		/// <summary>
		/// メディアファイル・タグ中間テーブル
		/// </summary>
		public DbSet<MediaFileTag> MediaFileTags {
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
			// Primary Keys
			modelBuilder.Entity<MediaFile>().HasKey(mf => mf.MediaFileId);
			modelBuilder.Entity<MediaFileTag>().HasKey(mft => new { mft.MediaFileId, mft.TagId });
			modelBuilder.Entity<Tag>().HasKey(t => t.TagId);

			// Relation
			modelBuilder.Entity<MediaFileTag>()
				.HasOne(mft => mft.MediaFile)
				.WithMany(mf => mf.MediaFileTags)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<MediaFileTag>()
				.HasOne(mft => mft.Tag)
				.WithMany(t => t.MediaFileTags)
				.OnDelete(DeleteBehavior.Cascade);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			switch (this._dbConnection) {
				case SqliteConnection conn:
					optionsBuilder.UseSqlite(conn);
					break;
			}
#if DEBUG
			var factory = new LoggerFactory(new[] { new MediaBoxDbLoggerProvider() });
			optionsBuilder.UseLoggerFactory(factory);
#endif
		}
	}
}
