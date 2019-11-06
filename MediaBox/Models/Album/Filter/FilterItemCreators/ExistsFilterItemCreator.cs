using System;
using System.IO;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// ファイル存在フィルタークリエイター
	/// </summary>
	public class ExistsFilterItemCreator : IFilterItemCreator {
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
		public ExistsFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="exists">ファイルが存在するか否か</param>
		public ExistsFilterItemCreator(bool exists) {
			this.Exists = exists;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(
				x => File.Exists(x.FilePath) == this.Exists,
				x => x.Exists == this.Exists,
				false);
		}
		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}