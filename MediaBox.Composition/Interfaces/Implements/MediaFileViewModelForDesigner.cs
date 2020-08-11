using System;
using System.ComponentModel;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Implements {
	/// <summary>
	/// デザイナ用<see cref="IMediaFileViewModel"/>実装
	/// </summary>
	[Obsolete("for designer")]
	public class MediaFileViewModelForDesigner : IMediaFileViewModel {
		public event PropertyChangedEventHandler PropertyChanged;
		public void Dispose() {
		}

		public IMediaFileModel Model {
			get;
		}

		public string FileName {
			get;
		}

		public string FilePath {
			get;
		}

		public DateTime CreationTime {
			get;
		}

		public DateTime ModifiedTime {
			get;
		}

		public DateTime LastAccessTime {
			get;
		}

		public long? FileSize {
			get;
		}

		public string ThumbnailFilePath {
			get;
		}

		public ComparableSize? Resolution {
			get;
		}

		public GpsLocation Location {
			get;
		}

		public int Rate {
			get;
			set;
		}

		public bool IsInvalid {
			get;
		}

		public ReadOnlyReactiveCollection<string> Tags {
			get;
		}

		public Attributes<string> Properties {
			get;
		}

		public bool Exists {
			get;
		}
	}
}
