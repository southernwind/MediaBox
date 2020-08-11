using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Map {
	public interface IGeoCodingManager {
		void Reverse(GpsLocation location);
	}
}