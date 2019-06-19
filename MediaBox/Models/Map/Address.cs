using System;
using System.Collections.Generic;
using System.Linq;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Map {
	/// <summary>
	/// 住所
	/// </summary>
	internal class Address {
		/// <summary>
		/// 場所の種類
		/// </summary>
		public string Type {
			get;
		}

		/// <summary>
		/// 場所の名前
		/// </summary>
		public string Name {
			get;
		}

		/// <summary>
		/// 子要素
		/// </summary>
		public Address[] Children {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="positions">この場所に含まれるPositionテーブルのデータ</param>
		public Address(IEnumerable<Position> positions) : this(null, null, positions) {
		}

		/// <summary>
		/// コンストラクタ子要素作成用
		/// </summary>
		/// <param name="type">場所の種類</param>
		/// <param name="name">場所の名前</param>
		/// <param name="positions">この場所に含まれるPositionテーブルのデータ</param>
		private Address(string type, string name, IEnumerable<Position> positions) {
			this.Name = $"{name}[{positions.Count()}]";
			var children = positions
				.Where(x => x.Addresses != null)
				.GroupBy(x => {
					IEnumerable<PositionAddress> q = x.Addresses.OrderByDescending(x => x.SequenceNumber);
					// type指定がある場合はその次の場所からはじめる
					if (type != null) {
						q = q.SkipWhile(x => x.Type != type)
						.Skip(1);
					}
					var pos = q
						.Where(x => !new[] { "postcode", "country_code" }.Contains(x.Type))
						.FirstOrDefault();
					return (pos?.Type, pos?.Name);
				}).Where(x => x.Key.Type != null)
				.Select(x => new Address(x.Key.Type, x.Key.Name, x.ToArray()));

			if (positions.Any(x => x.Addresses == null)) {
				children = children.Union(new[] { new Address("未取得", positions.Where(x => x.Addresses == null).ToArray()) });
			}
			this.Children = children.ToArray();
		}

		/// <summary>
		/// コンストラクタ未取得用
		/// </summary>
		/// <param name="type">場所の種類</param>
		/// <param name="name">場所の名前</param>
		/// <param name="positions">この場所に含まれるPositionテーブルのデータ</param>
		private Address(string name, IEnumerable<Position> positions) {
			// 未取得の座標一覧を出力する。
			this.Name = $"{name}[{positions.Count()}]";
			if (name == "未取得") {
				this.Children =
					positions
						.GroupBy(x => (x.Latitude, x.Longitude))
						.Select(x => new Address($"{x.Key.Latitude} {x.Key.Longitude}", x.ToArray()))
						.ToArray();
			} else {
				this.Children = Array.Empty<Address>();
			}
		}
	}
}
