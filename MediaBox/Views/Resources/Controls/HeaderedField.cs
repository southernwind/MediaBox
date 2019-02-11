using System.Windows;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Views.Resources.Controls {
	/// <summary>
	/// ヘッダ付きフィールド
	/// </summary>
	internal class HeaderedField : HeaderedContentControl {
		/// <summary>
		/// ヘッダ部SharedSizeGroup 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty HeaderSharedSizeGroupProperty =
			DependencyProperty.Register(nameof(HeaderSharedSizeGroup),
				typeof(string),
				typeof(HeaderedField),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) => {

				}));

		/// <summary>
		/// ヘッダ部SharedSizeGroup CLR用
		/// </summary>
		public string HeaderSharedSizeGroup {
			get {
				return (string)this.GetValue(HeaderSharedSizeGroupProperty);
			}
			set {
				this.SetValue(HeaderSharedSizeGroupProperty, value);
			}
		}
	}
}
