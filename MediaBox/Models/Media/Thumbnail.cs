﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using SandBeige.MediaBox.Base;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// サムネイル
	/// </summary>
	class Thumbnail :ModelBase{
		/// <summary>
		/// byte配列からサムネイル生成
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public Thumbnail Initialize(byte[] image) {
			this.Image = image;
			return this;
		}

		/// <summary>
		/// ファイル名からサムネイル生成
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public Thumbnail Initialize(string fileName) {
			this.FileName = fileName;
			return this;
		}
		
		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName {
			get;
			private set;
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
		public byte[] Image{
			get;
			private set;
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