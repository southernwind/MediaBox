using System;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// ファイルタイプフィルタークリエイター
	/// </summary>
	public class MediaTypeFilterItemCreator : IFilterItemCreator {
		private readonly ISettings _settings;
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				if (this.IsVideo) {
					return "動画ファイル";
				} else {
					return "画像ファイル";
				}
			}
		}

		/// <summary>
		/// 動画ファイルか否か
		/// </summary>
		public bool IsVideo {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public MediaTypeFilterItemCreator() {
			this._settings = null!;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isVideo">動画ファイルか否か</param>
		public MediaTypeFilterItemCreator(bool isVideo, ISettings settings) {
			this._settings = settings;
			this.IsVideo = isVideo;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(
				x => x.FilePath.IsVideoExtension(this._settings) == this.IsVideo,
				x => x.FilePath.IsVideoExtension(this._settings) == this.IsVideo,
				false);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}
