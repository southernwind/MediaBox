using System.Windows;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Controls.AttachedProperties {
	/// <summary>
	/// ヘルパー添付プロパティ
	/// </summary>
	public static class Helper {

		/// <summary>
		/// マスク添付プロパティ
		/// </summary>
		public static readonly DependencyProperty IsMaskedProperty
			= DependencyProperty.RegisterAttached("IsMasked",
				typeof(bool),
				typeof(Helper),
				new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		/// <summary>
		/// マスクがかけられているかどうかの値を取得する
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
		[AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
		public static bool GetIsMasked(UIElement element) {
			return (bool)element.GetValue(IsMaskedProperty);
		}

		/// <summary>
		/// マスクがかけられているかどうかの値を設定する。
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(ListBoxItem))]
		[AttachedPropertyBrowsableForType(typeof(TreeViewItem))]
		public static void SetIsMasked(UIElement element, bool value) {
			element.SetValue(IsMaskedProperty, value);
		}
	}
}