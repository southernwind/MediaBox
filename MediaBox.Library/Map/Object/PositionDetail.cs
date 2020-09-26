using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace SandBeige.MediaBox.Library.Map.Object {
	// TODO: nullable精査
	public class PositionDetail {
		private Dictionary<string, string>? _nameDetails;
		private Dictionary<string, string>? _address;

		[JsonProperty("place_id")]
		public long PlaceId {
			get;
			set;
		}

		[JsonProperty("licence")]
		public string? License {
			get;
			set;
		}

		[JsonProperty("osm_type")]
		public string? OsmType {
			get;
			set;
		}

		[JsonProperty("osm_id")]
		public long OsmId {
			get;
			set;
		}

		[JsonProperty("display_name")]
		public string? DisplayName {
			get;
			set;
		}

		public Dictionary<string, string> Address {
			get {
				return this._address ?? throw new InvalidOperationException();
			}
			set {
				this._address = value;
			}
		}

		public Dictionary<string, string> NameDetails {
			get {
				return this._nameDetails ?? throw new InvalidOperationException();
			}
			set {
				this._nameDetails = value;
			}
		}

		public double[]? BoundingBox {
			get;
			set;
		}
	}
}
