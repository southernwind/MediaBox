using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Tests.Models.Map {
	internal class AddressTest : ModelTestClassBase {
		[Test]
		public void インスタンス生成() {
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
					Latitude = 22.5589007,
					Longitude = 114.1319156,
					IsAcquired = false,
					Addresses =  null
				},
				new Position {
					Latitude = 34.6976361,
					Longitude = 135.5018,
					IsAcquired = false,
					Addresses =  null
				},
				new Position {
					Latitude = 34.6976361,
					Longitude = 135.5018,
					IsAcquired = false,
					Addresses =  null
				},
				new Position {
					Latitude = 33.592028,
					Longitude = 132.2921533,
					IsAcquired = false,
					Addresses =  null
				},
				new Position {
					Latitude = -2.8543694,
					Longitude = 164.7743447,
					IsAcquired = true,
					Addresses =  null
				},
				new Position {
					Latitude = 28.2501111,
					Longitude = 155.046522,
					IsAcquired = true,
					Addresses =  null
				}
			});

			address.Children.Select(x => x.Name).Is("日本", "未取得", "取得不可");
			var c1 = address.Children.Single(x => x.Name == "日本");
			c1.Name.Is("日本");
			c1.Type.Is("country");
			c1.IsYet.IsFalse();
			c1.Count.Is(3);
			c1.IsFailure.IsFalse();
			c1.Parent.Is(address);

			var c1C1 = c1.Children.Single(x => x.Name == "関東地方");
			c1C1.Name.Is("関東地方");
			c1C1.Type.Is("region");
			c1C1.IsYet.IsFalse();
			c1C1.Count.Is(3);
			c1C1.IsFailure.IsFalse();
			c1C1.Parent.Is(c1);

			var c1C1C1 = c1C1.Children.Single(x => x.Name == "東京都");
			c1C1C1.Name.Is("東京都");
			c1C1C1.Type.Is("state");
			c1C1C1.IsYet.IsFalse();
			c1C1C1.Count.Is(2);
			c1C1C1.IsFailure.IsFalse();
			c1C1C1.Parent.Is(c1C1);

			var c1C1C1C1 = c1C1C1.Children.Single(x => x.Name == "墨田区");
			c1C1C1C1.Name.Is("墨田区");
			c1C1C1C1.Type.Is("city");
			c1C1C1C1.IsYet.IsFalse();
			c1C1C1C1.Count.Is(1);
			c1C1C1C1.IsFailure.IsFalse();
			c1C1C1C1.Parent.Is(c1C1C1);

			var c1C1C1C1C1 = c1C1C1C1.Children.Single(x => x.Name == "墨田区画街路第5号線");
			c1C1C1C1C1.Name.Is("墨田区画街路第5号線");
			c1C1C1C1C1.Type.Is("road");
			c1C1C1C1C1.IsYet.IsFalse();
			c1C1C1C1C1.Count.Is(1);
			c1C1C1C1C1.IsFailure.IsFalse();
			c1C1C1C1C1.Parent.Is(c1C1C1C1);

			var c1C1C1C1C1C1 = c1C1C1C1C1.Children.Single(x => x.Name == "東京ソラマチ");
			c1C1C1C1C1C1.Name.Is("東京ソラマチ");
			c1C1C1C1C1C1.Type.Is("mall");
			c1C1C1C1C1C1.IsYet.IsFalse();
			c1C1C1C1C1C1.Count.Is(1);
			c1C1C1C1C1C1.IsFailure.IsFalse();
			c1C1C1C1C1C1.Parent.Is(c1C1C1C1C1);

			var c1C1C1C2 = c1C1C1.Children.Single(x => x.Name == "港区");
			c1C1C1C2.Name.Is("港区");
			c1C1C1C2.Type.Is("city");
			c1C1C1C2.IsYet.IsFalse();
			c1C1C1C2.Count.Is(1);
			c1C1C1C2.IsFailure.IsFalse();
			c1C1C1C2.Parent.Is(c1C1C1);

			var c1C1C1C2C1 = c1C1C1C2.Children.Single(x => x.Name == "六丁目");
			c1C1C1C2C1.Name.Is("六丁目");
			c1C1C1C2C1.Type.Is("hamlet");
			c1C1C1C2C1.IsYet.IsFalse();
			c1C1C1C2C1.Count.Is(1);
			c1C1C1C2C1.IsFailure.IsFalse();
			c1C1C1C2C1.Parent.Is(c1C1C1C2);

			var c1C1C1C2C1C1 = c1C1C1C2C1.Children.Single(x => x.Name == "東京タワー通り");
			c1C1C1C2C1C1.Name.Is("東京タワー通り");
			c1C1C1C2C1C1.Type.Is("road");
			c1C1C1C2C1C1.IsYet.IsFalse();
			c1C1C1C2C1C1.Count.Is(1);
			c1C1C1C2C1C1.IsFailure.IsFalse();
			c1C1C1C2C1C1.Parent.Is(c1C1C1C2C1);

			var c1C1C1C2C1C1C1 = c1C1C1C2C1C1.Children.Single(x => x.Name == "東京タワー");
			c1C1C1C2C1C1C1.Name.Is("東京タワー");
			c1C1C1C2C1C1C1.Type.Is("address29");
			c1C1C1C2C1C1C1.IsYet.IsFalse();
			c1C1C1C2C1C1C1.Count.Is(1);
			c1C1C1C2C1C1C1.IsFailure.IsFalse();
			c1C1C1C2C1C1C1.Parent.Is(c1C1C1C2C1C1);

			var c1C1C2 = c1C1.Children.Single(x => x.Name == "神奈川県");
			c1C1C2.Name.Is("神奈川県");
			c1C1C2.Type.Is("state");
			c1C1C2.IsYet.IsFalse();
			c1C1C2.Count.Is(1);
			c1C1C2.IsFailure.IsFalse();
			c1C1C2.Parent.Is(c1C1);

			var c1C1C2C1 = c1C1C2.Children.Single(x => x.Name == "川崎市");
			c1C1C2C1.Name.Is("川崎市");
			c1C1C2C1.Type.Is("city");
			c1C1C2C1.IsYet.IsFalse();
			c1C1C2C1.Count.Is(1);
			c1C1C2C1.IsFailure.IsFalse();
			c1C1C2C1.Parent.Is(c1C1C2);

			var c1C1C2C1C1 = c1C1C2C1.Children.Single(x => x.Name == "中原区");
			c1C1C2C1C1.Name.Is("中原区");
			c1C1C2C1C1.Type.Is("suburb");
			c1C1C2C1C1.IsYet.IsFalse();
			c1C1C2C1C1.Count.Is(1);
			c1C1C2C1C1.IsFailure.IsFalse();
			c1C1C2C1C1.Parent.Is(c1C1C2C1);

			var c1C1C2C1C1C1 = c1C1C2C1C1.Children.Single(x => x.Name == "綱島街道");
			c1C1C2C1C1C1.Name.Is("綱島街道");
			c1C1C2C1C1C1.Type.Is("road");
			c1C1C2C1C1C1.IsYet.IsFalse();
			c1C1C2C1C1C1.Count.Is(1);
			c1C1C2C1C1C1.IsFailure.IsFalse();
			c1C1C2C1C1C1.Parent.Is(c1C1C2C1C1);

			var c1C1C2C1C1C1C1 = c1C1C2C1C1C1.Children.Single(x => x.Name == "グランツリー武蔵小杉");
			c1C1C2C1C1C1C1.Name.Is("グランツリー武蔵小杉");
			c1C1C2C1C1C1C1.Type.Is("address29");
			c1C1C2C1C1C1C1.IsYet.IsFalse();
			c1C1C2C1C1C1C1.Count.Is(1);
			c1C1C2C1C1C1C1.IsFailure.IsFalse();
			c1C1C2C1C1C1C1.Parent.Is(c1C1C2C1C1C1);

			var c2 = address.Children.Single(x => x.Name == "未取得");
			c2.Name.Is("未取得");
			c2.Type.IsNull();
			c2.IsYet.IsTrue();
			c2.Count.Is(4);
			c2.IsFailure.IsFalse();
			c2.Parent.Is(address);

			var c2C1 = c2.Children.Single(x => x.Name == "22.5589007 114.1319156");
			c2C1.Name.Is("22.5589007 114.1319156");
			c2C1.Type.IsNull();
			c2C1.IsYet.IsTrue();
			c2C1.Count.Is(1);
			c2C1.IsFailure.IsFalse();
			c2C1.Parent.Is(c2);
			c2C1.Children.Is();

			var c2C2 = c2.Children.Single(x => x.Name == "34.6976361 135.5018");
			c2C2.Name.Is("34.6976361 135.5018");
			c2C2.Type.IsNull();
			c2C2.IsYet.IsTrue();
			c2C2.Count.Is(2);
			c2C2.IsFailure.IsFalse();
			c2C2.Parent.Is(c2);
			c2C2.Children.Is();

			var c2C3 = c2.Children.Single(x => x.Name == "33.592028 132.2921533");
			c2C3.Name.Is("33.592028 132.2921533");
			c2C3.Type.IsNull();
			c2C3.IsYet.IsTrue();
			c2C3.Count.Is(1);
			c2C3.IsFailure.IsFalse();
			c2C3.Parent.Is(c2);
			c2C3.Children.Is();

			var c3 = address.Children.Single(x => x.Name == "取得不可");
			c3.Name.Is("取得不可");
			c3.Type.IsNull();
			c3.IsYet.IsFalse();
			c3.Count.Is(2);
			c3.IsFailure.IsTrue();
			c3.Parent.Is(address);

			var c3C1 = c3.Children.Single(x => x.Name == "-2.8543694 164.7743447");
			c3C1.Name.Is("-2.8543694 164.7743447");
			c3C1.Type.IsNull();
			c3C1.IsYet.IsFalse();
			c3C1.Count.Is(1);
			c3C1.IsFailure.IsTrue();
			c3C1.Parent.Is(c3);
			c3C1.Children.Is();

			var c3C2 = c3.Children.Single(x => x.Name == "28.2501111 155.046522");
			c3C2.Name.Is("28.2501111 155.046522");
			c3C2.Type.IsNull();
			c3C2.IsYet.IsFalse();
			c3C2.Count.Is(1);
			c3C2.IsFailure.IsTrue();
			c3C2.Parent.Is(c3);
			c3C2.Children.Is();
		}

		[Test]
		public void シリアライズ用インスタンス生成() {
#pragma warning disable 618
			var address = new Address();
#pragma warning restore 618
		}
	}
}
