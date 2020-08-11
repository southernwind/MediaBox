using System.Windows;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Map {
	public interface IRectangle {
		Point LeftTop {
			get;
		}
		Size Size {
			get;
		}

		double DistanceTo(IRectangle rect);
		bool IncludedIn(Point point);
		bool IntersectsWith(IRectangle rect);
		string ToString();
	}
}