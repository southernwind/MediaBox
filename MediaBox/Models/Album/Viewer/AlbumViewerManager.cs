using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Plugins;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Plugin;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album.Viewer;
using SandBeige.MediaBox.Views.Album.Viewer;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class AlbumViewerManager : ModelBase {
		public IReactiveProperty<IAlbumViewModel> Album {
			get;
		} = new ReactivePropertySlim<IAlbumViewModel>();

		public ReactiveCollection<AlbumViewerCreator> AlbumViewerList {
			get;
		} = new ReactiveCollection<AlbumViewerCreator>();

		public AlbumViewerManager() {
			var pluginManager = Get.Instance<PluginManager>();
			var albumViewerPluginList =
				pluginManager
					.PluginList
					.ToFilteredReadOnlyObservableCollection(x => x.PluginInstance is IAlbumViewerPlugin && x.IsEnabled.Value)
					.ToReadOnlyReactiveCollection(x => x.PluginInstance as IAlbumViewerPlugin);
			var defaultViewers = new[] {
				new AlbumViewerCreator("詳細",()=>new Detail(),a=>new DetailViewModel(a)),
				new AlbumViewerCreator("タイル",()=>new Tile(),a => new TileViewModel(a)),
				new AlbumViewerCreator("リスト",()=>new List(),a=>new ListViewModel(a)),
				new AlbumViewerCreator("マップ",()=>new Views.Album.Viewer.Map(),a=>new MapViewModel(a))
			};

			albumViewerPluginList.CollectionChangedAsObservable().ToUnit().Merge(Observable.Return(Unit.Default)).Subscribe(_ => {
				this.AlbumViewerList.Clear();
				this.AlbumViewerList.AddRange(
					defaultViewers
						.Union(
							albumViewerPluginList.Select(
								x => new AlbumViewerCreator(
									x.PluginName,
									x.CreateViewerControlInstance,
									x.CreateViewModelInstance)
								)
							)
					);
			});
		}
	}
}
