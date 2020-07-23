using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;

namespace SandBeige.MediaBox.Models.Album.History.Creator {
	/// <summary>
	/// アルバム作成インターフェイス
	/// </summary>
	public interface IAlbumCreator {
		/// <summary>
		/// タイトル
		/// </summary>
		string Title {
			get;
		}

		/// <summary>
		/// アルバムの作成
		/// </summary>
		/// <param name="selector">作成するアルバムを保有するセレクター</param>
		/// <returns>作成されたアルバム</returns>
		IAlbumModel Create(IAlbumSelector selector,
			ISettings settings,
			ILogging logging,
			IGestureReceiver gestureReceiver,
			MediaBoxDbContext rdb,
			MediaFactory mediaFactory,
			DocumentDb documentDb,
			NotificationManager notificationManager);
	}
}
