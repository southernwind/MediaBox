using System.Windows;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Controls.Controls {
	public class BindableSelectedItemTreeView : TreeView {
		public static readonly DependencyProperty BindableSelectedItemProperty
			= DependencyProperty.Register(
				nameof(BindableSelectedItem),
				typeof(object),
				typeof(BindableSelectedItemTreeView));

		/// <summary>
		/// Bind 可能な SelectedItem を表し、SelectedItem を設定または取得します。
		/// </summary>
		public object BindableSelectedItem {
			get {
				return this.GetValue(BindableSelectedItemProperty);
			}
			set {
				this.SetValue(BindableSelectedItemProperty, value);
			}
		}

		protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e) {
			this.BindableSelectedItem = this.SelectedItem;
			base.OnSelectedItemChanged(e);
		}
	}
}
