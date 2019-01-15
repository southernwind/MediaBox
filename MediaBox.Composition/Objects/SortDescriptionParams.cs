using System.ComponentModel;

namespace SandBeige.MediaBox.Composition.Objects {
	/// <summary>
	/// ソート順設定用パラメータ
	/// </summary>
	public class SortDescriptionParams {
		public string PropertyName {
			get;
			set;
		}

		public ListSortDirection Direction {
			get;
			set;
		}

		public SortDescriptionParams() {
		}
		public SortDescriptionParams(string propertyName, ListSortDirection direction) {
			this.PropertyName = propertyName;
			this.Direction = direction;
		}
	}
}
