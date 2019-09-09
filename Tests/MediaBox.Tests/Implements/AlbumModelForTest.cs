using System;
using System.Linq.Expressions;

using Livet;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album;

namespace SandBeige.MediaBox.Tests.Implements {

	internal class AlbumModelForTest : AlbumModel {
		public Expression<Func<MediaFile, bool>> Predicate {
			private get;
			set;
		} = _ => true;

		public AlbumModelForTest(ObservableSynchronizedCollection<IMediaFileModel> items, AlbumSelector selector) : base(items, selector) {

		}

		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			return this.Predicate;
		}
	}
}
