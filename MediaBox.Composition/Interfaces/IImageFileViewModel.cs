namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IImageFileViewModel : IMediaFileViewModel {
		/// <summary>
		/// フルサイズイメージ
		/// </summary>
		object Image {
			get;
		}
	}
}
