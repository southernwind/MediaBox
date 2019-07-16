using System;
using System.Collections.Generic;
using System.Linq;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Map {
	/// <summary>
	/// 住所
	/// </summary>
	public class Address {
		/// <summary>
		/// 親要素
		/// </summary>
		public Address Parent {
			get;
			set;
		}

		/// <summary>
		/// 場所の種類
		/// </summary>
		public string Type {
			get;
			set;
		}

		/// <summary>
		/// 場所の名前
		/// </summary>
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// 未取得の座標か否か
		/// </summary>
		public bool IsYet {
			get;
			set;
		}

		/// <summary>
		/// 取得に失敗した座標か否か
		/// </summary>
		public bool IsFailure {
			get;
			set;
		}

		/// <summary>
		/// 件数
		/// </summary>
		public int Count {
			get;
		}

		/// <summary>
		/// 子要素
		/// </summary>
		public Address[] Children {
			get;
		}

		[Obsolete("for serialize")]
		public Address() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="positions">この場所に含まれるPositionテーブルのデータ</param>
		public Address(IEnumerable<Position> positions) : this(null, null, null, positions) {
		}

		/// <summary>
		/// コンストラクタ子要素作成用
		/// </summary>
		/// <param name="parent">親要素</param>
		/// <param name="type">場所の種類</param>
		/// <param name="name">場所の名前</param>
		/// <param name="positions">この場所に含まれるPositionテーブルのデータ</param>
		private Address(Address parent, string type, string name, IEnumerable<Position> positions) {
			this.Parent = parent;
			this.Name = name;
			this.Count = positions.Count();
			this.Type = type;
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
				.Select(x => new Address(this, x.Key.Type, x.Key.Name, x.ToArray()));

			if (positions.Any(x => x.Addresses == null && !x.IsAcquired)) {
				children = children.Union(new[] { new Address(this, true, false, "未取得", positions.Where(x => x.Addresses == null && !x.IsAcquired).ToArray()) });
			}
			if (positions.Any(x => x.Addresses == null && x.IsAcquired)) {
				children = children.Union(new[] { new Address(this, false, true, "取得不可", positions.Where(x => x.Addresses == null && x.IsAcquired).ToArray()) });
			}
			this.Children = children.ToArray();
		}

		/// <summary>
		/// コンストラクタ未取得、取得不可用
		/// </summary>
		/// <param name="parent">親要素</param>
		/// <param name="isFailure">未取得レコードか否か</param>
		/// <param name="isYet">取得失敗レコードか否か</param>
		/// <param name="name">場所の名前</param>
		/// <param name="positions">この場所に含まれるPositionテーブルのデータ</param>
		private Address(Address parent, bool isYet, bool isFailure, string name, IEnumerable<Position> positions) {
			this.Parent = parent;
			this.Name = name;
			this.Count = positions.Count();
			this.IsYet = isYet;
			this.IsFailure = isFailure;
			// 未取得、取得不可の座標一覧を出力する。
			if (name == "未取得" || name == "取得不可") {
				this.Children =
					positions
						.GroupBy(x => (x.Latitude, x.Longitude))
						.Select(x => new Address(this, isYet, isFailure, $"{x.Key.Latitude} {x.Key.Longitude}", x.ToArray()))
						.ToArray();
			} else {
				this.Children = Array.Empty<Address>();
			}
		}
	}
}
