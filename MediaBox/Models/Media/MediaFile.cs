﻿using Livet;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SandBeige.MediaBox.Models.Media
{
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
    class MediaFile:ModelBase {
		/// <summary>
		/// ファイル名
		/// </summary>
		public ReadOnlyReactiveProperty<string> FileName {
			get;
			private set;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public ReactivePropertySlim<string> FilePath {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// サムネイルファイルパス
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> ThumbnailFilePath {
			get;
			private set;
		}

		/// <summary>
		/// サムネイルファイル名
		/// </summary>
		public ReactivePropertySlim<string> ThumbnailFileName {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns><see cref="this"/></returns>
		public MediaFile Initialize(string filePath) {
			this.FilePath.Value = filePath;
			this.FileName = this.FilePath.Select(x => Path.GetFileName(x)).ToReadOnlyReactiveProperty();
			this.ThumbnailFilePath = this.FilePath.Select(x => Path.Combine(this.Settings.GeneralSettings.ThumbnailDirectoryPath, x)).ToReadOnlyReactivePropertySlim();
			return this;
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public void CreateThumbnail() {
			using (var crypto = new SHA256CryptoServiceProvider()) {
				var file = File.ReadAllBytes(this.FilePath.Value);

				var thumbnail = ThumbnailCreator.Create(file, 200, 200).ToArray();
				var thumbFileName = $"{string.Join("", crypto.ComputeHash(thumbnail).Select(b => $"{b:X2}"))}.jpg";
				var thumbFilePath = Path.Combine(this.Settings.GeneralSettings.ThumbnailDirectoryPath, thumbFileName);
				if (!File.Exists(thumbFilePath)) {
					File.WriteAllBytes(thumbFilePath, thumbnail);
				}
				this.ThumbnailFileName.Value = thumbFileName;
			}
		}
	}
}
