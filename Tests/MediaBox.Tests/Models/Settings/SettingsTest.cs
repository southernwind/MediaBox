﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;
using Unity;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Tests.Models.Settings {
	[TestFixture]
	internal class SettingsTest :TestClassBase{
		[Test]
		public void SaveLoad() {
			// Settingsを毎回作り直すようにDIコンテナ登録内容変更
			UnityConfig.UnityContainer.RegisterType<ISettings, MediaBox.Models.Settings.Settings>(TransientLifetimeManager.Instance);
			UnityConfig.UnityContainer.RegisterType<IGeneralSettings, MediaBox.Models.Settings.GeneralSettings>(TransientLifetimeManager.Instance);
			UnityConfig.UnityContainer.RegisterType<IPathSettings, MediaBox.Models.Settings.PathSettings>(TransientLifetimeManager.Instance);

			using (var defaultSettings = Get.Instance<MediaBox.Models.Settings.Settings>("")) {
				using (var settings = Get.Instance<MediaBox.Models.Settings.Settings>(Path.Combine(TestDirectories["1"], "media.conf"))) {
					AreNotEqual(defaultSettings, settings, s => s);
					AreNotEqual(defaultSettings, settings, s => s.GeneralSettings);
					AreNotEqual(defaultSettings, settings, s => s.PathSettings);
					settings.GeneralSettings.BingMapApiKey.Value = "map";
					settings.GeneralSettings.MapPinSize.Value = 132;
					settings.GeneralSettings.TargetExtensions.Value = new[] {".png", ".mp4", ".avi"};
					settings.GeneralSettings.ThumbnailHeight.Value = 610;
					settings.GeneralSettings.ThumbnailWidth.Value = 315;
					settings.PathSettings.ThumbnailDirectoryPath.Value = TestDirectories["6"];
					settings.PathSettings.DataBaseFilePath.Value = Path.Combine(TestDirectories["4"], "m.db");
					// すべてデフォルトとは別の値
					AreNotEqual(settings, defaultSettings, s => s.GeneralSettings.BingMapApiKey.Value);
					AreNotEqual(settings, defaultSettings, s => s.GeneralSettings.MapPinSize.Value);
					CollectionAreNotEqual(settings, defaultSettings, s => s.GeneralSettings.TargetExtensions.Value);
					AreNotEqual(settings, defaultSettings, s => s.GeneralSettings.ThumbnailHeight.Value);
					AreNotEqual(settings, defaultSettings, s => s.GeneralSettings.ThumbnailWidth.Value);
					AreNotEqual(settings, defaultSettings, s => s.PathSettings.ThumbnailDirectoryPath.Value);
					AreNotEqual(settings, defaultSettings, s => s.PathSettings.DataBaseFilePath.Value);
					settings.Save();
				}

				using (var settings = Get.Instance<MediaBox.Models.Settings.Settings>(Path.Combine(TestDirectories["0"], "notExists.conf"))) {
					// 存在しないファイルを指定時、ロードしてもデフォルト値のまま
					settings.Load();
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.BingMapApiKey.Value);
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.MapPinSize.Value);
					CollectionAreEqual(settings, defaultSettings, s => s.GeneralSettings.TargetExtensions.Value);
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.ThumbnailHeight.Value);
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.ThumbnailWidth.Value);
					AreEqual(settings, defaultSettings, s => s.PathSettings.ThumbnailDirectoryPath.Value);
					AreEqual(settings, defaultSettings, s => s.PathSettings.DataBaseFilePath.Value);
				}
				using (var settings = Get.Instance<MediaBox.Models.Settings.Settings>(Path.Combine(TestDirectories["1"], "media.conf"))) {
					// Saveしておいた値を読み出し可能
					settings.Load();
					Assert.AreEqual("map", settings.GeneralSettings.BingMapApiKey.Value);
					Assert.AreEqual(132, settings.GeneralSettings.MapPinSize.Value);
					CollectionAssert.AreEqual(
						new[] { ".png", ".mp4", ".avi" }, settings.GeneralSettings.TargetExtensions.Value);
					Assert.AreEqual(610, settings.GeneralSettings.ThumbnailHeight.Value);
					Assert.AreEqual(315, settings.GeneralSettings.ThumbnailWidth.Value);
					Assert.AreEqual(TestDirectories["6"], settings.PathSettings.ThumbnailDirectoryPath.Value);
					Assert.AreEqual(Path.Combine(TestDirectories["4"], "m.db"), settings.PathSettings.DataBaseFilePath.Value);
				}

				File.AppendAllText(Path.Combine(TestDirectories["1"], "media.conf"), "noise");
				using (var settings = Get.Instance<MediaBox.Models.Settings.Settings>(Path.Combine(TestDirectories["1"], "media.conf"))) {
					// 存在していても壊れたファイルの場合はデフォルト値
					// TODO : ユーザーに何も伝えずデフォルト値でいいのか、要検討
					settings.Load();
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.BingMapApiKey.Value);
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.MapPinSize.Value);
					CollectionAreEqual(settings, defaultSettings, s => s.GeneralSettings.TargetExtensions.Value);
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.ThumbnailHeight.Value);
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.ThumbnailWidth.Value);
					AreEqual(settings, defaultSettings, s => s.PathSettings.ThumbnailDirectoryPath.Value);
					AreEqual(settings, defaultSettings, s => s.PathSettings.DataBaseFilePath.Value);
				}

				XamlServices.Save(Path.Combine(TestDirectories["1"], "otherType.conf"), defaultSettings.GeneralSettings);
				using (var settings = Get.Instance<MediaBox.Models.Settings.Settings>(Path.Combine(TestDirectories["1"], "otherType.conf"))) {
					// 存在していても他の型の場合はデフォルト値
					// TODO : ユーザーに何も伝えずデフォルト値でいいのか、要検討
					settings.Load();
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.BingMapApiKey.Value);
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.MapPinSize.Value);
					CollectionAreEqual(settings, defaultSettings, s => s.GeneralSettings.TargetExtensions.Value);
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.ThumbnailHeight.Value);
					AreEqual(settings, defaultSettings, s => s.GeneralSettings.ThumbnailWidth.Value);
					AreEqual(settings, defaultSettings, s => s.PathSettings.ThumbnailDirectoryPath.Value);
					AreEqual(settings, defaultSettings, s => s.PathSettings.DataBaseFilePath.Value);
				}
			}
		}
		private static void AreEqual<T>(ISettings settings1, ISettings settings2, Func<ISettings, T> selector) {
			Assert.AreEqual(selector(settings1),selector(settings2));
		}
		private static void AreNotEqual<T>(ISettings settings1, ISettings settings2, Func<ISettings, T> selector) {
			Assert.AreNotEqual(selector(settings1), selector(settings2));
		}
		
		private static void CollectionAreEqual<T>(ISettings settings1, ISettings settings2, Func<ISettings, T> selector) where T:IEnumerable{
			CollectionAssert.AreEqual(selector(settings1), selector(settings2));
		}
		private static void CollectionAreNotEqual<T>(ISettings settings1, ISettings settings2, Func<ISettings, T> selector) where T : IEnumerable {
			CollectionAssert.AreNotEqual(selector(settings1), selector(settings2));
		}
	}
}