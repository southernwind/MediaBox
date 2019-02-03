using System;

using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	public class MediaTypeFilterItemCreator : IFilterItemCreator {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return this.TypeToString(this.Type);
			}
		}

		/// <summary>
		/// 型
		/// </summary>
		public Type Type {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public MediaTypeFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public MediaTypeFilterItemCreator(Type type) {
			this.Type = type;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(x => x.GetType() == this.Type);
		}

		public string TypeToString(Type type) {
			if (type == typeof(ImageFileModel)) {
				return "画像ファイル";
			} else if (type == typeof(VideoFileModel)) {
				return "動画ファイル";
			} else {
				throw new ArgumentException();
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}
