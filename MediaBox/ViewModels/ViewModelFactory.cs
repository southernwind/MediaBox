using System;

using Prism.Services.Dialogs;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Plugin;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Album.Filter;
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
	internal class ViewModelFactory : FactoryBase<ModelBase, ViewModelBase> {
		private readonly IDialogService _dialogService;
		public ViewModelFactory(IDialogService dialogService) {
			this._dialogService = dialogService;
		}

		/// <summary>
		/// アルバムViewModelの作成
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="AlbumViewModel"/></returns>
		public AlbumViewModel Create(AlbumModel model) {
			return this.Create<AlbumModel, AlbumViewModel>(model, key => {
				var instance = new AlbumViewModel(key, this._dialogService);
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
			return this.Create<MapModel, MapViewModel>(model);
		}

		/// <summary>
		/// メディアファイル情報ViewModelの作成
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="MediaFileInformationViewModel"/></returns>
		public MediaFileInformationViewModel Create(MediaFileInformation model) {
			return this.Create<MediaFileInformation, MediaFileInformationViewModel>(model, key => {
				var instance = new MediaFileInformationViewModel(key, this._dialogService);
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
					return this.Create<ImageFileModel, ImageFileViewModel>(ifm);
				case VideoFileModel vfm:
					return this.Create<VideoFileModel, VideoFileViewModel>(vfm);
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
			return this.Create<MapPin, MapPinViewModel>(model);
		}

		/// <summary>
		/// アルバムボックスViewModelの作成
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="AlbumBoxViewModel"/></returns>
		public AlbumBoxViewModel Create(AlbumBox model) {
			return this.Create<AlbumBox, AlbumBoxViewModel>(model, key => {
				var instance = new AlbumBoxViewModel(key, this._dialogService);
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
			return this.Create<ExternalTool, ExternalToolViewModel>(model);
		}

		/// <summary>
		/// フィルタリング条件ViewModel
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="FilteringConditionViewModel"/></returns>
		public FilteringConditionViewModel Create(FilteringCondition model) {
			return this.Create<FilteringCondition, FilteringConditionViewModel>(model);
		}

		/// <summary>
		/// ソート条件ViewModel
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="SortConditionViewModel"/></returns>
		public SortConditionViewModel Create(SortCondition model) {
			return this.Create<SortCondition, SortConditionViewModel>(model);
		}

		/// <summary>
		/// プラグインViewModel
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="PluginViewModel"/></returns>
		public PluginViewModel Create(PluginModel model) {
			return this.Create<PluginModel, PluginViewModel>(model);
		}

		/// <summary>
		/// インスタンス生成関数
		/// </summary>
		/// <typeparam name="TKey">キーの型(ModelBase)</typeparam>
		/// <typeparam name="TValue">値の型(ViewModelBase)</typeparam>
		/// <param name="key">モデルインスタンス</param>
		/// <returns>作成された<see cref="ViewModelBase"/></returns>
		protected override ViewModelBase CreateInstance<TKey, TValue>(TKey key) {
			var instance = (TValue)Activator.CreateInstance(typeof(TValue), key);
			instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
			return instance;
		}
	}
}
