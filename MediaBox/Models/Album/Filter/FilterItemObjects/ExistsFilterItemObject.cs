using System;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects {
	/// <summary>
	/// ファイル存在フィルターアイテムオブジェクト
	/// </summary>
	public class ExistsFilterItemObject : IFilterItemObject {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return $"ファイルが存在{(this.Exists ? "する" : "しない")}";
			}
		}
		/// <summary>
		/// ファイルが存在するか否か
		/// </summary>
		public bool Exists {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public ExistsFilterItemObject() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="exists">ファイルが存在するか否か</param>
		public ExistsFilterItemObject(bool exists) {
			this.Exists = exists;
		}
	}
}