using Livet;
using Reactive.Bindings;
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
    class MediaFile:NotificationObject {
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
		public ReactivePropertySlim<string> ThumbnailFilePath {
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
			return this;
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public void CreateThumbnail() {
			using (var crypto = new SHA256CryptoServiceProvider()) {
				var file = File.ReadAllBytes(this.FilePath.Value);

				var thumbnail = ThumbnailCreator.Create(file, 200, 200).ToArray();
				var thumbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thumb",string.Join("", crypto.ComputeHash(thumbnail).Select(b => $"{b:X2}")) + ".jpg");
				if (!File.Exists(thumbFilePath)) {
					File.WriteAllBytes(thumbFilePath, thumbnail);
				}
				this.ThumbnailFilePath.Value = thumbFilePath;
			}
		}
	}
}
