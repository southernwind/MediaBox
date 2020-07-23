using System;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;

namespace SandBeige.MediaBox.Models.Album.History.Creator {
	/// <summary>
	/// フォルダアルバム作成
	/// </summary>
	public class FolderAlbumCreator : IAlbumCreator {
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
			set;
		}

		/// <summary>
		/// フォルダパス
		/// </summary>
		public string FolderPath {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		[Obsolete("for serialize")]
		public FolderAlbumCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="folderPath">フォルダパス</param>
		public FolderAlbumCreator(string title, string folderPath) {
			this.Title = title;
			this.FolderPath = folderPath;
		}

		/// <summary>
		/// アルバムの作成
		/// </summary>
		/// <param name="selector">作成するアルバムを保有するセレクター</param>
		/// <returns>作成されたアルバム</returns>
		public IAlbumModel Create(IAlbumSelector selector,
			ISettings settings,
			ILogging logging,
			IGestureReceiver gestureReceiver,
			MediaBoxDbContext rdb,
			MediaFactory mediaFactory,
			DocumentDb documentDb,
			NotificationManager notificationManager) {
			return new FolderAlbum(this.FolderPath,
				selector,
				settings,
				logging,
				gestureReceiver,
				rdb,
				mediaFactory,
				documentDb,
				notificationManager);
		}
	}
}
