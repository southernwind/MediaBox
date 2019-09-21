using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;

using SandBeige.MediaBox.Composition.Interfaces.Plugins;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Plugin;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.Views.Album.Viewer;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class AlbumViewerManager : ModelBase {
		public ReactiveCollection<AlbumViewer> AlbumViewerList {
			get;
		} = new ReactiveCollection<AlbumViewer>();

		public IReactiveProperty<AlbumViewer> CurrentAlbumViewer {
			get;
		} = new ReactivePropertySlim<AlbumViewer>();

		public AlbumViewerManager() {
			var pluginManager = Get.Instance<PluginManager>();
			var albumViewerPluginList =
				pluginManager
					.PluginList
					.ToFilteredReadOnlyObservableCollection(x => x.PluginInstance is IAlbumViewerPlugin && x.IsEnabled.Value)
					.ToReadOnlyReactiveCollection(x => x.PluginInstance as IAlbumViewerPlugin);
			var defaultViewers = new[] {
				new AlbumViewer("詳細",new Detail()),
				new AlbumViewer("タイル",new Tile()),
				new AlbumViewer("リスト",new List()),
				new AlbumViewer("マップ",new Views.Album.Viewer.Map())
			};

			albumViewerPluginList.CollectionChangedAsObservable().ToUnit().Merge(Observable.Return(Unit.Default)).Subscribe(_ => {
				this.AlbumViewerList.Clear();
				this.AlbumViewerList.AddRange(defaultViewers.Union(albumViewerPluginList.Select(x => new AlbumViewer(x.PluginName, x.CreateViewerControlInstance()))));
			});

			this.CurrentAlbumViewer.Value = this.AlbumViewerList.First();
		}
	}
}
