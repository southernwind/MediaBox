using System;

using Prism.Services.Dialogs;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Plugin;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Album.Box;
using SandBeige.MediaBox.ViewModels.Album.Filter;
using SandBeige.MediaBox.ViewModels.Album.Selector;
using SandBeige.MediaBox.ViewModels.Album.Sort;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;
using SandBeige.MediaBox.ViewModels.Plugin;
using SandBeige.MediaBox.ViewModels.Tools;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// ViewModelファクトリー
	/// </summary>
	/// <remarks>
	/// モデルから型に応じたViewModelを生成する。
	/// また、一度作成されたViewModelはキャッシュされ、同一キーで再度作成される場合は同一のインスタンスが返却される。
	/// ViewModelがGCされていたり、Disposeされていたりすると新しく生成しなおして返す。
	/// </remarks>
	public class ViewModelFactory : FactoryBase<IModelBase, IViewModelBase> {
		private readonly IDialogService _dialogService;
		private readonly ISettings _settings;
		private readonly ExternalToolsFactory _externalToolsFactory;
		private readonly IStates _states;
		public ViewModelFactory(IDialogService dialogService, ISettings settings, ExternalToolsFactory externalToolsFactory, IStates states) {
			this._dialogService = dialogService;
			this._settings = settings;
			this._externalToolsFactory = externalToolsFactory;
			this._states = states;
		}

		/// <summary>
		/// アルバムViewModelの作成
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="AlbumViewModel"/></returns>
		public AlbumViewModel Create(IAlbumModel model) {
			return this.Create<IAlbumModel, AlbumViewModel>(model, key => {
				var instance = new AlbumViewModel(key, this);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// アルバムViewModelの作成
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="AlbumForBoxViewModel"/></returns>
		public AlbumForBoxViewModel Create(IAlbumForBoxModel model) {
			return this.Create<IAlbumForBoxModel, AlbumForBoxViewModel>(model, key => {
				var instance = new AlbumForBoxViewModel(key);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// マップViewModelの作成
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="MapViewModel"/></returns>
		public MapViewModel Create(MapModel model) {
			return this.Create<MapModel, MapViewModel>(model, key => {
				var instance = new MapViewModel(key, this);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// モデルの型に応じたメディアファイルViewModelの作成
		/// </summary>
		/// <remarks>
		/// <see cref="IMediaFileModel"/>の実際の型が<see cref="ImageFileModel"/>であれば<see cref="ImageFileViewModel"/>が
		/// <see cref="VideoFileModel"/>であれば<see cref="VideoFileViewModel"/>が返却される。
		/// </remarks>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="ImageFileViewModel"/>もしくは<see cref="VideoFileViewModel"/></returns>
		public IMediaFileViewModel Create(IMediaFileModel model) {
			if (model == null) {
				return null;
			}
			switch (model) {
				case ImageFileModel ifm:
					return this.Create<ImageFileModel, ImageFileViewModel>(ifm, key => {
						var instance = new ImageFileViewModel(ifm, this._externalToolsFactory);
						instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
						return instance;
					});
				case VideoFileModel vfm:
					return this.Create<VideoFileModel, VideoFileViewModel>(vfm, key => {
						var instance = new VideoFileViewModel(vfm, this._settings, this._externalToolsFactory);
						instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
						return instance;
					});
				default:
					throw new ArgumentException();
			}
		}

		/// <summary>
		/// マップピンViewModelの作成
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="MapPinViewModel"/></returns>
		public MapPinViewModel Create(MapPin model) {
			return this.Create<MapPin, MapPinViewModel>(model, key => {
				var instance = new MapPinViewModel(model, this);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// アルバムボックスViewModelの作成
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="AlbumBoxViewModel"/></returns>
		public AlbumBoxViewModel Create(IAlbumBox model) {
			return this.Create<IAlbumBox, AlbumBoxViewModel>(model, key => {
				var instance = new AlbumBoxViewModel(key, this._dialogService, this);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// 外部ツールViewModel
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="ExternalToolViewModel"/></returns>
		public ExternalToolViewModel Create(ExternalTool model) {
			return this.Create<ExternalTool, ExternalToolViewModel>(model, key => {
				var instance = new ExternalToolViewModel(model);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// フィルタリング条件ViewModel
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="AlbumSelectorViewModel"/></returns>
		public AlbumSelectorViewModel Create(IAlbumSelector model) {
			return this.Create<IAlbumSelector, AlbumSelectorViewModel>(model, key => {
				var instance = new AlbumSelectorViewModel(model, this._dialogService, this._states, this);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// フィルタリング条件ViewModel
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="FilteringConditionViewModel"/></returns>
		public FilteringConditionViewModel Create(FilteringCondition model) {
			return this.Create<FilteringCondition, FilteringConditionViewModel>(model, key => {
				var instance = new FilteringConditionViewModel(model);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// ソート条件ViewModel
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="SortConditionViewModel"/></returns>
		public SortConditionViewModel Create(SortCondition model) {
			return this.Create<SortCondition, SortConditionViewModel>(model, key => {
				var instance = new SortConditionViewModel(model);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// プラグインViewModel
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="PluginViewModel"/></returns>
		public PluginViewModel Create(PluginModel model) {
			return this.Create<PluginModel, PluginViewModel>(model, key => {
				var instance = new PluginViewModel(model);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			});
		}

		/// <summary>
		/// インスタンス生成関数
		/// </summary>
		/// <typeparam name="TKey">キーの型(ModelBase)</typeparam>
		/// <typeparam name="TValue">値の型(ViewModelBase)</typeparam>
		/// <param name="key">モデルインスタンス</param>
		/// <returns>作成された<see cref="ViewModelBase"/></returns>
		protected override IViewModelBase CreateInstance<TKey, TValue>(TKey key) {
			var instance = (TValue)Activator.CreateInstance(typeof(TValue), key);
			instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
			return instance;
		}
	}
}
