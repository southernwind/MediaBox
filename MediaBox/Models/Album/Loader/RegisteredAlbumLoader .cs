using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Expressions;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Selector;
using SandBeige.MediaBox.Models.Notification;

namespace SandBeige.MediaBox.Models.Album.Loader {
	public class RegisteredAlbumLoader : AlbumLoader {
		private readonly IAlbumContainer _albumContainer;
		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
			private set;
		}

		/// <summary>
		/// 読み込み対象ディレクトリ
		/// </summary>
		public IEnumerable<string> Directories {
			get;
			private set;
		}

		/// <summary>
		/// ファイル追加通知
		/// </summary>
		public override IObservable<IEnumerable<IMediaFileModel>> OnAddFile {
			get {
				return this.mediaFileManager
					.OnRegisteredMediaFiles
					.Select(x => x.Where(
						m =>
							this.Directories.Any(
								d => m.FilePath.StartsWith(d))
							).Where(this.albumSelector.FilterSetter));
			}
		}

		/// <summary>
		/// アルバム定義更新通知
		/// </summary>
		public override IObservable<Unit> OnAlbumDefinitionUpdated {
			get {
				return this._albumContainer
					.AlbumUpdated
					.Where(x => x == this.AlbumId)
					.ToUnit();

			}
		}

		public RegisteredAlbumLoader(
			IMediaBoxDbContext rdb,
			IDocumentDb documentDb,
			IMediaFactory mediaFactory,
			AlbumSelector albumSelector,
			NotificationManager notificationManager,
			IMediaFileManager mediaFileManager,
			IAlbumContainer albumContainer) : base(rdb, documentDb, mediaFactory, albumSelector, notificationManager, mediaFileManager) {
			this._albumContainer = albumContainer;
		}


		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			// 普通に書くと↓で良い。
			// return mediaFile => mediaFile.AlbumMediaFiles.Any(x => x.AlbumId == this.AlbumId) ||
			//	this.Directories
			//		.Any(x => mediaFile.DirectoryPath.StartsWith(x));
			// ただ、これだとthis.Dictionaries.Any(...)の部分が大量の小さなSQLに分割されてしまうので、パフォーマンスがかなり落ちる。
			// .Any(...)は普通だとIN句に変換されるんだけど、中でStartsWithをやってしまっているので、IN句にできなくて仕方なく複数SQLに変換している模様？
			// これを、
			// this.AlbumId IN AlbumMediaFiles.AlbumId OR
			// this.Directories[0].StartsWith(mediaFile.DirectoryPath) OR
			// this.Directories[1].StartsWith(mediaFile.DirectoryPath) OR
			// this.Directories[2].StartsWith(mediaFile.DirectoryPath)
			// ...というようなSQLに変換させるため、式木を組み立てる。

			// アルバムIDは絶対に条件に含むので、これをベースに組み立てる
			var ids = this.rdb.AlbumMediaFiles.Where(x => x.AlbumId == this.AlbumId).Select(x => x.MediaFileId).OrderByDescending(x => x).ToArray();
			Expression<Func<MediaFile, bool>> exp1 = mediaFile => ids.Any(x => x == mediaFile.MediaFileId);
			var exp = exp1.Body;
			var visitor = new ParameterVisitor(exp1.Parameters);

			// ディレクトリ指定があれば
			foreach (var dir in this.Directories) {
				Expression<Func<MediaFile, bool>> exp2 = mediaFile => mediaFile.DirectoryPath.StartsWith(dir);
				exp = Expression.OrElse(exp, visitor.Visit(exp2.Body));
			}

			return Expression.Lambda<Func<MediaFile, bool>>(
				exp,
				visitor.Parameters
			);
		}

		/// <summary>
		/// 条件設定用アルバムオブジェクト設定
		/// </summary>
		/// <param name="albumObject">アルバムオブジェクト</param>
		public override void SetAlbumObject(IAlbumObject albumObject) {
			if (albumObject is not RegisteredAlbumObject rao) {
				throw new ArgumentException();
			}
			this.AlbumId = rao.AlbumId;
			lock (this.rdb) {
				var album =
					this.rdb
						.Albums
						.Include(x => x.AlbumScanDirectories)
						.Where(x => x.AlbumId == this.AlbumId)
						.Select(x => new { x.Title, x.AlbumBoxId, Directories = x.AlbumScanDirectories.Select(d => d.Directory) })
						.Single();
				this.Directories = album.Directories;
			}
		}
	}
}