using System;
using System.IO;
using System.Linq;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Pngメタデータ取得クラス
	/// </summary>
	public class Png : IImage {
		/// <summary>
		/// 幅
		/// </summary>
		public int Width {
			get;
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public int Height {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream"></param>
		internal Png(Stream stream) {
			var buff = new byte[24];
			stream.Read(buff, 0, buff.Length);
			var span = buff.AsSpan();
			// pngの場合はバイナリの16~19バイト目が幅、20~23バイト目が高さと決まっている。
			this.Width = BitConverter.ToInt32(span.Slice(16, 4).ToArray().Reverse().ToArray(), 0);
			this.Height = BitConverter.ToInt32(span.Slice(20, 4).ToArray().Reverse().ToArray(), 0);
		}
	}
}
