using System.Collections.Generic;
using System.Linq;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Library.Video {

	/// <summary>
	/// 動画メタデータ
	/// </summary>
	public class Metadata {
		internal Dictionary<string, string> Formats {
			get;
			set;
		}

		internal IEnumerable<Dictionary<string, string>> Streams {
			get;
			set;
		}

		/// <summary>
		/// 動画の長さ
		/// </summary>
		public double? Duration {
			get {
				return
					this.Formats?
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
				return
					this.Streams
						.SingleOrDefault(x => x.Any(kv => kv.Key == "codec_type" && kv.Value == "video"))?
						.GetOrDefault("rotation", null);
			}
		}

		public GpsLocation Location {
			get {
				return new GpsLocation(0, 0, 0);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal Metadata() {
		}
	}
}
