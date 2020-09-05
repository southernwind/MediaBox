using System;
using System.ComponentModel;

namespace SandBeige.MediaBox.Composition.Objects {
	/// <summary>
	/// ソート順設定用パラメータ
	/// </summary>
	public class SortDescriptionParams : IEquatable<SortDescriptionParams> {
		/// <summary>
		/// プロパティ名
		/// </summary>
		public string PropertyName {
			get;
			set;
		}

		/// <summary>
		/// ソート方向
		/// </summary>
		public ListSortDirection Direction {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public SortDescriptionParams() {
			this.PropertyName = null!;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		/// <param name="direction">ソート方向</param>
		public SortDescriptionParams(string propertyName, ListSortDirection direction) {
			this.PropertyName = propertyName;
			this.Direction = direction;
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.PropertyName}>";
		}

		public bool Equals(SortDescriptionParams? other) {
			return this.PropertyName == other?.PropertyName && this.Direction == other?.Direction;
		}
	}
}
