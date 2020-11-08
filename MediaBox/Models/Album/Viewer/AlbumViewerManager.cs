using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Viewer;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Plugin;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.ViewModels.Album.Viewer;
using SandBeige.MediaBox.Views.Album.Viewer;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	public class AlbumViewerManager : ModelBase, IAlbumViewerManager {
		public ReactiveCollection<IAlbumViewerViewViewModelPairCreator> AlbumViewerList {
			get;
		} = new ReactiveCollection<IAlbumViewerViewViewModelPairCreator>();

		public AlbumViewerManager(IDialogService dialogService, PluginManager pluginManager, ISettings settings, ViewModelFactory viewModelFactory, IMapControl mapControl, IMediaFileListContextMenuViewModel contextMenuViewModel) {
			var albumViewerPluginList =
				pluginManager
					.PluginList
					.ToFilteredReadOnlyObservableCollection(x => x.PluginInstance is IAlbumViewerPlugin && x.IsEnabled.Value)
					.ToReadOnlyReactiveCollection(x => (IAlbumViewerPlugin)x.PluginInstance);
			var defaultViewers = new[] {
				new AlbumViewerViewViewModelPairCreator("詳細",()=>new Detail(),(a,c)=>new DetailViewerViewModel(a,c)),
				new AlbumViewerViewViewModelPairCreator("タイル",()=>new Tile(),(a,c) => new TileViewerViewModel(a,c)),
				new AlbumViewerViewViewModelPairCreator("リスト",()=>new List(),(a,c)=>new ListViewerViewModel(a,dialogService,settings,c)),
				new AlbumViewerViewViewModelPairCreator("マップ",()=>new Views.Album.Viewer.Map(),(a,c)=>new MapViewerViewModel(a,settings,viewModelFactory,mapControl,c))
			};

			albumViewerPluginList.CollectionChangedAsObservable().ToUnit().Merge(Observable.Return(Unit.Default)).Subscribe(_ => {
				this.AlbumViewerList.Clear();
				this.AlbumViewerList.AddRange(
					defaultViewers
						.Union(
							albumViewerPluginList.Select(
								x => new AlbumViewerViewViewModelPairCreator(
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
