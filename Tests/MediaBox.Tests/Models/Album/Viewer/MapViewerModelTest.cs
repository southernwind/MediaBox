using System;

using Livet;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Tests.Implements;

namespace SandBeige.MediaBox.Tests.Models.Album.Viewer {
	internal class MapViewerModelTest : ModelTestClassBase {
		[Test]
		public void Mapメディアファイルコレクション() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new MapViewerModel(album);
			(model.Map.Value.Items == album.Items).IsTrue();
		}

		[Test]
		public void Mapカレントアイテム() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new MapViewerModel(album);

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			using var media4 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			using var media5 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);

			album.Items.AddRange(media1, media2, media3, media4, media5);

			model.Map.Value.CurrentMediaFiles.Value.Is();

			album.CurrentMediaFiles.Value = new[] { media2, media4, media5 };
			model.Map.Value.CurrentMediaFiles.Value.Is(media2, media4, media5);

			album.CurrentMediaFiles.Value = Array.Empty<IMediaFileModel>();
			model.Map.Value.CurrentMediaFiles.Value.Is();
		}
	}
}
