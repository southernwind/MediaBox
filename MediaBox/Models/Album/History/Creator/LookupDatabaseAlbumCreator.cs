using System;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;

namespace SandBeige.MediaBox.Models.Album.History.Creator {
	/// <summary>
	/// データベース検索アルバム作成
	/// </summary>
	public class LookupDatabaseAlbumCreator : IAlbumCreator {
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
			set;
		}

		/// <summary>
		/// 検索条件 タグ名
		/// </summary>
		public string TagName {
			get;
			set;
		}

		/// <summary>
		/// 検索条件 ワード
		/// </summary>
		public string Word {
			get;
			set;
		}

		/// <summary>
		/// 検索条件 場所
		/// </summary>
		public Address Address {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		[Obsolete("for serialize")]
		public LookupDatabaseAlbumCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		public LookupDatabaseAlbumCreator(string title) {
			this.Title = title;
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
			var lda = new LookupDatabaseAlbum(selector, settings, logging, gestureReceiver, rdb, mediaFactory, documentDb, notificationManager);
			lda.Title.Value = this.Title;
			lda.TagName = this.TagName;
			lda.Word = this.Word;
			lda.Address = this.Address;
			lda.LoadFromDataBase();
			return lda;
		}
	}
}
