namespace SandBeige.MediaBox.Composition.Utilities {
	public interface IChecker {
		bool IsTargetExtension(string path);
		bool IsVideoExtension(string path);
	}
}