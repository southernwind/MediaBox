using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Map;

namespace SandBeige.MediaBox.Library.Tests.Map {
	[TestFixture]
	internal class GeoCodingTest {
		[Test]
		public async Task パターン1() {
			var geo = new GeoCoding();
			var p = await geo.Reverse(34.22795833, 131.303619444);
			p.PlaceId.Is(15287257);
			p.OsmType.Is("node");
			p.OsmId.Is(1350895116);
			p.DisplayName.Is("秋芳洞, 馬ころび, 美祢市, 山口県, 中国地方, 日本");
			p.Address.Is(
				new Dictionary<string, string>() {
					{ "cave_entrance", "秋芳洞" },
					{ "footway", "秋芳洞" },
					{ "hamlet", "馬ころび" },
					{ "city", "美祢市" },
					{ "state", "中国地方" },
					{ "country", "日本" },
					{ "country_code", "jp" }
				}
			);
			p.NameDetails.Is(
				new Dictionary<string, string>() {
					{ "name", "秋芳洞" },
					{ "name:en", "Akiyoshido" },
					{ "name:ja", "秋芳洞" }
				}
			);
			p.BoundingBox.Is(
				34.2279614,
				34.2281614,
				131.3033105,
				131.3035105
			);
		}


		[Test]
		public async Task パターン2() {
			var geo = new GeoCoding();
			var p = await geo.Reverse(35.628852, 139.882162);
			p.PlaceId.Is(97100510);
			p.OsmType.Is("way");
			p.OsmId.Is(97767433);
			p.DisplayName.Is("シンドバッド・ストーリーブック・ヴォヤッジ, Kingdoms Bridge, 浦安市, 千葉県, 関東地方, 279-0031, 日本");
			p.Address.Is(
				new Dictionary<string, string>() {
					 {"attraction", "シンドバッド・ストーリーブック・ヴォヤッジ" },
					 {"pedestrian", "Kingdoms Bridge"},
					 {"city", "浦安市"},
					 {"state", "千葉県"},
					 {"region", "関東地方"},
					 {"postcode", "279-0031"},
					 {"country", "日本"},
					 {"country_code", "jp"}
				}
			);
			p.NameDetails.Is(
				new Dictionary<string, string>() {
					{ "name", "シンドバッド・ストーリーブック・ヴォヤッジ"},
					{"name:en", "Sindbad's Storybook Voyage" },
					{ "name:es", "Sindbad's Storybook Voyage" },
					{ "name:ja", "シンドバッド・ストーリーブック・ヴォヤッジ" }
				}
			);
			p.BoundingBox.Is(
				35.6284351,
				35.6290724,
				139.880952,
				139.8822882
			);
		}
	}
}
