using System;
using System.ComponentModel;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Models.Album.Sort {
	public class SortItemCreator : ISortItemCreator {

		/// <summary>
		/// ソートキー
		/// </summary>
		public SortItemKeys SortItemKey {
			get;
			set;
		}

		/// <summary>
		/// ソート方向
		/// </summary>
		public ListSortDirection Direction {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public SortItemCreator() {

		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sortItemKey">ソートキー</param>
		/// <param name="direction">ソート方向</param>
		public SortItemCreator(SortItemKeys sortItemKey, ListSortDirection direction = ListSortDirection.Ascending) {
			this.SortItemKey = sortItemKey;
			this.Direction = direction;
		}

		public ISortItem Create() {
			return this.SortItemKey switch
			{
				SortItemKeys.FileName => new SortItem<string>(this.SortItemKey, x => x.FileName, this.Direction),
				SortItemKeys.FilePath => new SortItem<string>(this.SortItemKey, x => x.FilePath, this.Direction),
				SortItemKeys.CreationTime => new SortItem<DateTime>(this.SortItemKey, x => x.CreationTime, this.Direction),
				SortItemKeys.ModifiedTime => new SortItem<DateTime>(this.SortItemKey, x => x.ModifiedTime, this.Direction),
				SortItemKeys.LastAccessTime => new SortItem<DateTime>(this.SortItemKey, x => x.LastAccessTime, this.Direction),
				SortItemKeys.FileSize => new SortItem<long>(this.SortItemKey, x => x.FileSize, this.Direction),
				SortItemKeys.Location => new SortItem<GpsLocation?>(this.SortItemKey, x => x.Location, this.Direction),
				SortItemKeys.Rate => new SortItem<int>(this.SortItemKey, x => x.Rate, this.Direction),
				SortItemKeys.Resolution => new SortItem<ComparableSize?>(this.SortItemKey, x => x.Resolution, this.Direction),
				_ => throw new ArgumentException(),
			};
		}

		public bool Equals(ISortItemCreator? other) {
			if (other is null) {
				return false;
			}

			if (ReferenceEquals(this, other)) {
				return true;
			}

			return this.SortItemKey == other.SortItemKey && this.Direction == other.Direction;
		}

		public override bool Equals(object? obj) {
			if (obj is null) {
				return false;
			}

			if (ReferenceEquals(this, obj)) {
				return true;
			}

			return obj is ISortItemCreator sic && this.Equals(sic);
		}

		public override int GetHashCode() {
			unchecked {
				return ((int)this.SortItemKey * 397) ^ (int)this.Direction;
			}
		}
	}
}
