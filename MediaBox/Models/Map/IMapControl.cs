using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Maps.MapControl.WPF;

namespace SandBeige.MediaBox.Models.Map {
	public interface IMapControl : IInputElement {
		Location ViewportPointToLocation(Point viewportPoint);
		Point LocationToViewportPoint(Location location);
		event EventHandler<MapEventArgs> ViewChangeOnFrame;
		event MouseButtonEventHandler MouseDoubleClick;
	}
}
