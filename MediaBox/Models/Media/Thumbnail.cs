using System;
using System.IO;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// サムネイル
	/// </summary>
	internal class Thumbnail : ModelBase {
		/// <summary>
		/// byte配列からサムネイル生成
		/// </summary>
		/// <param name="source"></param>
		public Thumbnail(object source) {
			switch (source) {
				case string filename:
					this.FileName = filename;
					break;
				case byte[] image:
					this.Image = image;
					break;
				default:
					throw new ArgumentException();
			}
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName {
			get;
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
		/// イメージバイナリ
		/// </summary>
		public byte[] Image {
			get;
		}

		/// <summary>
		/// 画像の方向
		/// </summary>
		public int? Orientation {
			get;
			set;
		}

		/// <summary>
		/// イメージバイナリストリーム
		/// </summary>
		public Stream ImageStream {
			get {
				if (this.Image == null) {
					return null;
				}
				return new MemoryStream(this.Image);
			}
		}

		/// <summary>
		/// ファイルパスかイメージバイナリストリームどちらかが変えるプロパティ
		/// </summary>
		public object Source {
			get {
				return this.FilePath ?? (object)this.ImageStream;
			}
		}
	}
}
