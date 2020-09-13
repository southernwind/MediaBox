using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Map.Object;

namespace SandBeige.MediaBox.Library.Map {
	public class GeoCoding : IDisposable {
		private const string _apiUrl = "https://nominatim.openstreetmap.org/reverse";
		private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
		private readonly HttpClient _httpClient;
		public GeoCoding() {
			var handler = new HttpClientHandler();
			this._httpClient = new HttpClient(handler).AddTo(this._compositeDisposable);
			this._httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (MediaBox,xmi1996@gmail.com)");
			this._httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
			this._httpClient.DefaultRequestHeaders.Add("Accept-Language", "ja,en-us;q=0.7,en;q=0.3");
			this._httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
		}

		public async Task<PositionDetail> Reverse(GpsLocation location) {
			return await this.Reverse(location.Latitude, location.Longitude);
		}

		public async Task<PositionDetail> Reverse(double latitude, double longitude) {
			var url = _apiUrl;
			url += "?";
			url += "format=json&";
			url += $"lat={latitude}&";
			url += $"lon={longitude}&";
			url += "zoom=18&";
			url += "addressdetails=1&";
			url += "namedetails=1";
			var hrm = await this._httpClient.GetAsync(url);
			var json = await HrmToString(hrm);
			// jsonじゃなかったら？
			return JsonConvert.DeserializeObject<PositionDetail>(json);
		}

		private static async Task<string> HrmToString(HttpResponseMessage hrm) {
			try {
				if (hrm.Content.Headers.ContentEncoding.ToString() == "gzip") {
					var st = await hrm.Content.ReadAsStreamAsync();
					var gzip = new GZipStream(st, CompressionMode.Decompress);
					var sr = new StreamReader(gzip);
					return await sr.ReadToEndAsync();
				} else {
					return await hrm.Content.ReadAsStringAsync();
				}
			} catch {
				// TODO: 例外が来たらどうする？
				return "";
			}
		}

		public void Dispose() {
			this._compositeDisposable.Dispose();
		}
	}
}
