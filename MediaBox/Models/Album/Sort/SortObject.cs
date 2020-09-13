using System;
using System.Collections.Generic;
using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// フィルター設定復元用オブジェクト
	/// </summary>
	public class SortObject : ISortObject {
		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// フィルター条件クリエイター
		/// </summary>
		public ReactiveCollection<ISortItemCreator> SortItemCreators {
			get;
			set;
		} = new ReactiveCollection<ISortItemCreator>();

		public SortObject() {
		}

		public SortObject(string displayName, IEnumerable<ISortItemCreator> sortItemCreators) {
			this.DisplayName.Value = displayName;
			this.SortItemCreators.AddRange(sortItemCreators);
		}

		public bool Equals(ISortObject? other) {
			if (other is null) {
				return false;
			}

			if (ReferenceEquals(this, other)) {
				return true;
			}

			return Equals(this.DisplayName.Value, other.DisplayName.Value) && this.SortItemCreators.SequenceEqual(other.SortItemCreators);
		}

		public override bool Equals(object? obj) {
			return obj is ISortObject rso && this.Equals(rso);
		}

		public override int GetHashCode() {
			unchecked {
				return ((this.DisplayName.Value?.GetHashCode() ?? 0) * 397) ^ this.SortItemCreators.Select(x => x.GetHashCode()).Aggregate((x, y) => x ^ y);
			}
		}
	}
}