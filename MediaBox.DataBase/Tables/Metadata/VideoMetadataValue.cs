using System;

namespace SandBeige.MediaBox.DataBase.Tables.Metadata {
	public class VideoMetadataValue {
		private VideoFile? _videoFile;
		private string? _key;
		private string? _value;

		/// <summary>
		/// メディアファイルID
		/// </summary>
		public long MediaFileId {
			get;
			set;
		}

		/// <summary>
		/// メディアファイル
		/// </summary>
		public VideoFile VideoFile {
			get {
				return this._videoFile ?? throw new InvalidOperationException();
			}
			set {
				this._videoFile = value;
			}
		}

		/// <summary>
		/// キー
		/// </summary>
		public string Key {
			get {
				return this._key ?? throw new InvalidOperationException();
			}
			set {
				this._key = value;
			}
		}

		/// <summary>
		/// 値
		/// </summary>
		public string Value {
			get {
				return this._value ?? throw new InvalidOperationException();
			}
			set {
				this._value = value;
			}
		}
	}
}
