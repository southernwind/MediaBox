﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Tests.Models.Media;
using SandBeige.MediaBox.TestUtilities.TestData;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	[TestFixture]
	internal class MediaFileViewModelTest : ViewModelTestClassBase {
		[Test]
		public void モデル() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			using var vm = new MediaFileViewModelImpl(model);
			vm.Model.Is(model);
		}

		[Test]
		public void ファイル名() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			using var vm = new MediaFileViewModelImpl(model);
			vm.FileName.Is(TestFileNames.Image1Jpg);
		}

		[Test]
		public void ファイルパス() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			using var vm = new MediaFileViewModelImpl(model);
			vm.FilePath.Is(this.TestFiles.Image1Jpg.FilePath);
		}

		[Test]
		public void ファイルサイズ() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.FileSize = 5;
			using var vm = new MediaFileViewModelImpl(model);
			vm.FileSize.Is(5);
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.FileSize = 9;
			vm.FileSize.Is(9);
			args.Where(x => x.e.PropertyName == nameof(vm.FileSize)).Count().Is(1);
		}

		[Test]
		public void 作成日時() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.CreationTime = new DateTime(5000000);
			using var vm = new MediaFileViewModelImpl(model);
			vm.CreationTime.Is(new DateTime(5000000));
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.CreationTime = new DateTime(7000000);
			vm.CreationTime.Is(new DateTime(7000000));
			args.Where(x => x.e.PropertyName == nameof(vm.CreationTime)).Count().Is(1);
		}

		[Test]
		public void 編集日時() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.ModifiedTime = new DateTime(5000000);
			using var vm = new MediaFileViewModelImpl(model);
			vm.ModifiedTime.Is(new DateTime(5000000));
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.ModifiedTime = new DateTime(7000000);
			vm.ModifiedTime.Is(new DateTime(7000000));
			args.Where(x => x.e.PropertyName == nameof(vm.ModifiedTime)).Count().Is(1);
		}

		[Test]
		public void 最終アクセス日時() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.LastAccessTime = new DateTime(5000000);
			using var vm = new MediaFileViewModelImpl(model);
			vm.LastAccessTime.Is(new DateTime(5000000));
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.LastAccessTime = new DateTime(7000000);
			vm.LastAccessTime.Is(new DateTime(7000000));
			args.Where(x => x.e.PropertyName == nameof(vm.LastAccessTime)).Count().Is(1);
		}

		[Test]
		public void 解像度() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.Resolution = new ComparableSize(200, 400);
			using var vm = new MediaFileViewModelImpl(model);
			vm.Resolution.Is(new ComparableSize(200, 400));
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.Resolution = new ComparableSize(500, 700);
			vm.Resolution.Is(new ComparableSize(500, 700));
			args.Where(x => x.e.PropertyName == nameof(vm.Resolution)).Count().Is(1);
		}

		[Test]
		public void 座標() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.Location = new GpsLocation(35.1, 135.2, 10);
			using var vm = new MediaFileViewModelImpl(model);
			vm.Location.Is(new GpsLocation(35.1, 135.2, 10));
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.Location = new GpsLocation(37.4, 132.7, 3);
			vm.Location.Is(new GpsLocation(37.4, 132.7, 3));
			args.Where(x => x.e.PropertyName == nameof(vm.Location)).Count().Is(1);
		}

		[Test]
		public void 評価() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.Rate = 4;
			using var vm = new MediaFileViewModelImpl(model);
			vm.Rate.Is(4);
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.Rate = 2;
			vm.Rate.Is(2);
			args.Where(x => x.e.PropertyName == nameof(vm.Rate)).Count().Is(1);
		}

		[Test]
		public void 不正なファイル() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.IsInvalid = true;
			using var vm = new MediaFileViewModelImpl(model);
			vm.IsInvalid.Is(true);
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.IsInvalid = false;
			vm.IsInvalid.Is(false);
			args.Where(x => x.e.PropertyName == nameof(vm.IsInvalid)).Count().Is(1);
		}

		[Test]
		public void タグリスト() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.Tags.AddRange("aaa", "bbb");
			using var vm = new MediaFileViewModelImpl(model);
			vm.Tags.Is("aaa", "bbb");
			model.Tags.Add("ccc");
			vm.Tags.Is("aaa", "bbb", "ccc");
		}

		[Test]
		public void 存在するファイルか() {
			using var model = new MediaFileTest.MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			model.Exists = true;
			using var vm = new MediaFileViewModelImpl(model);
			vm.Exists.Is(true);
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.Exists = false;
			vm.Exists.Is(false);
			args.Where(x => x.e.PropertyName == nameof(vm.Exists)).Count().Is(1);
		}

		private class MediaFileViewModelImpl : MediaFileViewModel<MediaFileTest.MediaFileModelImpl> {
			public MediaFileViewModelImpl(MediaFileTest.MediaFileModelImpl mediaFile) : base(mediaFile) {
			}
		}

	}
}
