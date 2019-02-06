using System;

using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;
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

		/// <summary>
		/// アルバムViewModelの作成
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="AlbumViewModel"/></returns>
		public AlbumViewModel Create(AlbumModel model) {
			return this.Create<AlbumModel, AlbumViewModel>(model);
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
		/// <returns>作成された<see cref="MediaFileInformationsViewModel"/></returns>
		public MediaFileInformationsViewModel Create(MediaFileInformations model) {
			return this.Create<MediaFileInformations, MediaFileInformationsViewModel>(model);
		}

		/// <summary>
		/// モデルの型に応じたメディアファイルViewModelの作成
		/// </summary>
		/// <remarks>
		/// <see cref="MediaFileModel"/>の実際の型が<see cref="ImageFileModel"/>であれば<see cref="ImageFileViewModel"/>が
		/// <see cref="VideoFileModel"/>であれば<see cref="VideoFileViewModel"/>が返却される。
		/// </remarks>
		/// <param name="model">モデルインスタンス</param>
		/// <returns>作成された<see cref="ImageFileViewModel"/>もしくは<see cref="VideoFileViewModel"/></returns>
		public IMediaFileViewModel Create(MediaFileModel model) {
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
			return this.Create<AlbumBox, AlbumBoxViewModel>(model);
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
		/// インスタンス生成関数
		/// </summary>
		/// <typeparam name="TKey">キーの型(ModelBase)</typeparam>
		/// <typeparam name="TValue">値の型(ViewModelBase)</typeparam>
		/// <param name="key">モデルインスタンス</param>
		/// <returns>作成された<see cref="ViewModelBase"/></returns>
		protected override ViewModelBase CreateInstance<TKey, TValue>(TKey key) {
			var instance = Get.Instance<TValue>(key);
			instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
			return instance;
		}
	}
}
