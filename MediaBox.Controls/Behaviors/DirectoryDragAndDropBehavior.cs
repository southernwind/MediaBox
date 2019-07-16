using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace SandBeige.MediaBox.Controls.Behaviors {
	/// <summary>
	/// ディレクトリドラッグビヘイビア
	/// </summary>
	public class DirectoryDragAndDropBehavior : Behavior<UIElement> {
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached(
				nameof(Command),
				typeof(ICommand),
				typeof(DirectoryDragAndDropBehavior));

		public ICommand Command {
			get {
				return (ICommand)this.GetValue(CommandProperty);
			}
			set {
				this.SetValue(CommandProperty, value);
			}
		}

		protected override void OnAttached() {
			this.AssociatedObject.AllowDrop = true;
			this.AssociatedObject.PreviewDragOver += OnPreviewDragOver;
			this.AssociatedObject.Drop += this.OnDrop;
		}

		protected override void OnDetaching() {
			this.AssociatedObject.AllowDrop = false;
			this.AssociatedObject.PreviewDragOver -= OnPreviewDragOver;
			this.AssociatedObject.Drop -= this.OnDrop;
		}

		private static void OnPreviewDragOver(object sender, DragEventArgs e) {
			if (!(e.Data.GetData(DataFormats.FileDrop) is string[] folders)) {
				e.Effects = DragDropEffects.None;
			} else {
				e.Effects = folders.All(Directory.Exists) ? DragDropEffects.Copy : DragDropEffects.None;
			}
			e.Handled = true;
		}

		private void OnDrop(object sender, DragEventArgs e) {
			if (e.Data.GetData(DataFormats.FileDrop) is string[] folders) {
				this.Command.Execute(folders.Where(Directory.Exists));
			}
		}
	}
}
