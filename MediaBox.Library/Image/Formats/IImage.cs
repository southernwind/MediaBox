namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// 画像ファイルメタデータ取得インターフェイス
	/// </summary>
	public interface IImage {
		int Width {
			get;
		}

		int Height {
			get;
		}
	}
}
