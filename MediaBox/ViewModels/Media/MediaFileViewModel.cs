using System;
using System.Collections.Generic;
using System.Windows.Media;

using Livet.EventListeners;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Exif;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Tools;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// メディアファイルViewModel
	/// </summary>
	internal class MediaFileViewModel : ViewModelBase {
		/// <summary>
		/// メディアファイルModel
		/// </summary>
		public MediaFile Model {
			get;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName {
			get {
				return this.Model.FileName;
			}
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public string FilePath {
			get {
				return this.Model.FilePath;
			}
		}

		/// <summary>
		/// 対象外部ツール
		/// </summary>
		public ReadOnlyReactiveCollection<ExternalToolViewModel> ExternalTools {
			get;
		}

		/// <summary>
		/// フルサイズイメージ
		/// </summary>
		public ImageSource Image {
			get {
				return this.Model.Image;
			}
		}

		/// <summary>
		/// サムネイル
		/// </summary>
		public Thumbnail Thumbnail {
			get {
				return this.Model.Thumbnail;
			}
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public double? Latitude {
			get {
				return this.Model.Latitude;
			}
		}

		/// <summary>
		/// 経度
		/// </summary>
		public double? Longitude {
			get {
				return this.Model.Longitude;
			}
		}

		/// <summary>
		/// Exif情報のタイトル・値ペアリスト
		/// </summary>
		public IEnumerable<TitleValuePair> Exif {
			get {
				return this.Model.Exif?.ToTitleValuePair();
			}
		}

		/// <summary>
		/// 日付時刻
		/// </summary>
		public DateTime Date {
			get {
				return this.Model.Date;
			}
		}

		/// <summary>
		/// ファイルサイズ
		/// </summary>
		public long? FileSize {
			get {
				return this.Model.FileSize;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mediaFile">メディアファイルModel</param>
		public MediaFileViewModel(MediaFile mediaFile) {
			this.Model = mediaFile;
			var pcel = new PropertyChangedEventListener(this.Model, (_, e) => {
				this.RaisePropertyChanged(e.PropertyName);
			}).AddTo(this.CompositeDisposable);

			this.ExternalTools = this.Model.ExternalTools.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create).AddTo(this.CompositeDisposable);

			// モデル破棄時にこのインスタンスも破棄
			this.AddTo(this.Model.CompositeDisposable);
		}
	}
}
