using System;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;

namespace SandBeige.MediaBox.Models.Album.History.Creator {
	/// <summary>
	/// 登録アルバム作成
	/// </summary>
	public class RegisterAlbumCreator : IAlbumCreator {

		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
			set;
		}

		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		[Obsolete("for serialize")]
		public RegisterAlbumCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="albumId">アルバムID</param>
		public RegisterAlbumCreator(
			string title,
			int albumId) {
			this.Title = title;
			this.AlbumId = albumId;
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
			var ra = new RegisteredAlbum(selector, settings, logging, gestureReceiver, rdb, mediaFactory, documentDb, notificationManager);
			try {
				ra.LoadFromDataBase(this.AlbumId);
			} catch (InvalidOperationException) {
				// アルバムがすでに消えていた場合
				return null;
			}
			return ra;
		}
	}
}
