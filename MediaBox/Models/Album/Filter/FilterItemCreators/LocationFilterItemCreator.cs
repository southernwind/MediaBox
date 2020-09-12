
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// 座標に関するフィルタークリエイター
	/// </summary>
	/// <remarks>
	/// どのプロパティに値を代入するかで複数の役割を持つ
	/// ・地名フィルター
	/// ・座標情報を含むか否かのフィルター
	/// ・座標範囲フィルター
	/// </remarks>
	public class LocationFilterItemCreator : IFilterItemCreator<LocationFilterItemObject> {
		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create(LocationFilterItemObject filterItemObject) {
			if (filterItemObject.Text != null) {
				return new FilterItem(
					x => x.Position!.DisplayName!.Contains(filterItemObject.Text),
					// TODO : とりあえず現状では素通ししておく。モデル側にもロケーション名の情報を読み込む必要がある。ロケーション名は後から生成されることもあるので、生成されたときにモデル側にも反映する必要もあり。結構大掛かりになりそうなのであとまわし
					x => true,
					true);
			}
			if (filterItemObject.Contains is { } b) {
				return new FilterItem(
					x => (x.Latitude == null && x.Longitude == null) != b,
					x => (x.Location == null) != b,
					true);
			}
			if (filterItemObject.LeftTop != null && filterItemObject.RightBottom != null) {
				return new FilterItem(x =>
						filterItemObject.LeftTop.Latitude > x.Latitude &&
					x.Latitude > filterItemObject.RightBottom.Latitude &&
					filterItemObject.LeftTop.Longitude < x.Longitude &&
					filterItemObject.RightBottom.Longitude > x.Longitude,
					x => filterItemObject.LeftTop > x.Location && x.Location > filterItemObject.RightBottom,
					true);
			}

			return new FilterItem(x => false, x => false, true);
		}
	}
}
