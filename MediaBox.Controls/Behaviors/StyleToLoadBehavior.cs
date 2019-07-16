using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace SandBeige.MediaBox.Controls.Behaviors {
	public class ApplyStyleBehavior {
		public static readonly DependencyProperty StyleProperty =
			DependencyProperty.RegisterAttached("Style", typeof(object), typeof(ApplyStyleBehavior), new PropertyMetadata(null,
				(o, e) => {
					if (e.NewValue is Style s) {
						if (o is ContentPresenter cp) {
							cp.Resources.Add(s.TargetType, s);
							if (cp.Content is FrameworkElement fe2) {
								if (!fe2.Resources.Keys.OfType<object>().Contains(s.TargetType)) {
									fe2.Resources.Add(s.TargetType, s);
								}
							}
						}
					}
				}));

		public static object GetStyle(DependencyObject obj) {
			return obj.GetValue(StyleProperty);
		}

		public static void SetStyle(DependencyObject obj, object value) {
			obj.SetValue(StyleProperty, value);
		}

	}
}
