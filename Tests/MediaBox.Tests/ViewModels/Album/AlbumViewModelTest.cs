﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	[TestFixture]
	internal class AlbumViewModelTest :ViewModelTestClassBase{
		[Test]
		public void Title() {
			var model = Get.Instance<RegisteredAlbum>();
			var vm = Get.Instance<AlbumViewModel>(model);
			Assert.IsNull(vm.Title.Value);
			model.Title.Value = "potato";
			Assert.AreEqual("potato", vm.Title.Value);
			model.Title.Value = "Controller";
			Assert.AreEqual("Controller", vm.Title.Value);
		}

		[Test]
		public void Count() {
			var model = Get.Instance<RegisteredAlbum>();
			var vm = Get.Instance<AlbumViewModel>(model);
			Assert.AreEqual(0, vm.Count.Value);
			model.Count.Value = 15;
			Assert.AreEqual(15, vm.Count.Value);
			model.Count.Value = int.MaxValue;
			Assert.AreEqual(int.MaxValue, vm.Count.Value);
		}

		[Test]
		public async Task Items() {
			var model = Get.Instance<RegisteredAlbum>();
			var vm = Get.Instance<AlbumViewModel>(model);
			Assert.AreEqual(0, vm.Items.Count);
			for (var i = 0; i < 3; i++) {
				model.Items.Add(Get.Instance<MediaFile>(""));
			}

			await Task.Delay(30);
			Assert.AreEqual(3, vm.Items.Count);
			for (var i = 0; i < 2; i++) {
				model.Items.Add(Get.Instance<MediaFile>(""));
			}
			await Task.Delay(30);
			Assert.AreEqual(5, vm.Items.Count);
			CollectionAssert.AreEqual(model.Items, vm.Items.Select(x => x.Model));
		}

		[Test]
		public async Task MonitoringDirectories() {
			var model = Get.Instance<RegisteredAlbum>();
			var vm = Get.Instance<AlbumViewModel>(model);
			Assert.AreEqual(0, vm.MonitoringDirectories.Count);
			for (var i = 0; i < 3; i++) {
				model.MonitoringDirectories.Add("a");
			}

			await Task.Delay(30);
			Assert.AreEqual(3, vm.MonitoringDirectories.Count);
			for (var i = 0; i < 2; i++) {
				model.MonitoringDirectories.Add("b");
			}
			await Task.Delay(30);
			Assert.AreEqual(5, vm.MonitoringDirectories.Count);
			CollectionAssert.AreEqual(model.MonitoringDirectories, vm.MonitoringDirectories);
		}
	}
}
