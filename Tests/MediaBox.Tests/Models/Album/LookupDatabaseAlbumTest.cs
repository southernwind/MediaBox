
using System;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class LookupDatabaseAlbumTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();

			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, tags: new[] { "aaa", "bbb" }, subTable: SubTable.Image, latitude: 35.65846388888889, longitude: 139.74511666666666, position: new Position {
				Latitude = 35.65846388888889,
				Longitude = 139.74511666666666,
				DisplayName = "東京タワー, 東京タワー通り, 六丁目, 港区, 東京都, 関東地方, 105-0011, 日本",
				IsAcquired = true,
				Addresses = new[] {
					new PositionAddress { Name="東京タワー", SequenceNumber=0, Type="address29" },
					new PositionAddress { Name="東京タワー通り", SequenceNumber=1, Type="road" },
					new PositionAddress { Name="六丁目", SequenceNumber=2, Type="hamlet" },
					new PositionAddress { Name="港区", SequenceNumber=3, Type="city" },
					new PositionAddress { Name="東京都", SequenceNumber=4, Type="state" },
					new PositionAddress { Name="関東地方", SequenceNumber=5, Type="region" },
					new PositionAddress { Name="105-0011", SequenceNumber=6, Type="postcode" },
					new PositionAddress { Name="日本", SequenceNumber=7, Type="country" },
					new PositionAddress { Name="jp", SequenceNumber=8, Type="country_code" }
				}
			});
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, tags: new[] { "aaa", "bbb", "ccc" }, subTable: SubTable.Image, latitude: 35.7100527777777, longitude: 139.809525, position: new Position {
				Latitude = 35.7100527777777,
				Longitude = 139.809525,
				DisplayName = "東京ソラマチ, 墨田区画街路第5号線, 墨田区, 東京都, 関東地方, 131-0045, 日本",
				IsAcquired = true,
				Addresses = new[] {
					new PositionAddress { Name="東京ソラマチ", SequenceNumber=0, Type="mall" },
					new PositionAddress { Name="墨田区画街路第5号線", SequenceNumber=1, Type="road" },
					new PositionAddress { Name="墨田区", SequenceNumber=2, Type="city" },
					new PositionAddress { Name="東京都", SequenceNumber=3, Type="state" },
					new PositionAddress { Name="関東地方", SequenceNumber=4, Type="region" },
					new PositionAddress { Name="131-0045", SequenceNumber=5, Type="postcode" },
					new PositionAddress { Name="日本", SequenceNumber=6, Type="country" },
					new PositionAddress { Name="jp", SequenceNumber=7, Type="country_code" }
				}
			});
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, tags: new[] { "aaa", "ddd" }, subTable: SubTable.Image, latitude: null, longitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, tags: Array.Empty<string>(), subTable: SubTable.Image, latitude: 35.5740694444444, longitude: 139.659836111111, position: new Position {
				Latitude = 35.5740694444444,
				Longitude = 139.659836111111,
				DisplayName = "グランツリー武蔵小杉, 綱島街道, 中原区, 川崎市, 神奈川県, 関東地方, 211-0004, 日本",
				IsAcquired = true,
				Addresses = new[] {
					new PositionAddress { Name="グランツリー武蔵小杉", SequenceNumber=0, Type="address29" },
					new PositionAddress { Name="綱島街道", SequenceNumber=1, Type="road" },
					new PositionAddress { Name="中原区", SequenceNumber=2, Type="suburb" },
					new PositionAddress { Name="川崎市", SequenceNumber=3, Type="city" },
					new PositionAddress { Name="神奈川県", SequenceNumber=4, Type="state" },
					new PositionAddress { Name="関東地方", SequenceNumber=5, Type="region" },
					new PositionAddress { Name="211-0004", SequenceNumber=6, Type="postcode" },
					new PositionAddress { Name="日本", SequenceNumber=7, Type="country" },
					new PositionAddress { Name="jp", SequenceNumber=8, Type="country_code" }
				}
			});
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, tags: new[] { "aaa", "eee" }, subTable: SubTable.Image, latitude: 34.69763611111111, longitude: 135.5018, position: new Position {
				Latitude = 34.69763611111111,
				Longitude = 135.5018,
				DisplayName = null,
				IsAcquired = false
			});
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, tags: new[] { "aaa", "ccc", "ddd" }, subTable: SubTable.Video, latitude: null, longitude: null);
		}

		[TestCase("aaa", 1, 2, 3, 5, 6)]
		[TestCase("bbb", 1, 2)]
		[TestCase("ccc", 2, 6)]
		[TestCase("ddd", 3, 6)]
		[TestCase("eee", 5)]
		[TestCase("fff")]
		[TestCase("aa")]
		[TestCase("aaaa")]
		public async Task ロードパターンタグ(string tag, params long[] idList) {
			using var selector = new AlbumSelector("main");
			using var la = new LookupDatabaseAlbum(selector);
			la.TagName = tag;
			la.Items.Count.Is(0);
			la.LoadMediaFiles();
			await this.WaitTaskCompleted(3000);
			la.Items.Select(x => x.MediaFileId.Value).OrderBy(x => x).Is(idList);
		}

		[TestCase("aa", 1, 2, 3, 5, 6)]
		[TestCase("aaa", 1, 2, 3, 5, 6)]
		[TestCase("bb", 1, 2)]
		[TestCase("関東", 1, 2, 4)]
		[TestCase("神奈川", 4)]
		[TestCase("image", 1, 2, 3, 4)]
		[TestCase("jpg", 1, 2, 3, 5)]
		[TestCase("タワー", 1)]
		public async Task ロードパターンワード(string word, params long[] idList) {
			using var selector = new AlbumSelector("main");
			using var la = new LookupDatabaseAlbum(selector);
			la.Word = word;
			la.Items.Count.Is(0);
			la.LoadMediaFiles();
			await this.WaitTaskCompleted(3000);
			la.Items.Select(x => x.MediaFileId.Value).OrderBy(x => x).Is(idList);
		}

		[Test]
		public async Task ロードパターン場所() {
			using var selector = new AlbumSelector("main");
			using var la = new LookupDatabaseAlbum(selector);

			var address = new Address(new[] {
				new Position { Addresses = new[] {
					new PositionAddress { Name="東京ソラマチ", SequenceNumber=0, Type="mall" },
					new PositionAddress { Name="墨田区画街路第5号線", SequenceNumber=1, Type="road" },
					new PositionAddress { Name="墨田区", SequenceNumber=2, Type="city" },
					new PositionAddress { Name="東京都", SequenceNumber=3, Type="state" },
					new PositionAddress { Name="関東地方", SequenceNumber=4, Type="region" },
					new PositionAddress { Name="131-0045", SequenceNumber=5, Type="postcode" },
					new PositionAddress { Name="日本", SequenceNumber=6, Type="country" },
					new PositionAddress { Name="jp", SequenceNumber=7, Type="country_code" }
				}},
				new Position { Addresses = new[] {
					new PositionAddress { Name="グランツリー武蔵小杉", SequenceNumber=0, Type="address29" },
					new PositionAddress { Name="綱島街道", SequenceNumber=1, Type="road" },
					new PositionAddress { Name="中原区", SequenceNumber=2, Type="suburb" },
					new PositionAddress { Name="川崎市", SequenceNumber=3, Type="city" },
					new PositionAddress { Name="神奈川県", SequenceNumber=4, Type="state" },
					new PositionAddress { Name="関東地方", SequenceNumber=5, Type="region" },
					new PositionAddress { Name="211-0004", SequenceNumber=6, Type="postcode" },
					new PositionAddress { Name="日本", SequenceNumber=7, Type="country" },
					new PositionAddress { Name="jp", SequenceNumber=8, Type="country_code" }
				}},
				new Position{Addresses = new[] {
					new PositionAddress { Name="東京タワー", SequenceNumber=0, Type="address29" },
					new PositionAddress { Name="東京タワー通り", SequenceNumber=1, Type="road" },
					new PositionAddress { Name="六丁目", SequenceNumber=2, Type="hamlet" },
					new PositionAddress { Name="港区", SequenceNumber=3, Type="city" },
					new PositionAddress { Name="東京都", SequenceNumber=4, Type="state" },
					new PositionAddress { Name="関東地方", SequenceNumber=5, Type="region" },
					new PositionAddress { Name="105-0011", SequenceNumber=6, Type="postcode" },
					new PositionAddress { Name="日本", SequenceNumber=7, Type="country" },
					new PositionAddress { Name="jp", SequenceNumber=8, Type="country_code" }
				}},
				new Position {
					Latitude = 34.69763611111111,
					Longitude = 135.5018,
					IsAcquired = false
				}
			});

			var country = address.Children.First(x => x.Name == "日本");
			var non = address.Children.First(x => x.IsYet);
			var region = country.Children[0];
			var tokyo = region.Children.First(x => x.Name == "東京都");
			var kanagawa = region.Children.First(x => x.Name == "神奈川県");
			var minato = tokyo.Children.First(x => x.Name == "港区");

			la.Address = kanagawa;
			la.Items.Count.Is(0);
			la.LoadFromDataBase();
			await this.WaitTaskCompleted(3000);
			la.Items.Select(x => x.MediaFileId).OrderBy(x => x).Is(4);

			la.Address = tokyo;
			la.LoadFromDataBase();
			await this.WaitTaskCompleted(3000);
			la.Items.Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2);

			la.Address = minato;
			la.LoadFromDataBase();
			await this.WaitTaskCompleted(3000);
			la.Items.Select(x => x.MediaFileId).OrderBy(x => x).Is(1);

			la.Address = non;
			la.LoadFromDataBase();
			await this.WaitTaskCompleted(3000);
			la.Items.Select(x => x.MediaFileId).OrderBy(x => x).Is(5);
		}
	}
}
