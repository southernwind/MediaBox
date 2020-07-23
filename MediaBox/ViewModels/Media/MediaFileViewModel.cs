using System;

using Livet.EventListeners;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Tools;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// メディアファイルViewModel
	/// </summary>
	public class MediaFileViewModel<T> : ViewModelBase, IMediaFileViewModel where T : MediaFileModel {
		private ReadOnlyReactiveCollection<string> _tags;

		/// <summary>
		/// メディアファイルModel
		/// </summary>
		public IMediaFileModel Model {
			get {
				return this.ConcreteModel;
			}
		}

		/// <summary>
		/// 具象モデルクラス
		/// </summary>
		protected T ConcreteModel {
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
		/// ファイルサイズ
		/// </summary>
		public long? FileSize {
			get {
				return this.Model.FileSize;
			}
		}

		/// <summary>
		/// 作成日時
		/// </summary>
		public DateTime CreationTime {
			get {
				return this.Model.CreationTime;
			}
		}

		/// <summary>
		/// 編集日時
		/// </summary>
		public DateTime ModifiedTime {
			get {
				return this.Model.ModifiedTime;
			}
		}

		/// <summary>
		/// 最終アクセス日時
		/// </summary>
		public DateTime LastAccessTime {
			get {
				return this.Model.LastAccessTime;
			}
		}

		/// <summary>
		/// サムネイル
		/// </summary>
		public string ThumbnailFilePath {
			get {
				return this.Model.ThumbnailFilePath;
			}
		}

		/// <summary>
		/// 解像度
		/// </summary>
		public ComparableSize? Resolution {
			get {
				return this.Model.Resolution;
			}
		}

		/// <summary>
		/// 座標
		/// </summary>
		public GpsLocation Location {
			get {
				return this.Model.Location;
			}
		}

		/// <summary>
		/// 評価
		/// </summary>
		public int Rate {
			get {
				return this.Model.Rate;
			}
			set {
				this.Model.Rate = value;
			}
		}

		/// <summary>
		/// 不正なファイル
		/// </summary>
		public bool IsInvalid {
			get {
				return this.Model.IsInvalid;
			}
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		public ReadOnlyReactiveCollection<string> Tags {
			get {
				return this._tags ??= this.Model.Tags.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			}
		}

		/// <summary>
		/// 対象外部ツール
		/// </summary>
		public ReadOnlyReactiveCollection<ExternalToolViewModel> ExternalTools {
			get {
				return
					Get.Instance<ExternalToolsFactory>()
						.Create(this.Model.Extension)
						.ToReadOnlyReactiveCollection(x => new ExternalToolViewModel(x));
			}
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		public Attributes<string> Properties {
			get {
				return this.Model.Properties;
			}
		}

		/// <summary>
		/// 存在するファイルか否か
		/// </summary>
		public bool Exists {
			get {
				return this.Model.Exists;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mediaFile">メディアファイルModel</param>
		protected MediaFileViewModel(T mediaFile) {
			this.ConcreteModel = mediaFile;
			this.ModelForToString = mediaFile;
			new PropertyChangedEventListener(this.Model, (_, e) => {
				this.RaisePropertyChanged(e.PropertyName);
			}).AddTo(this.CompositeDisposable);

			// モデル破棄時にこのインスタンスも破棄
			this.AddTo(mediaFile.CompositeDisposable);
		}
	}
}
