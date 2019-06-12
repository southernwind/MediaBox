using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Views.Map {
	/// <summary>
	/// MapControl.xaml の相互作用ロジック
	/// </summary>
	public partial class MapControl : IMapControl {
		public MapControl() {
			this.InitializeComponent();
		}

		/// <summary>
		/// 範囲プロパティの値が異常値か否か
		/// </summary>
		public bool HasAreaPropertyError {
			get {
				var result = false;
				this.Dispatcher.Invoke(() => {
					result =
						this.BoundingRectangle.Center.Longitude > this.Center.Longitude + 0.00001 ||
						this.BoundingRectangle.Center.Longitude < this.Center.Longitude - 0.00001 ||
						this.BoundingRectangle.West > this.BoundingRectangle.East;
				});
				return result;
			}
		}
	}
}
