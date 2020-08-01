using System.Data.Common;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.DataBase {
	public class MediaBoxDbContext : DbContext, IMediaBoxDbContext {
		private readonly DbConnection _dbConnection;

		/// <summary>
		/// アルバムテーブル
		/// </summary>
		public DbSet<Album> Albums {
			get;
			set;
		}

		/// <summary>
		/// アルバムボックステーブル
		/// </summary>
		public DbSet<AlbumBox> AlbumBoxes {
			get;
			set;
		}

		/// <summary>
		/// アルバムディレクトリ
		/// </summary>
		public DbSet<AlbumScanDirectory> AlbumScanDirectories {
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
			modelBuilder.Entity<AlbumBox>().HasKey(ab => ab.AlbumBoxId);
			modelBuilder.Entity<AlbumScanDirectory>().HasKey(ad => new { ad.AlbumId, ad.Directory });
			modelBuilder.Entity<AlbumMediaFile>().HasKey(amf => new { amf.AlbumId, amf.MediaFileId });

			// Relation
			modelBuilder.Entity<Album>()
				.HasOne(a => a.AlbumBox)
				.WithMany(ab => ab.Albums)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<AlbumBox>()
				.HasOne(ab => ab.Parent)
				.WithMany(ab => ab.Children)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<AlbumMediaFile>()
				.HasOne(amf => amf.Album)
				.WithMany(a => a.AlbumMediaFiles)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<AlbumScanDirectory>()
				.HasOne(ad => ad.Album)
				.WithMany(a => a.AlbumScanDirectories)
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
