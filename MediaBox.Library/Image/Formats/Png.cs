using System;
using System.IO;
using System.Linq;

namespace SandBeige.MediaBox.Library.Image.Formats {
	public class Png : IImage {
		public int Width {
			get;
		}

		public int Height {
			get;
		}

		internal Png(Stream stream) {
			var buff = new byte[24];
			stream.Read(buff, 0, buff.Length);
			var span = buff.AsSpan();
			this.Width = BitConverter.ToInt32(span.Slice(16, 4).ToArray().Reverse().ToArray(), 0);
			this.Height = BitConverter.ToInt32(span.Slice(20, 4).ToArray().Reverse().ToArray(), 0);
		}
	}
}
