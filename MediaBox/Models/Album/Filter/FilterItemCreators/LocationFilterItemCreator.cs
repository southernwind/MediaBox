using System;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// 座標に関するフィルタークリエイター
	/// </summary>
	/// <remarks>
	/// どのプロパティに値を代入するかで複数の役割を持つ
	/// ・地名フィルター
	/// ・座標情報を含むか否かのフィウrター
	/// ・座標範囲フィルター
	/// </remarks>
	public class LocationFilterItemCreator : IFilterItemCreator {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				if (this.Text != null) {
					return $"{this.Text}を地名に含む";
				}
				if (this.Contains is { } b) {
					return $"座標情報を含{(b ? "む" : "まない")}";
				}
				if (this.LeftTop != null && this.RightBottom != null) {
					return $"[{this.LeftTop}]と[{this.RightBottom}]の範囲内";
				}

				return "エラー";
			}
		}

		/// <summary>
		/// 地名に含まれる文字列
		/// </summary>
		public string Text {
			get;
			set;
		}

		/// <summary>
		/// 座標情報を含む/含まない
		/// </summary>
		public bool? Contains {
			get;
			set;
		}

		/// <summary>
		/// 左上座標
		/// </summary>
		public GpsLocation LeftTop {
			get;
			set;
		}

		/// <summary>
		/// 右上座標
		/// </summary>
		public GpsLocation RightBottom {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public LocationFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="text">パスに含まれる文字列</param>
		public LocationFilterItemCreator(string text) {
			this.Text = text;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			if (this.Text != null) {
				return new FilterItem(x => x.Position.DisplayName.Contains(this.Text));
			}
			if (this.Contains is { } b) {
				return new FilterItem(x => (x.Latitude == null && x.Longitude == null) != b);
			}
			if (this.LeftTop != null && this.RightBottom != null) {
				return new FilterItem(x =>
					this.LeftTop.Latitude > x.Latitude &&
					x.Latitude > this.RightBottom.Latitude &&
					this.LeftTop.Longitude < x.Longitude &&
					this.RightBottom.Longitude > x.Longitude);
			}

			return new FilterItem(x => false);
		}
		public override string ToString() {
			return $"<[{base.ToString()}] {this.Text}>";
		}
	}
}
