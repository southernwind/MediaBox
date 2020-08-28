using System;
using System.IO;

using MetadataExtractor;

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
		public virtual Rational[]? Latitude {
			get {
				return null;
			}
		}

		/// <summary>
		/// 経度
		/// </summary>
		public virtual Rational[]? Longitude {
			get {
				return null;
			}
		}

		/// <summary>
		/// 高度
		/// </summary>
		public virtual Rational? Altitude {
			get;
		}

		/// <summary>
		/// 緯度方向(N/S)
		/// </summary>
		public virtual string? LatitudeRef {
			get {
				return null;
			}
		}

		/// <summary>
		/// 経度方向(E/W)
		/// </summary>
		public virtual string? LongitudeRef {
			get {
				return null;
			}
		}

		/// <summary>
		/// 高度方向(0/1)
		/// </summary>
		public virtual byte? AltitudeRef {
			get;
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
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">破棄するためのStreamオブジェクト</param>
		protected internal ImageBase(Stream stream) {
			this._stream = stream;
		}

		/// <summary>
		/// 文字取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		protected string? GetString(MetadataExtractor.Directory? directory, int tag) {
			if (directory == null) {
				return default;
			}
			return directory.GetString(tag);
		}

		/// <summary>
		/// 数値取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		protected short? GetShort(MetadataExtractor.Directory? directory, int tag) {
			if (directory == null) {
				return default;
			}
			if (directory.TryGetInt16(tag, out var value)) {
				return value;
			}
			return default;
		}

		/// <summary>
		/// 数値取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		protected int? GetInt(MetadataExtractor.Directory? directory, int tag) {
			if (directory == null) {
				return default;
			}
			if (directory.TryGetInt32(tag, out var value)) {
				return value;
			}
			return default;
		}

		/// <summary>
		/// 数値取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		protected double? GetDouble(MetadataExtractor.Directory? directory, int tag) {
			if (directory == null) {
				return default;
			}
			if (directory.TryGetDouble(tag, out var value)) {
				return value;
			}
			return default;
		}

		/// <summary>
		/// 日付取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		protected DateTime? GetDateTime(MetadataExtractor.Directory? directory, int tag) {
			if (directory == null) {
				return default;
			}
			if (directory.TryGetDateTime(tag, out var value)) {
				return value;
			}
			return default;
		}

		/// <summary>
		/// 文字型取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		protected byte[]? GetBinary(MetadataExtractor.Directory? directory, int tag) {
			if (directory == null) {
				return default;
			}
			return directory.GetByteArray(tag);
		}

		/// <summary>
		/// 文字型取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		protected (long?, long?) GetRational(MetadataExtractor.Directory? directory, int tag) {
			if (directory == null) {
				return default;
			}
			if (directory.TryGetRational(tag, out var value)) {
				return (value.Denominator, value.Numerator);
			}
			return default;
		}

		/// <summary>
		/// 文字型取得
		/// </summary>
		/// <param name="directory">ディレクトリ</param>
		/// <param name="tag">タグ</param>
		/// <returns>取得した値</returns>
		protected (double?, double?, double?) Get3Rational(MetadataExtractor.Directory? directory, int tag) {
			if (directory == null) {
				return default;
			}
			var value = directory.GetRationalArray(tag);
			if (value?.Length == 3) {
				return (value[0].ToDouble(), value[1].ToDouble(), value[2].ToDouble());
			}

			return default;
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
