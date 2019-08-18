using System;
using System.Collections.Generic;
using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// フィルター設定復元用オブジェクト
	/// </summary>
	public class RestorableSortObject : IEquatable<RestorableSortObject> {
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
		public ReactiveCollection<SortItemCreator> SortItemCreators {
			get;
			set;
		} = new ReactiveCollection<SortItemCreator>();

		public RestorableSortObject() {
		}

		public RestorableSortObject(string displayName, IEnumerable<SortItemCreator> sortItemCreators) {
			this.DisplayName.Value = displayName;
			this.SortItemCreators.AddRange(sortItemCreators);
		}

		public bool Equals(RestorableSortObject other) {
			if (other is null) {
				return false;
			}

			if (ReferenceEquals(this, other)) {
				return true;
			}

			return Equals(this.DisplayName.Value, other.DisplayName.Value) && this.SortItemCreators.SequenceEqual(other.SortItemCreators);
		}

		public override bool Equals(object obj) {
			return obj is RestorableSortObject rso && this.Equals(rso);
		}

		public override int GetHashCode() {
			unchecked {
				return ((this.DisplayName?.Value != null ? this.DisplayName.Value.GetHashCode() : 0) * 397) ^ this.SortItemCreators.Select(x => x.GetHashCode()).Aggregate((x, y) => x ^ y);
			}
		}
	}
}