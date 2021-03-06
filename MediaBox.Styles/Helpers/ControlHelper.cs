using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Styles.Helpers
{
	public static class ControlHelper
	{
		public static readonly DependencyProperty InnerMarginProperty
			= DependencyProperty.RegisterAttached("InnerMargin",
				typeof(Thickness),
				typeof(ControlHelper),
				new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(Control))]
		public static Thickness GetInnerMargin(DependencyObject obj) {
			return (Thickness)obj.GetValue(InnerMarginProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(Control))]
		public static void SetInnerMargin(DependencyObject obj, Thickness value) {
			obj.SetValue(InnerMarginProperty, value);
		}

	}
}
