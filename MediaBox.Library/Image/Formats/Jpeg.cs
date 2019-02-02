using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SandBeige.MediaBox.Library.Image.Formats {
	public class Jpeg : IImage {
		/// <summary>
		/// セグメント情報
		/// </summary>
		private IEnumerable<Segment> _segments;

		private readonly Stream _stream;

		/// <summary>
		/// 幅
		/// </summary>
		public int Width {
			get {
				return this.GetValue(new byte[] { 0xFF, 0xC0 }, 7, 2, b => BitConverter.ToUInt16(b.Reverse().ToArray(), 0));
			}
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public int Height {
			get {
				return this.GetValue(new byte[] { 0xFF, 0xC0 }, 5, 2, b => BitConverter.ToUInt16(b.Reverse().ToArray(), 0));
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream"></param>
		internal Jpeg(Stream stream) {
			this._stream = stream;
			this.LoadSegments();
		}

		/// <summary>
		/// セグメント情報読み込み
		/// </summary>
		/// <param name="stream"></param>
		private void LoadSegments() {
			// SOI分飛ばす
			var position = 2;
			var segments = new List<Segment>();

			var buff = new byte[4];
			while (true) {
				this._stream.Seek(position, SeekOrigin.Begin);
				if (this._stream.Read(buff, 0, buff.Length) != 4) {
					break;
				}
				var segment = new Segment();
				var span = buff.AsSpan();
				segment.Marker = span.Slice(0, 2).ToArray();
				segment.Position = position;
				segment.Size = BitConverter.ToUInt16(span.Slice(2, 2).ToArray().Reverse().ToArray(), 0);
				// サイズはマーカー部分を含まないため、マーカー分を加えてシーク位置を変更する
				position = segment.Position + segment.Size + 2;
				segments.Add(segment);
			}
			this._segments = segments;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="marker"></param>
		/// <param name="relationalPosition"></param>
		/// <param name="length"></param>
		/// <param name="selector"></param>
		/// <returns></returns>
		private T GetValue<T>(byte[] marker, int offset, int length, Func<byte[], T> selector) {
			var segment = this._segments
					.SingleOrDefault(
						x =>
							x.Marker
								.AsSpan()
								.SequenceEqual(marker)
					);
			if (segment == null) {
				return default;
			}
			var position = segment.Position + offset;
			var buff = new byte[length];
			this._stream.Seek(position, SeekOrigin.Begin);
			this._stream.Read(buff, 0, buff.Length);
			return selector(buff);
		}

		/// <summary>
		/// セグメント
		/// </summary>
		private class Segment {
			/// <summary>
			/// マーカー
			/// </summary>
			public byte[] Marker {
				get;
				set;
			}

			/// <summary>
			/// サイズ
			/// </summary>
			public int Size {
				get;
				set;
			}

			/// <summary>
			/// 開始位置
			/// </summary>
			public int Position {
				get;
				set;
			}

			public override string ToString() {
				return $"<[{base.ToString()}] {string.Join("", this.Marker.Select(x => $"{x:X2}"))}[{this.Position}]>";
			}
		}
	}
}
