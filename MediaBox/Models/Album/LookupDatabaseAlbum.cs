using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading;

using Microsoft.EntityFrameworkCore;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.TaskQueue;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// データベース検索アルバム
	/// </summary>
	internal class LookupDatabaseAlbum : AlbumModel {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LookupDatabaseAlbum() {

		}

		/// <summary>
		/// データベース読み込み
		/// </summary>
		/// <param name="wherePredicate">絞り込み条件</param>
		public void LoadFromDataBase(Func<MediaFile, bool> wherePredicate) {
			lock (this.Items.SyncRoot) {
				this.Items.Clear();
				this.Items.AddRange(
					this.DataBase
						.MediaFiles
						.Where(mf => wherePredicate(mf))
						.Include(mf => mf.MediaFileTags)
						.ThenInclude(mft => mft.Tag)
						.Include(mf => mf.ImageFile)
						.Include(mf => mf.VideoFile)
						.AsEnumerable()
						.Select(x => {
							var m = this.MediaFactory.Create(x.FilePath);
							m.LoadFromDataBase(x);
							return m;
						}).ToList()
				);
			}

			// 非同期で順次ファイル情報の読み込みを行う
			var count = this.Items.Count;
			var lockObj = new object();
			foreach (var item in this.Items) {
				var ta = new TaskAction(
					$"ファイル情報読み込み[{item.FileName}]",
					item.GetFileInfoIfNotLoaded,
					Priority.LoadRegisteredAlbumOnLoad,
					this.CancellationToken
				);
				ta.OnTaskCompleted.Subscribe(_ => {
					lock (lockObj) {
						if (--count == 0) {
							this.OnInitializedSubject.OnNext(Unit.Default);
						}
					}
				});
				this.PriorityTaskQueue.AddTask(ta);
			}
		}

		/// <summary>
		/// 未使用
		/// </summary>
		/// <param name="directoryPath">未使用</param>
		/// <param name="cancellationToken">未使用</param>
		protected override void LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken) {
		}

		/// <summary>
		/// 未使用
		/// </summary>
		/// <param name="e">未使用</param>
		protected override void OnFileSystemEvent(FileSystemEventArgs e) {
		}
	}
}
