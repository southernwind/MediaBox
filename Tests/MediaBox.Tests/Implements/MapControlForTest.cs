using System;
using System.Windows;
using System.Windows.Input;

using Microsoft.Maps.MapControl.WPF;

using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Tests.Implements {
	internal class MapControlForTest : IMapControl {
#pragma warning disable CS0067 // The event 'event' is never used
		public Location ViewportPointToLocation(Point viewportPoint) {
			return new Location();
		}

		public Point LocationToViewportPoint(Location location) {
			return new Point();
		}

		public double ActualWidth {
			get;
		}

		public double ActualHeight {
			get;
		}

		public bool HasAreaPropertyError {
			get;
			set;
		}

		public event EventHandler<MapEventArgs> ViewChangeOnFrame;
		public event MouseButtonEventHandler MouseDoubleClick;

		public void RaiseEvent(RoutedEventArgs e) {
			throw new NotImplementedException();
		}

		public void AddHandler(RoutedEvent routedEvent, Delegate handler) {
			throw new NotImplementedException();
		}

		public void RemoveHandler(RoutedEvent routedEvent, Delegate handler) {
			throw new NotImplementedException();
		}

		public bool CaptureMouse() {
			throw new NotImplementedException();
		}

		public void ReleaseMouseCapture() {
			throw new NotImplementedException();
		}

		public bool CaptureStylus() {
			throw new NotImplementedException();
		}

		public void ReleaseStylusCapture() {
			throw new NotImplementedException();
		}

		public bool Focus() {
			throw new NotImplementedException();
		}

		public bool IsMouseOver {
			get;
		}
		public bool IsMouseDirectlyOver {
			get;
		}
		public bool IsMouseCaptured {
			get;
		}
		public bool IsStylusOver {
			get;
		}
		public bool IsStylusDirectlyOver {
			get;
		}
		public bool IsStylusCaptured {
			get;
		}
		public bool IsKeyboardFocusWithin {
			get;
		}
		public bool IsKeyboardFocused {
			get;
		}
		public bool IsEnabled {
			get;
		}
		public bool Focusable {
			get;
			set;
		}

		public event MouseButtonEventHandler PreviewMouseLeftButtonDown;
		public event MouseButtonEventHandler MouseLeftButtonDown;
		public event MouseButtonEventHandler PreviewMouseLeftButtonUp;
		public event MouseButtonEventHandler MouseLeftButtonUp;
		public event MouseButtonEventHandler PreviewMouseRightButtonDown;
		public event MouseButtonEventHandler MouseRightButtonDown;
		public event MouseButtonEventHandler PreviewMouseRightButtonUp;
		public event MouseButtonEventHandler MouseRightButtonUp;
		public event MouseEventHandler PreviewMouseMove;
		public event MouseEventHandler MouseMove;
		public event MouseWheelEventHandler PreviewMouseWheel;
		public event MouseWheelEventHandler MouseWheel;
		public event MouseEventHandler MouseEnter;
		public event MouseEventHandler MouseLeave;
		public event MouseEventHandler GotMouseCapture;
		public event MouseEventHandler LostMouseCapture;
		public event StylusDownEventHandler PreviewStylusDown;
		public event StylusDownEventHandler StylusDown;
		public event StylusEventHandler PreviewStylusUp;
		public event StylusEventHandler StylusUp;
		public event StylusEventHandler PreviewStylusMove;
		public event StylusEventHandler StylusMove;
		public event StylusEventHandler PreviewStylusInAirMove;
		public event StylusEventHandler StylusInAirMove;
		public event StylusEventHandler StylusEnter;
		public event StylusEventHandler StylusLeave;
		public event StylusEventHandler PreviewStylusInRange;
		public event StylusEventHandler StylusInRange;
		public event StylusEventHandler PreviewStylusOutOfRange;
		public event StylusEventHandler StylusOutOfRange;
		public event StylusSystemGestureEventHandler PreviewStylusSystemGesture;
		public event StylusSystemGestureEventHandler StylusSystemGesture;
		public event StylusButtonEventHandler StylusButtonDown;
		public event StylusButtonEventHandler PreviewStylusButtonDown;
		public event StylusButtonEventHandler PreviewStylusButtonUp;
		public event StylusButtonEventHandler StylusButtonUp;
		public event StylusEventHandler GotStylusCapture;
		public event StylusEventHandler LostStylusCapture;
		public event KeyEventHandler PreviewKeyDown;
		public event KeyEventHandler KeyDown;
		public event KeyEventHandler PreviewKeyUp;
		public event KeyEventHandler KeyUp;
		public event KeyboardFocusChangedEventHandler PreviewGotKeyboardFocus;
		public event KeyboardFocusChangedEventHandler GotKeyboardFocus;
		public event KeyboardFocusChangedEventHandler PreviewLostKeyboardFocus;
		public event KeyboardFocusChangedEventHandler LostKeyboardFocus;
		public event TextCompositionEventHandler PreviewTextInput;
		public event TextCompositionEventHandler TextInput;

#pragma warning restore CS0067 // The event 'event' is never used
	}
}
