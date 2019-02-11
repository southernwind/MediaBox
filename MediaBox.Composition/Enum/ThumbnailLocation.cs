using System;

namespace SandBeige.MediaBox.Composition.Enum {

	/// <summary>
	/// サムネイル生成場所
	/// </summary>
	[Flags]
	public enum ThumbnailLocation {
		/// <summary>
		/// なし
		/// </summary>
		None = 0x0,
		/// <summary>
		/// ファイル
		/// </summary>
		File = 0x1,
		/// <summary>
		/// メモリ上
		/// </summary>
		Memory = 0x2
	}
}
