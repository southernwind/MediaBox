using System;
using System.ComponentModel;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
// ReSharper disable UnassignedGetOnlyAutoProperty

namespace SandBeige.MediaBox.Plugins.Viewer.TileStyle.ForMarkUp {
	internal class ImageVM : IImageFileViewModel {
		public event PropertyChangedEventHandler PropertyChanged;
		public void Dispose() {
			throw new NotImplementedException();
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

		public object Image {
			get;
		}
	}
}
