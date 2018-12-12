﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	[TestFixture]
	internal class MediaFileCollectionViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Count() {
			var vm = Get.Instance<MediaFileCollectionViewModel>();
			Assert.AreEqual(0, vm.Count.Value);
			vm.Model.Count.Value = 15;
			Assert.AreEqual(15, vm.Count.Value);
			vm.Model.Count.Value = int.MaxValue;
			Assert.AreEqual(int.MaxValue, vm.Count.Value);
		}

		[Test]
		public async Task Items() {
			var vm = Get.Instance<MediaFileCollectionViewModel>();
			Assert.AreEqual(0, vm.Items.Count);
			for (var i = 0; i < 3; i++) {
				vm.Model.Items.Add(Get.Instance<MediaFile>(""));
			}

			await Task.Delay(30);
			Assert.AreEqual(3, vm.Items.Count);
			for (var i = 0; i < 2; i++) {
				vm.Model.Items.Add(Get.Instance<MediaFile>(""));
			}
			await Task.Delay(30);
			Assert.AreEqual(5, vm.Items.Count);
			CollectionAssert.AreEqual(vm.Model.Items, vm.Items.Select(x => x.Model));
		}

		[Test]
		public async Task AddRemove() {
			var vm = Get.Instance<MediaFileCollectionViewModel>();
			Assert.AreEqual(0, vm.Items.Count);
			for (var i = 0; i < 3; i++) {
				vm.Add(Get.Instance<MediaFileViewModel>(Get.Instance<MediaFile>("")));
			}

			await Task.Delay(30);
			Assert.AreEqual(3, vm.Items.Count);
			for (var i = 0; i < 2; i++) {
				vm.Add(Get.Instance<MediaFileViewModel>(Get.Instance<MediaFile>("")));
			}
			await Task.Delay(30);
			Assert.AreEqual(5, vm.Items.Count);
			CollectionAssert.AreEqual(vm.Model.Items, vm.Items.Select(x => x.Model));

			// Remove
			vm.Remove(vm.Items.First());
			await Task.Delay(30);
			Assert.AreEqual(4, vm.Items.Count);
			CollectionAssert.AreEqual(vm.Model.Items, vm.Items.Select(x => x.Model));

			vm.Remove(vm.Items.First());
			await Task.Delay(30);
			Assert.AreEqual(3, vm.Items.Count);
			CollectionAssert.AreEqual(vm.Model.Items, vm.Items.Select(x => x.Model));
		}

	}
}