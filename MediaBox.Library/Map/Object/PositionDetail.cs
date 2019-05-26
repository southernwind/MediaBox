using System.Collections.Generic;

using Newtonsoft.Json;

namespace SandBeige.MediaBox.Library.Map.Object {
	public class PositionDetail {
		[JsonProperty("place_id")]
		public long PlaceId {
			get;
			set;
		}

		[JsonProperty("licence")]
		public string License {
			get;
			set;
		}

		[JsonProperty("osm_type")]
		public string OsmType {
			get;
			set;
		}

		[JsonProperty("osm_id")]
		public long OsmId {
			get;
			set;
		}

		[JsonProperty("display_name")]
		public string DisplayName {
			get;
			set;
		}

		public Dictionary<string, string> Address {
			get;
			set;
		}

		public Dictionary<string, string> NameDetails {
			get;
			set;
		}

		public double[] BoundingBox {
			get;
			set;
		}
	}
}
