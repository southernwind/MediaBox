using System.Collections.Generic;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Media {
	public interface IMediaFileInformation {
		IReactiveProperty<double> AverageRate {
			get;
		}
		ReactiveCollection<IMediaFileModel> Files {
			get;
		}
		IReadOnlyReactiveProperty<int> FilesCount {
			get;
		}
		IReactiveProperty<IEnumerable<IMediaFileProperty>> Metadata {
			get;
		}
		IReactiveProperty<IAddress?> Positions {
			get;
		}
		IReactiveProperty<IEnumerable<IMediaFileProperty>> Properties {
			get;
		}
		IReadOnlyReactiveProperty<IMediaFileModel?> RepresentativeMediaFile {
			get;
		}
		IReactiveProperty<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		}
		IReactiveProperty<bool> Updating {
			get;
		}

		void AddTag(string tagName);
		void CreateThumbnail();
		void DeleteFileFromRegistry();
		void OpenDirectory(string filePath);
		void RemoveTag(string tagName);
		void ReverseGeoCoding();
		void SetRate(int rate);
		string ToString();
	}
}