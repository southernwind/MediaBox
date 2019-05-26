using System;

using SandBeige.MediaBox.Utilities;

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
		public IAlbumModel Create(IAlbumSelector selector) {
			var lda = Get.Instance<LookupDatabaseAlbum>(selector);
			lda.Title.Value = this.Title;
			lda.TagName = this.TagName;
			lda.LoadFromDataBase();
			return lda;
		}
	}
}
