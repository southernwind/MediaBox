using System.Windows;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace SandBeige.MediaBox.Controls.Behaviors {
	public class DeviceEventBehaviorForUIElement : DeviceEventBehavior<UIElement> {
	}

	public class DeviceEventBehavior<T> : Behavior<T>
		where T : DependencyObject, IInputElement {
		public static readonly DependencyProperty KeyEventCommandProperty =
			DependencyProperty.RegisterAttached(
				nameof(KeyEventCommand),
				typeof(ICommand),
				typeof(DeviceEventBehavior<T>));

		public static readonly DependencyProperty MouseEventCommandProperty =
			DependencyProperty.RegisterAttached(
				nameof(MouseEventCommand),
				typeof(ICommand),
				typeof(DeviceEventBehavior<T>));
		public static readonly DependencyProperty MouseWheelEventCommandProperty =
			DependencyProperty.RegisterAttached(
				nameof(MouseWheelEventCommand),
				typeof(ICommand),
				typeof(DeviceEventBehavior<T>));

		public ICommand KeyEventCommand {
			get {
				return (ICommand)this.GetValue(KeyEventCommandProperty);
			}
			set {
				this.SetValue(KeyEventCommandProperty, value);
			}
		}

		public ICommand MouseEventCommand {
			get {
				return (ICommand)this.GetValue(MouseEventCommandProperty);
			}
			set {
				this.SetValue(MouseEventCommandProperty, value);
			}
		}

		public ICommand MouseWheelEventCommand {
			get {
				return (ICommand)this.GetValue(MouseWheelEventCommandProperty);
			}
			set {
				this.SetValue(MouseWheelEventCommandProperty, value);
			}
		}

		/// <summary>
		/// アタッチ
		/// </summary>
		protected override void OnAttached() {
			base.OnAttached();
			this.AssociatedObject.PreviewKeyDown += this.OnPreviewKeyEvent;
			this.AssociatedObject.PreviewKeyUp += this.OnPreviewKeyEvent;
			this.AssociatedObject.PreviewMouseWheel += this.OnPreviewMouseWheelEvent;
			this.AssociatedObject.PreviewMouseLeftButtonDown += this.OnPreviewMouseEvent;
			this.AssociatedObject.PreviewMouseLeftButtonUp += this.OnPreviewMouseEvent;
			this.AssociatedObject.PreviewMouseRightButtonDown += this.OnPreviewMouseEvent;
			this.AssociatedObject.PreviewMouseRightButtonUp += this.OnPreviewMouseEvent;
			this.AssociatedObject.PreviewMouseMove += this.OnPreviewMouseEvent;
		}

		/// <summary>
		/// デタッチ
		/// </summary>
		protected override void OnDetaching() {
			base.OnDetaching();
			this.AssociatedObject.PreviewKeyDown -= this.OnPreviewKeyEvent;
			this.AssociatedObject.PreviewKeyUp -= this.OnPreviewKeyEvent;
			this.AssociatedObject.PreviewMouseWheel -= this.OnPreviewMouseWheelEvent;
			this.AssociatedObject.PreviewMouseLeftButtonDown -= this.OnPreviewMouseEvent;
			this.AssociatedObject.PreviewMouseLeftButtonUp -= this.OnPreviewMouseEvent;
			this.AssociatedObject.PreviewMouseRightButtonDown -= this.OnPreviewMouseEvent;
			this.AssociatedObject.PreviewMouseRightButtonUp -= this.OnPreviewMouseEvent;
			this.AssociatedObject.PreviewMouseMove -= this.OnPreviewMouseEvent;
		}

		/// <summary>
		/// キーダウン
		/// </summary>
		/// <param name="sender">未使用</param>
		/// <param name="e">イベント引数</param>
		private void OnPreviewKeyEvent(object sender, KeyEventArgs e) {
			this.KeyEventCommand?.Execute(e);
		}

		/// <summary>
		/// キーダウン
		/// </summary>
		/// <param name="sender">未使用</param>
		/// <param name="e">イベント引数</param>
		private void OnPreviewMouseWheelEvent(object sender, MouseWheelEventArgs e) {
			this.MouseWheelEventCommand?.Execute(e);
		}

		/// <summary>
		/// キーアップ
		/// </summary>
		/// <param name="sender">未使用</param>
		/// <param name="e">イベント引数</param>
		private void OnPreviewMouseEvent(object sender, MouseEventArgs e) {
			this.MouseEventCommand?.Execute(e);
		}
	}
}
