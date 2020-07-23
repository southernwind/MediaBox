using System;
using System.Collections.Generic;
using System.Linq;

using Livet.EventListeners;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Media;
namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// 動画ファイルViewModel
	/// </summary>
	public class VideoFileViewModel : MediaFileViewModel<VideoFileModel>, IVideoFileViewModel {
		private readonly ISettings _settings;
		private int _selectedThumbnailIndex;
		/// <summary>
		/// 回転
		/// </summary>
		public int? Rotation {
			get {
				return this.ConcreteModel.Rotation;
			}
		}

		/// <summary>
		/// パフォーマンス改善のため、Bindingエラーを起こさないよう、ImageFileViewModelに合わせて作成
		/// サムネイルファイルパスを渡しておく
		/// </summary>
		public object Image {
			get {
				return this.ConcreteModel.ThumbnailFilePath;
			}
		}

		/// <summary>
		/// サムネイルファイルリスト
		/// </summary>
		public IEnumerable<string> ThumbnailFileList {
			get {
				// TODO : 変更通知
				return Enumerable.Range(0, this._settings.GeneralSettings.NumberOfVideoThumbnail.Value).Select(x => $"{this.Model.ThumbnailFilePath}{(x == 0 ? "" : $"_{x}")}");
			}
		}

		/// <summary>
		/// サムネイル作成コマンド
		/// </summary>
		public ReactiveCommand<double> CreateThumbnailCommand {
			get;
		} = new ReactiveCommand<double>();

		/// <summary>
		/// 選択中サムネイルインデックス
		/// </summary>
		public int SelectedThumbnailIndex {
			get {
				return this._selectedThumbnailIndex;
			}
			set {
				this.SetProperty(ref this._selectedThumbnailIndex, value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mediaFile">モデルインスタンス</param>
		public VideoFileViewModel(VideoFileModel mediaFile, ISettings settings) : base(mediaFile) {
			this._settings = settings;
			this.CreateThumbnailCommand.Subscribe(x => mediaFile.CreateThumbnail(this.SelectedThumbnailIndex, x));

			new PropertyChangedEventListener(this.Model, (sender, e) => {
				if (e.PropertyName == nameof(mediaFile.ThumbnailFilePath)) {
					this.RaisePropertyChanged(nameof(this.ThumbnailFileList));
				}
			});
			this._settings.GeneralSettings.NumberOfVideoThumbnail.Subscribe(_ => {
				this.RaisePropertyChanged(nameof(this.ThumbnailFileList));
			});
		}
	}
}
