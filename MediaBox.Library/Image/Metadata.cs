using System;
using System.IO;
using System.Reactive.Disposables;

using SandBeige.MediaBox.Library.Image.Formats;

namespace SandBeige.MediaBox.Library.Image {
	public class Metadata : IDisposable {
		private bool disposedValue = false;
		private readonly CompositeDisposable _disposable = new CompositeDisposable();
		private readonly IImage _image;
		/// <summary>
		/// 幅
		/// </summary>
		public int Width {
			get {
				return this._image.Width;
			}
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public int Height {
			get {
				return this._image.Height;
			}
		}

		public Metadata(Stream stream) {
			this._disposable.Add(stream);
			var buff = new byte[12];
			stream.Read(buff, 0, buff.Length);
			stream.Seek(0, SeekOrigin.Begin);
			var span = buff.AsSpan();
			if (span.Slice(0, 2).SequenceEqual(new byte[] { 0xff, 0xd8 })) {
				// Jpeg
				this._image = new Jpeg(stream);
			} else if (span.Slice(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) {
				// Png
				this._image = new Png(stream);
			} else {
				// TODO : heicなど他の拡張子対応対応
				throw new ArgumentException("未対応ファイル形式");
			}
		}

		protected virtual void Dispose(bool disposing) {
			if (!this.disposedValue) {
				if (disposing) {
					this._disposable.Dispose();
				}
				this.disposedValue = true;
			}
		}

		public void Dispose() {
			this.Dispose(true);
		}
	}
}
