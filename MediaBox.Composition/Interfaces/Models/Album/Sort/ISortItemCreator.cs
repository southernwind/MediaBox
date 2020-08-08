using System;
using System.ComponentModel;

using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort {
	public interface ISortItemCreator : IEquatable<ISortItemCreator> {
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

		public ISortItem Create();
	}
}
