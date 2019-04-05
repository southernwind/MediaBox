using System.Data.Common;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

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
		/// メディアファイルテーブル
		/// </summary>
		public DbSet<ImageFile> ImageFiles {
			get;
			set;
		}

		/// <summary>
		/// メディアファイルテーブル
		/// </summary>
		public DbSet<VideoFile> VideoFiles {
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

		/// <summary>
		/// Jpegメタデータテーブル
		/// </summary>
		public DbSet<Jpeg> Jpegs {
			get;
			set;
		}

		/// <summary>
		/// Pngメタデータテーブル
		/// </summary>
		public DbSet<Png> Pngs {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dbConnection"></param>
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
			modelBuilder.Entity<ImageFile>().HasKey(i => i.MediaFileId);
			modelBuilder.Entity<VideoFile>().HasKey(v => v.MediaFileId);
			modelBuilder.Entity<MediaFileTag>().HasKey(mft => new { mft.MediaFileId, mft.TagId });
			modelBuilder.Entity<Tag>().HasKey(t => t.TagId);
			modelBuilder.Entity<Jpeg>().HasKey(j => j.MediaFileId);
			modelBuilder.Entity<Png>().HasKey(p => p.MediaFileId);

			// Index
			modelBuilder.Entity<MediaFile>()
				.HasIndex(x => x.FilePath)
				.IsUnique();

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

			modelBuilder.Entity<ImageFile>()
				.HasOne(i => i.MediaFile)
				.WithOne(m => m.ImageFile)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<VideoFile>()
				.HasOne(v => v.MediaFile)
				.WithOne(m => m.VideoFile)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<MediaFileTag>()
				.HasOne(mft => mft.MediaFile)
				.WithMany(mf => mf.MediaFileTags)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<MediaFileTag>()
				.HasOne(mft => mft.Tag)
				.WithMany(t => t.MediaFileTags)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Jpeg>()
				.HasOne(j => j.MediaFile)
				.WithOne(m => m.Jpeg)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Png>()
				.HasOne(p => p.MediaFile)
				.WithOne(m => m.Png)
				.OnDelete(DeleteBehavior.Cascade);
		}

		/// <summary>
		/// 構成設定
		/// </summary>
		/// <param name="optionsBuilder">構成オブジェクト</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			switch (this._dbConnection) {
				case SqliteConnection conn:
					optionsBuilder.UseSqlite(conn);
					break;
			}
#if SQL_LOG
			var factory = new Microsoft.Extensions.Logging.LoggerFactory(new[] { new MediaBoxDbLoggerProvider() });
			optionsBuilder.UseLoggerFactory(factory);
#endif
		}
	}
}
