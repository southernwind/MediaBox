using System;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Utilities;

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
		public RegisterAlbumCreator(string title, int albumId) {
			this.Title = title;
			this.AlbumId = albumId;
		}

		/// <summary>
		/// アルバムの作成
		/// </summary>
		/// <param name="filter">アルバムに適用するフィルター</param>
		/// <param name="sort">アルバムに適用するソート</param>
		/// <returns>作成されたアルバム</returns>
		public IAlbumModel Create(IFilterSetter filter, ISortSetter sort) {
			var ra = Get.Instance<RegisteredAlbum>(filter, sort);
			ra.LoadFromDataBase(this.AlbumId);
			return ra;
		}
	}
}
