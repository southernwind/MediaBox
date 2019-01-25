﻿using System;
using System.Collections.Generic;

using Livet.EventListeners;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Collection;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Tools;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// メディアファイルViewModel
	/// </summary>
	internal class MediaFileViewModel<T> : ViewModelBase, IMediaFileViewModel where T : MediaFileModel {
		/// <summary>
		/// メディアファイルModel
		/// </summary>
		public MediaFileModel Model {
			get {
				return this.ConcreteModel;
			}
		}

		/// <summary>
		/// 具象モデルクラス
		/// </summary>
		public T ConcreteModel {
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
		/// ファイルサイズ
		/// </summary>
		public long? FileSize {
			get {
				return this.Model.FileSize;
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
				this.Model.UpdateRate();
			}
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		public IEnumerable<TitleValuePair> Properties {
			get {
				return this.Model.Properties;
			}
		}

		/// <summary>
		/// サムネイル再作成コマンド
		/// </summary>
		public ReactiveCommand RecreateThumbnailCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mediaFile">メディアファイルModel</param>
		public MediaFileViewModel(T mediaFile) {
			this.ConcreteModel = mediaFile;
			var pcel = new PropertyChangedEventListener(this.Model, (_, e) => {
				this.RaisePropertyChanged(e.PropertyName);
			}).AddTo(this.CompositeDisposable);

			this.ExternalTools = this.Model.ExternalTools.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create).AddTo(this.CompositeDisposable);

			this.RecreateThumbnailCommand.Subscribe(x => this.Model.CreateThumbnail(ThumbnailLocation.File));

			// モデル破棄時にこのインスタンスも破棄
			this.AddTo(this.Model.CompositeDisposable);
		}
	}
}
