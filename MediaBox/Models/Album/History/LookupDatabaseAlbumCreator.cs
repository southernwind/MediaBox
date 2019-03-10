using System;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album.History {
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
		/// 検索条件
		/// </summary>
		public Func<MediaFile, bool> WherePredicate {
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
		/// <param name="wherePredicate">検索条件</param>
		public LookupDatabaseAlbumCreator(string title, Func<MediaFile, bool> wherePredicate) {
			this.Title = title;
			this.WherePredicate = wherePredicate;
		}

		/// <summary>
		/// アルバムの作成
		/// </summary>
		/// <returns>作成されたアルバム</returns>
		public IAlbumModel Create() {
			var lda = Get.Instance<LookupDatabaseAlbum>(this.WherePredicate);
			lda.Title.Value = this.Title;
			return lda;
		}
	}
}
