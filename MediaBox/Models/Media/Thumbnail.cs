﻿using System;
using System.IO;
using System.Windows.Media;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Library.Creator;
using SandBeige.MediaBox.Resources;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// サムネイル
	/// </summary>
	internal class Thumbnail : ModelBase, IThumbnail {
		private ImageSource _imageSource;
		private string _fileName;
		private byte[] _binary;
		private bool _hasError;
		private bool _imageSourceCreated;

		/// <summary>
		/// サムネイル生成済みの場所
		/// </summary>
		public ThumbnailLocation Location {
			get;
			private set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName {
			get {
				return this._fileName;
			}
			set {
				this._fileName = value;
				this.Location |= ThumbnailLocation.File;
				this.UpdateImageSourceIfLoaded();
			}
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public string FilePath {
			get {
				if (this.FileName == null) {
					return null;
				}
				return Path.Combine(this.Settings.PathSettings.ThumbnailDirectoryPath.Value, this.FileName);
			}
		}

		/// <summary>
		/// サムネイル画像イメージ
		/// </summary>
		public byte[] Binary {
			private get {
				return this._binary;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._binary, value);
				this.Location |= ThumbnailLocation.Memory;
				this.UpdateImageSourceIfLoaded();
			}
		}

		/// <summary>
		/// 画像方向などを適用したイメージソース
		/// </summary>
		public ImageSource ImageSource {
			get {
				if (this._imageSource == null) {
					this.UpdateImageSource();
				}
				return this._imageSource;
			}
			private set {
				this.RaisePropertyChangedIfSet(ref this._imageSource, value);
			}
		}

		/// <summary>
		/// このサムネイルでエラーが発生しているか
		/// </summary>
		/// <remarks>
		/// エラーフラグ
		/// </remarks>
		public bool HasError {
			get {
				return this._hasError;
			}
			private set {
				this.RaisePropertyChangedIfSet(ref this._hasError, value);
			}
		}

		/// <summary>
		/// このサムネイルが有効かどうか
		/// </summary>
		public bool Enabled {
			get {
				return this.FilePath != null || this.Binary != null;
			}
		}

		/// <summary>
		/// イメージソースの更新
		/// </summary>
		/// <remarks>
		/// ファイルパスがあればファイルパスから生成、なければメモリ上のサムネイル画像イメージから生成する。
		/// 両方ない場合はエラーフラグが立つ。
		/// </remarks>
		private void UpdateImageSource() {
			try {
				this._imageSourceCreated = true;
				if (this.FilePath != null) {
					this.ImageSource = ImageSourceCreator.Create(this.FilePath);
				} else if (this.Binary != null) {
					this.ImageSource = ImageSourceCreator.Create(new MemoryStream(this.Binary, false));
				} else {
					this.HasError = true;
				}
				this.HasError = false;
			} catch (Exception ex) {
				this.Logging.Log("サムネイルイメージ生成失敗", LogLevel.Warning, ex);
				this.ImageSource = Images.NoImage;
				this.HasError = true;
			}
		}

		/// <summary>
		/// イメージソースが作成済みなら更新する。
		/// </summary>
		private void UpdateImageSourceIfLoaded() {
			if (!this._imageSourceCreated) {
				return;
			}
			this.UpdateImageSource();
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.FileName}>";
		}
	}
}
