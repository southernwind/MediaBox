using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SandBeige.MediaBox.Controls.Behaviors {
	public class KeyEventBehaviorForUIElement : KeyEventBehavior<UIElement> {
	}

	public class KeyEventBehavior<T> : Behavior<T>
		where T : DependencyObject, IInputElement {
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached(
				nameof(Command),
				typeof(ICommand),
				typeof(KeyEventBehavior<T>));

		public ICommand Command {
			get {
				return (ICommand)this.GetValue(CommandProperty);
			}
			set {
				this.SetValue(CommandProperty, value);
			}
		}

		/// <summary>
		/// アタッチ
		/// </summary>
		protected override void OnAttached() {
			base.OnAttached();
			this.AssociatedObject.PreviewKeyDown += this.OnPreviewKeyDown;
			this.AssociatedObject.PreviewKeyUp += this.OnPreviewKeyUp;
		}

		/// <summary>
		/// デタッチ
		/// </summary>
		protected override void OnDetaching() {
			base.OnDetaching();
			this.AssociatedObject.PreviewKeyDown -= this.OnPreviewKeyDown;
			this.AssociatedObject.PreviewKeyUp -= this.OnPreviewKeyUp;
		}

		/// <summary>
		/// キーダウン
		/// </summary>
		/// <param name="sender">未使用</param>
		/// <param name="e">イベント引数</param>
		private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
			this.Command.Execute(e);
			e.Handled = true;
		}

		/// <summary>
		/// キーダウン
		/// </summary>
		/// <param name="sender">未使用</param>
		/// <param name="e">イベント引数</param>
		private void OnPreviewKeyUp(object sender, KeyEventArgs e) {
			this.Command.Execute(e);
			e.Handled = true;
		}
	}
}
