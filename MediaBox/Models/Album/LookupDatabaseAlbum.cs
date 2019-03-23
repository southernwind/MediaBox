using System;
using System.Linq;

using Livet;

using Microsoft.EntityFrameworkCore;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.TaskQueue;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// データベース検索アルバム
	/// </summary>
	internal class LookupDatabaseAlbum : AlbumModel {

		/// <summary>
		/// 検索条件 タグ名
		/// </summary>
		public string TagName {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LookupDatabaseAlbum() : base(new ObservableSynchronizedCollection<IMediaFileModel>()) {
		}

		/// <summary>
		/// データベース読み込み
		/// </summary>
		public void LoadFromDataBase() {
			lock (this.Items.SyncRoot) {
				this.Items.Clear();
				this.Items.AddRange(
					this.DataBase
						.MediaFiles
						.Where(mf => mf.MediaFileTags.Select(x => x.Tag.TagName).Contains(this.TagName))
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
			foreach (var item in this.Items) {
				var ta = new TaskAction(
					$"ファイル情報読み込み[{item.FileName}]",
					item.GetFileInfoIfNotLoaded,
					Priority.LoadRegisteredAlbumOnLoad,
					this.CancellationToken
				);
				this.PriorityTaskQueue.AddTask(ta);
			}
		}
	}
}
