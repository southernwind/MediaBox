namespace SandBeige.MediaBox.Utilities {
	public interface IChecker {
		bool IsTargetExtension(string path);
		bool IsVideoExtension(string path);
	}
}