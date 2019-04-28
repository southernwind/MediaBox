using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Livet;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Tests.Models.Media;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumTest : MediaFileCollectionTest {
		/// <summary>
		/// テスト用インスタンス取得
		/// </summary>
		/// <returns>テスト用インスタンス</returns>
		protected override MediaFileCollection GetInstance(ObservableSynchronizedCollection<IMediaFileModel> items) {
			return new AlbumImpl(items);
		}

		[Test]
		public void カレントアイテム変更() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc) as AlbumImpl;

			var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var media4 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			var media5 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);

			album.Items.AddRange(media1, media2, media3, media4, media5);

			album.CurrentMediaFile.Value.IsNull();

			album.CurrentMediaFiles.Value = new[] { media1, media2, media3, media4 };
			album.CurrentMediaFile.Value.Is(media1);

			album.CurrentMediaFiles.Value = new[] { media3, media4 };
			album.CurrentMediaFile.Value.Is(media3);

			album.CurrentMediaFiles.Value = Array.Empty<IMediaFileModel>();
			album.CurrentMediaFile.Value.IsNull();
		}

		[Test]
		public void Mapメディアファイルコレクション() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc) as AlbumImpl;
			(album.Map.Value.Items == album.Items).IsTrue();
		}

		[Test]
		public void Mapカレントアイテム() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc) as AlbumImpl;

			var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var media4 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			var media5 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);

			album.Items.AddRange(media1, media2, media3, media4, media5);

			album.CurrentMediaFile.Value = media1;
			album.Map.Value.CurrentMediaFile.Value.Is(media1);

			album.CurrentMediaFiles.Value = new[] { media2, media4, media5 };
			album.Map.Value.CurrentMediaFiles.Value.Is(media2, media4, media5);
		}

		[Test]
		public void フィルター変更() {

		}

		[Test]
		public void メディアリストロード() {

		}

		[TestCase(DisplayMode.Detail)]
		[TestCase(DisplayMode.Library)]
		[TestCase(DisplayMode.Map)]
		public async Task ChangeDisplayMode(DisplayMode mode) {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using (var album = this.GetInstance(osc) as AlbumImpl) {
				album.ChangeDisplayMode(mode);
				await Task.Delay(10);
				album.DisplayMode.Value.Is(mode);
				this.Settings.GeneralSettings.DisplayMode.Value.Is(mode);
			}
		}

		[Test]
		public void 事前読み込み() {

		}

		private class AlbumImpl : AlbumModel {
			public AlbumImpl(ObservableSynchronizedCollection<IMediaFileModel> items) : base(items) {

			}

			protected override Expression<Func<MediaFile, bool>> WherePredicate() {
				return _ => true;
			}
		}
	}
}
