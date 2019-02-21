using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SandBeige.MediaBox.Controls.Controls {
	/// <summary>
	/// ImageEx.xaml の相互作用ロジック
	/// </summary>
	public partial class ImageEx {
		private Point _point;
		/// <summary>
		/// イメージソース 依存プロパティ
		/// </summary>
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register(
				nameof(Source),
				typeof(ImageSource),
				typeof(ImageEx),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		/// イメージソース
		/// </summary>
		public ImageSource Source {
			get {
				return (ImageSource)this.GetValue(SourceProperty);
			}
			set {
				this.SetValue(SourceProperty, value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImageEx() {
			this.InitializeComponent();
		}

		/// <summary>
		/// マウスホイール時
		/// </summary>
		/// <param name="e">イベントパラメータ</param>
		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			var matrix = this.Image.RenderTransform.Value;
			if (e.Delta > 0) {
				matrix.ScaleAt(1.25, 1.25, e.GetPosition(this.Image).X, e.GetPosition(this.Image).Y);
			} else if (matrix.Determinant > 1) {
				matrix.ScaleAt(0.8, 0.8, e.GetPosition(this.Image).X, e.GetPosition(this.Image).Y);
				if (matrix.Determinant < 1.0000001) {
					matrix.M11 = 1;
					matrix.M12 = 0;
					matrix.M21 = 0;
					matrix.M22 = 1;
					matrix.OffsetX = 0;
					matrix.OffsetY = 0;
				}
			}
			this.Image.RenderTransform = new MatrixTransform(matrix);
			e.Handled = true;

			base.OnMouseWheel(e);
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			this._point = e.GetPosition(this.Border);
			this.CaptureMouse();
			base.OnMouseLeftButtonDown(e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			this.ReleaseMouseCapture();
			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			if (this.IsMouseCaptured) {

				var matrix = this.Image.RenderTransform.Value;

				var v = this._point - e.GetPosition(this.Border);
				matrix.Translate(-v.X, -v.Y);
				this.Image.RenderTransform = new MatrixTransform(matrix);
				this._point = e.GetPosition(this.Border);
			}
			base.OnMouseMove(e);
		}
	}
}
