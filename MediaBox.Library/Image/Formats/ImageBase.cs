using System.IO;

using MetadataExtractor;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// 各種イメージフォーマットの基底クラス
	/// </summary>
	/// <remarks>
	/// コンストラクタで受け取ったStreamをDisposeする。
	/// </remarks>
	public abstract class ImageBase : IImage {
		private bool _disposedValue;
		private readonly Stream _stream;

		/// <summary>
		/// 幅
		/// </summary>
		public abstract int Width {
			get;
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public abstract int Height {
			get;
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public virtual Rational[] Latitude {
			get {
				return null;
			}
		}

		/// <summary>
		/// 経度
		/// </summary>
		public virtual Rational[] Longitude {
			get {
				return null;
			}
		}

		/// <summary>
		/// 緯度方向(N/S)
		/// </summary>
		public virtual string LatitudeRef {
			get {
				return null;
			}
		}

		/// <summary>
		/// 経度方向(E/W)
		/// </summary>
		public virtual string LongitudeRef {
			get {
				return null;
			}
		}

		/// <summary>
		/// 画像の方向
		/// </summary>
		public virtual int? Orientation {
			get {
				return null;
			}
		}

		/// <summary>
		/// メタデータの値と名前のペアのリストをを持つタグディレクトリのリスト
		/// </summary>
		public abstract Attributes<Attributes<string>> Properties {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">破棄するためのStreamオブジェクト</param>
		protected internal ImageBase(Stream stream) {
			this._stream = stream;
		}

		/// <summary>
		/// Dispose
		/// </summary>
		/// <param name="disposing">マネージドリソースを破棄するか</param>
		protected virtual void Dispose(bool disposing) {
			if (!this._disposedValue) {
				if (disposing) {
					this._stream?.Dispose();
				}
				this._disposedValue = true;
			}
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
		}
	}
}
