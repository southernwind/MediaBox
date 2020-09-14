using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Library.Video {

	/// <summary>
	/// 動画メタデータ
	/// </summary>
	public class Metadata {
		private IEnumerable<Attributes<string>>? _videoStreams;
		private IEnumerable<Attributes<string>> VideoStreams {
			get {
				return this._videoStreams ??= this.Streams
					.Where(x => x.Any(tv => tv.Title == "codec_type" && tv.Value == "video"))
					.ToArray();
			}
		}

		public Attributes<string> Formats {
			get;
		}

		public IEnumerable<Attributes<string>> Streams {
			get;
		}

		/// <summary>
		/// 動画の長さ
		/// </summary>
		public double? Duration {
			get {
				return
					this.Formats
						.GetOrDefault(
							"duration",
							(double?)null);
			}
		}

		/// <summary>
		/// 回転
		/// </summary>
		public int? Rotation {
			get {
				return this.VideoStreams.Select(x => x.GetOrDefault("rotation", null)).Max() ??
					   this.VideoStreams.Select(x => x.GetOrDefault("TAG:rotate", null)).Max();
			}
		}

		/// <summary>
		/// GPS座標
		/// </summary>
		public GpsLocation? Location {
			get {
				var title = new[] {
					"TAG:location",
					"TAG:com.apple.quicktime.location.ISO6709"
				}.FirstOrDefault(x => this.Formats.Any(f => f.Title == x));
				if (title == null) {
					return null;
				}

				var num = @"[\+-]\d+\.\d+?";
				var regex = new Regex($"^(?<lat>{num})(?<lon>{num})(?<alt>{num})?/$");

				var match = regex.Match(this.Formats.Single(x => x.Title == title).Value);
				if (!match.Success) {
					return null;
				}
				var lat = match.Result("${lat}");
				var lon = match.Result("${lon}");
				var alt = match.Result("${alt}");
				if (alt != "") {
					return new GpsLocation(double.Parse(lat), double.Parse(lon), double.Parse(alt));
				} else {
					return new GpsLocation(double.Parse(lat), double.Parse(lon));

				}
			}
		}

		/// <summary>
		/// 幅
		/// </summary>
		public int? Width {
			get {
				return this.VideoStreams.Select(x => x.GetOrDefault("width", null)).Max();
			}
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public int? Height {
			get {
				return this.VideoStreams.Select(x => x.GetOrDefault("height", null)).Max();
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal Metadata(Attributes<string> formats, IEnumerable<Attributes<string>> streams) {
			this.Formats = formats;
			this.Streams = streams;
		}
	}
}
