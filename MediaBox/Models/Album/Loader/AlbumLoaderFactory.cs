
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

using LiteDB;

using Prism.Ioc;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.Loader {
	public class AlbumLoaderFactory : IAlbumLoaderFactory {
		private readonly IContainerProvider _containerProvider;
		public AlbumLoaderFactory(IContainerProvider containerProvider) {
			this._containerProvider = containerProvider;
		}

		public IAlbumLoader Create(IAlbumObject albumObject, IFilterDescriptionManager filterSetter, ISortDescriptionManager sortSetter) {
			IAlbumLoader result = albumObject switch
			{
				FolderAlbumObject => this._containerProvider.Resolve<FolderAlbumLoader>(),
				RegisteredAlbumObject => this._containerProvider.Resolve<RegisteredAlbumLoader>(),
				LookupDatabaseAlbumObject => this._containerProvider.Resolve<LookupDatabaseAlbumLoader>(),
				_ => throw new ArgumentException()
			};
			result.SetAlbumObject(albumObject);
			result.SetFilterAndSort(filterSetter, sortSetter);

			return result;
		}
		public IAlbumLoader CreateWithoutSortAndFilter(IAlbumObject albumObject) {
			return this.Create(albumObject, new DummyFilter(), new DummySort());
		}

		private class DummyFilter : ModelBase, IFilterDescriptionManager {
			public IReactiveProperty<IFilteringCondition?> CurrentFilteringCondition {
				get;
			} = new ReactivePropertySlim<IFilteringCondition?>();

			public ReadOnlyReactiveCollection<IFilteringCondition> FilteringConditions {
				get;
			} = new ReactiveCollection<IFilteringCondition>().ToReadOnlyReactiveCollection();

			public IObservable<Unit> OnFilteringConditionChanged {
				get {
					return Observable.Empty<Unit>();
				}
			}

			public IReactiveProperty<string> Name {
				get;
			} = new ReactivePropertySlim<string>("dummy");

			public void AddCondition() {
			}

			public void RemoveCondition(IFilteringCondition filteringCondition) {
			}

			public IEnumerable<IMediaFileModel> SetFilterConditions(IEnumerable<IMediaFileModel> files) {
				return files;
			}

			public IEnumerable<MediaFile> SetFilterConditions(ILiteQueryable<MediaFile> query) {
				return query.ToEnumerable();
			}
		}

		private class DummySort : ModelBase, ISortDescriptionManager {
			public IObservable<Unit> OnSortConditionChanged {
				get {
					return Observable.Empty<Unit>();
				}
			}

			public IReactiveProperty<string> Name {
				get;
			} = new ReactivePropertySlim<string>("dummy");

			public IReactiveProperty<ISortCondition?> CurrentSortCondition {
				get;
			} = new ReactivePropertySlim<ISortCondition?>();

			public IReactiveProperty<ListSortDirection> Direction {
				get;
			} = new ReactivePropertySlim<ListSortDirection>();

			public ReadOnlyReactiveCollection<ISortCondition> SortConditions {
				get;
			} = new ReactiveCollection<ISortCondition>().ToReadOnlyReactiveCollection();

			public void AddCondition() {
			}

			public void RemoveCondition(ISortCondition sortCondition) {
			}

			public IEnumerable<IMediaFileModel> SetSortConditions(IEnumerable<IMediaFileModel> array) {
				return array;
			}
		}
	}
}
