using System;

using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// ファイルタイプフィルタークリエイター
	/// </summary>
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
		/// <param name="type">型</param>
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

		/// <summary>
		/// 型を対応する文字列に変換
		/// </summary>
		/// <param name="type">型</param>
		/// <returns>変換後文字列</returns>
		private string TypeToString(Type type) {
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
