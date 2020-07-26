using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Models.Album.AlbumObjects {
	/// <summary>
	/// データベース検索アルバム
	/// </summary>
	public class LookupDatabaseAlbumObject : IAlbumObject {
		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
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
	}
}
