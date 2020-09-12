
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// ファイルタイプフィルタークリエイター
	/// </summary>
	public class MediaTypeFilterItemCreator : IFilterItemCreator<MediaTypeFilterItemObject> {
		private readonly ISettings _settings;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaTypeFilterItemCreator(ISettings settings) {
			this._settings = settings;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create(MediaTypeFilterItemObject filterItemObject) {
			return new FilterItem(
				x => x.FilePath.IsVideoExtension(this._settings) == filterItemObject.IsVideo,
				x => x.FilePath.IsVideoExtension(this._settings) == filterItemObject.IsVideo,
				false);
		}
	}
}
