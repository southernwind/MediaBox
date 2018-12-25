using System.Data.Common;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.DataBase {
	public class MediaBoxDbContext : DbContext {
		private readonly DbConnection _dbConnection;

		/// <summary>
		/// アルバムテーブル
		/// </summary>
		public DbSet<Album> Albums {
			get;
			set;
		}

		/// <summary>
		/// アルバムディレクトリ
		/// </summary>
		public DbSet<AlbumDirectory> AlbumDirectories {
			get;
			set;
		}

		/// <summary>
		/// アルバム・メディアファイル中間テーブル
		/// </summary>
		public DbSet<AlbumMediaFile> AlbumMediaFiles {
			get;
			set;
		}

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
			modelBuilder.Entity<Album>().HasKey(a => a.AlbumId);
			modelBuilder.Entity<AlbumDirectory>().HasKey(ad => new { ad.AlbumId, ad.Directory });
			modelBuilder.Entity<AlbumMediaFile>().HasKey(amf => new { amf.AlbumId, amf.MediaFileId });
			modelBuilder.Entity<MediaFile>().HasKey(mf => mf.MediaFileId);
			modelBuilder.Entity<MediaFileTag>().HasKey(mft => new { mft.MediaFileId, mft.TagId });
			modelBuilder.Entity<Tag>().HasKey(t => t.TagId);

			// Relation
			modelBuilder.Entity<AlbumMediaFile>()
				.HasOne(amf => amf.Album)
				.WithMany(a => a.AlbumMediaFiles)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<AlbumDirectory>()
				.HasOne(ad => ad.Album)
				.WithMany(a => a.AlbumDirectories)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<AlbumMediaFile>()
				.HasOne(amf => amf.MediaFile)
				.WithMany(a => a.AlbumMediaFiles)
				.OnDelete(DeleteBehavior.Cascade);

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
