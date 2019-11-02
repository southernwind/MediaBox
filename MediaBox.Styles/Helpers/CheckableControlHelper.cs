using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SandBeige.MediaBox.Styles.Helpers {
	public static class CheckableControlHelper {
		#region CheckedBackground

		public static readonly DependencyProperty CheckedBackgroundBrushProperty
			= DependencyProperty.RegisterAttached("CheckedBackgroundBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetCheckedBackgroundBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(CheckedBackgroundBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetCheckedBackgroundBrush(DependencyObject obj, Brush value) {
			obj.SetValue(CheckedBackgroundBrushProperty, value);
		}

		#endregion

		#region CheckedBorder

		public static readonly DependencyProperty CheckedBorderBrushProperty
			= DependencyProperty.RegisterAttached("CheckedBorderBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetCheckedBorderBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(CheckedBorderBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetCheckedBorderBrush(DependencyObject obj, Brush value) {
			obj.SetValue(CheckedBorderBrushProperty, value);
		}

		#endregion

		#region CheckedForeground

		public static readonly DependencyProperty CheckedForegroundBrushProperty
			= DependencyProperty.RegisterAttached("CheckedForegroundBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetCheckedForegroundBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(CheckedForegroundBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetCheckedForegroundBrush(DependencyObject obj, Brush value) {
			obj.SetValue(CheckedForegroundBrushProperty, value);
		}

		#endregion

		#region HoverBackground

		public static readonly DependencyProperty HoverBackgroundBrushProperty
			= DependencyProperty.RegisterAttached("HoverBackgroundBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetHoverBackgroundBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(HoverBackgroundBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetHoverBackgroundBrush(DependencyObject obj, Brush value) {
			obj.SetValue(HoverBackgroundBrushProperty, value);
		}

		#endregion

		#region HoverBorder

		public static readonly DependencyProperty HoverBorderBrushProperty
			= DependencyProperty.RegisterAttached("HoverBorderBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetHoverBorderBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(HoverBorderBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetHoverBorderBrush(DependencyObject obj, Brush value) {
			obj.SetValue(HoverBorderBrushProperty, value);
		}

		#endregion

		#region HoverForeground

		public static readonly DependencyProperty HoverForegroundBrushProperty
			= DependencyProperty.RegisterAttached("HoverForegroundBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetHoverForegroundBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(HoverForegroundBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetHoverForegroundBrush(DependencyObject obj, Brush value) {
			obj.SetValue(HoverForegroundBrushProperty, value);
		}

		#endregion

		#region DisabledBackground

		public static readonly DependencyProperty DisabledBackgroundBrushProperty
			= DependencyProperty.RegisterAttached("DisabledBackgroundBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetDisabledBackgroundBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(DisabledBackgroundBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetDisabledBackgroundBrush(DependencyObject obj, Brush value) {
			obj.SetValue(DisabledBackgroundBrushProperty, value);
		}

		#endregion

		#region DisabledBorder

		public static readonly DependencyProperty DisabledBorderBrushProperty
			= DependencyProperty.RegisterAttached("DisabledBorderBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetDisabledBorderBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(DisabledBorderBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetDisabledBorderBrush(DependencyObject obj, Brush value) {
			obj.SetValue(DisabledBorderBrushProperty, value);
		}

		#endregion

		#region DisabledForeground

		public static readonly DependencyProperty DisabledForegroundBrushProperty
			= DependencyProperty.RegisterAttached("DisabledForegroundBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetDisabledForegroundBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(DisabledForegroundBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetDisabledForegroundBrush(DependencyObject obj, Brush value) {
			obj.SetValue(DisabledForegroundBrushProperty, value);
		}

		#endregion

		#region PressedBackground

		public static readonly DependencyProperty PressedBackgroundBrushProperty
			= DependencyProperty.RegisterAttached("PressedBackgroundBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetPressedBackgroundBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(PressedBackgroundBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetPressedBackgroundBrush(DependencyObject obj, Brush value) {
			obj.SetValue(PressedBackgroundBrushProperty, value);
		}

		#endregion

		#region PressedBorder

		public static readonly DependencyProperty PressedBorderBrushProperty
			= DependencyProperty.RegisterAttached("PressedBorderBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetPressedBorderBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(PressedBorderBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetPressedBorderBrush(DependencyObject obj, Brush value) {
			obj.SetValue(PressedBorderBrushProperty, value);
		}

		#endregion

		#region PressedForeground

		public static readonly DependencyProperty PressedForegroundBrushProperty
			= DependencyProperty.RegisterAttached("PressedForegroundBrush",
				typeof(Brush),
				typeof(CheckableControlHelper),
				new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static Brush GetPressedForegroundBrush(DependencyObject obj) {
			return (Brush)obj.GetValue(PressedForegroundBrushProperty);
		}

		[AttachedPropertyBrowsableForType(typeof(CheckBox))]
		[AttachedPropertyBrowsableForType(typeof(ToggleButton))]
		public static void SetPressedForegroundBrush(DependencyObject obj, Brush value) {
			obj.SetValue(PressedForegroundBrushProperty, value);
		}

		#endregion
	}
}
