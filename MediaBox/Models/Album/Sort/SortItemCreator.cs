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
			switch (this.SortItemKey) {
				case SortItemKeys.FileName:
					return new SortItem<string>(this.SortItemKey, x => x.FileName, this.Direction);
				case SortItemKeys.FilePath:
					return new SortItem<string>(this.SortItemKey, x => x.FilePath, this.Direction);
				case SortItemKeys.CreationTime:
					return new SortItem<DateTime>(this.SortItemKey, x => x.CreationTime, this.Direction);
				case SortItemKeys.ModifiedTime:
					return new SortItem<DateTime>(this.SortItemKey, x => x.ModifiedTime, this.Direction);
				case SortItemKeys.LastAccessTime:
					return new SortItem<DateTime>(this.SortItemKey, x => x.LastAccessTime, this.Direction);
				case SortItemKeys.FileSize:
					return new SortItem<long>(this.SortItemKey, x => x.FileSize, this.Direction);
				case SortItemKeys.Location:
					return new SortItem<GpsLocation>(this.SortItemKey, x => x.Location, this.Direction);
				case SortItemKeys.Rate:
					return new SortItem<int>(this.SortItemKey, x => x.Rate, this.Direction);
				case SortItemKeys.Resolution:
					return new SortItem<ComparableSize?>(this.SortItemKey, x => x.Resolution, this.Direction);
			}
			throw new ArgumentException();
		}

		public bool Equals(ISortItemCreator other) {
			if (ReferenceEquals(null, other)) {
				return false;
			}

			if (ReferenceEquals(this, other)) {
				return true;
			}

			return this.SortItemKey == other.SortItemKey && this.Direction == other.Direction;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
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
