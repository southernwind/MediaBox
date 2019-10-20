using System.Windows;
using System.Windows.Controls;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Plugins.Viewer.TileStyle {
	internal class MediaDataTemplateSelector : DataTemplateSelector {
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if (item is IVideoFileViewModel) {
				return (DataTemplate)((FrameworkElement)container).FindResource("VideoTemplate");
			}

			return (DataTemplate)((FrameworkElement)container).FindResource("ImageTemplate");
		}
	}
}
