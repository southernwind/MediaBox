﻿using SandBeige.MediaBox.Composition.Settings;
using System;
using System.IO;

namespace SandBeige.MediaBox.Models.Settings {
	public class GeneralSettings : IGeneralSettings {
		/// <summary>
		/// データベースファイルパス
		/// </summary>
		public string DataBaseFilePath {
			get;
			set;
		} = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.db");

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		public string ThumbnailDirectoryPath {
			get;
			set;
		} = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thumbs");

		public void Dispose() {
		}
	}
}