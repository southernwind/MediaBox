using System;
using System.Collections;
using System.IO;
using System.Xaml;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Repository;

using Unity;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Tests.Models.Settings {
	internal class SettingsTest : ModelTestClassBase {
		private MediaBox.Models.Settings.Settings _defaultSettings;
		public override void SetUp() {
			base.SetUp();
			this.UseFileSystem();
			// Settingsを毎回作り直すようにDIコンテナ登録内容変更
			UnityConfig.UnityContainer.RegisterType<ISettings, MediaBox.Models.Settings.Settings>(TransientLifetimeManager.Instance);
			UnityConfig.UnityContainer.RegisterType<IGeneralSettings, MediaBox.Models.Settings.GeneralSettings>(TransientLifetimeManager.Instance);
			UnityConfig.UnityContainer.RegisterType<IPathSettings, MediaBox.Models.Settings.PathSettings>(TransientLifetimeManager.Instance);

			this._defaultSettings = new MediaBox.Models.Settings.Settings("");
			this._defaultSettings.Load();
		}

		[Test]
		public void 設定変更後保存() {
			using (var settings = new MediaBox.Models.Settings.Settings(Path.Combine(this.TestDirectories["1"], "media.conf"))) {
				AreNotEqual(settings, this._defaultSettings, s => s);
				AreNotEqual(settings, this._defaultSettings, s => s.GeneralSettings);
				AreNotEqual(settings, this._defaultSettings, s => s.PathSettings);
				settings.GeneralSettings.BingMapApiKey.Value = "map";
				var etp = new ExternalToolParams();
				etp.Arguments.Value = "a g f";
				etp.Command.Value = @"C:\test\test.exe";
				etp.DisplayName.Value = "test.exe";
				etp.TargetExtensions.AddRange(".jpg", ".png");
				var etp2 = new ExternalToolParams();
				etp2.Arguments.Value = "a g f";
				etp2.Command.Value = @"C:\test\vp.exe";
				etp2.DisplayName.Value = "vp.exe";
				etp2.TargetExtensions.AddRange(".mp4", ".mov");
				settings.GeneralSettings.ExternalTools.AddRange(etp, etp2);
				settings.GeneralSettings.MapPinSize.Value = 132;
				settings.GeneralSettings.ImageExtensions.Clear();
				settings.GeneralSettings.ImageExtensions.AddRange(new[] { ".png" });
				settings.GeneralSettings.VideoExtensions.Clear();
				settings.GeneralSettings.VideoExtensions.AddRange(new[] { ".mp4", ".avi" });
				settings.GeneralSettings.ThumbnailHeight.Value = 610;
				settings.GeneralSettings.ThumbnailWidth.Value = 315;
				settings.PathSettings.ThumbnailDirectoryPath.Value = @"C:\thumb\";
				settings.PathSettings.DataBaseFilePath.Value = @"C:\test\m.db";
				settings.PathSettings.FfmpegDirectoryPath.Value = @"C:\Programs\ffmpeg\";
				settings.ScanSettings.ScanDirectories.Clear();
				settings.ScanSettings.ScanDirectories.AddRange(
					new ScanDirectory(@"C:\test\", true, true),
					new ScanDirectory(@"C:\picture\", false, false)
				);
				// すべてデフォルトとは別の値
				AreNotEqual(settings, this._defaultSettings, s => s.GeneralSettings.BingMapApiKey.Value);
				CollectionAreNotEqual(settings, this._defaultSettings, s => s.GeneralSettings.ExternalTools.Value);
				AreNotEqual(settings, this._defaultSettings, s => s.GeneralSettings.MapPinSize.Value);
				CollectionAreNotEqual(settings, this._defaultSettings, s => s.GeneralSettings.ImageExtensions);
				CollectionAreNotEqual(settings, this._defaultSettings, s => s.GeneralSettings.VideoExtensions);
				AreNotEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailHeight.Value);
				AreNotEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailWidth.Value);
				AreNotEqual(settings, this._defaultSettings, s => s.PathSettings.ThumbnailDirectoryPath.Value);
				AreNotEqual(settings, this._defaultSettings, s => s.PathSettings.DataBaseFilePath.Value);
				AreNotEqual(settings, this._defaultSettings, s => s.PathSettings.FfmpegDirectoryPath.Value);
				CollectionAreNotEqual(settings, this._defaultSettings, s => s.ScanSettings.ScanDirectories.Value);
				settings.Save();
			}
		}

		[Test]
		public void 保存していた値の読み出し() {
			this.設定変更後保存();
			using var settings = new MediaBox.Models.Settings.Settings(Path.Combine(this.TestDirectories["1"], "media.conf"));
			var etp = new ExternalToolParams();
			etp.Arguments.Value = "a g f";
			etp.Command.Value = @"C:\test\test.exe";
			etp.DisplayName.Value = "test.exe";
			etp.TargetExtensions.AddRange(".jpg", ".png");
			var etp2 = new ExternalToolParams();
			etp2.Arguments.Value = "a g f";
			etp2.Command.Value = @"C:\test\vp.exe";
			etp2.DisplayName.Value = "vp.exe";
			etp2.TargetExtensions.AddRange(".mp4", ".mov");

			// Saveしておいた値を読み出し可能
			settings.Load();
			settings.GeneralSettings.BingMapApiKey.Value.Is("map");
			settings.GeneralSettings.MapPinSize.Value.Is(132);
			settings.GeneralSettings.ExternalTools.Is(etp, etp2);
			settings.GeneralSettings.ImageExtensions.Is(".png");
			settings.GeneralSettings.VideoExtensions.Is(".mp4", ".avi");
			settings.GeneralSettings.ThumbnailHeight.Value.Is(610);
			settings.GeneralSettings.ThumbnailWidth.Value.Is(315);
			settings.PathSettings.ThumbnailDirectoryPath.Value.Is(@"C:\thumb\");
			settings.PathSettings.DataBaseFilePath.Value.Is(@"C:\test\m.db");
			settings.PathSettings.FfmpegDirectoryPath.Value.Is(@"C:\Programs\ffmpeg\");
			settings.ScanSettings.ScanDirectories.Is(
				new ScanDirectory(@"C:\test\", true, true),
				new ScanDirectory(@"C:\picture\", false, false)
			);
		}

		[Test]
		public void 存在しないファイルのロード() {
			this.設定変更後保存();
			using var settings = new MediaBox.Models.Settings.Settings(Path.Combine(this.TestDirectories["1"], "notExists.conf"));
			// 存在しないファイルを指定時、ロードしてもデフォルト値のまま
			settings.Load();

			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.BingMapApiKey.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ExternalTools.Value);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.MapPinSize.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ImageExtensions);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.VideoExtensions);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailHeight.Value);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailWidth.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.ThumbnailDirectoryPath.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.DataBaseFilePath.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.FfmpegDirectoryPath.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.ScanSettings.ScanDirectories.Value);
		}

		[Test]
		public void 存在しないファイルのロード2() {
			this.設定変更後保存();
			using var settings = new MediaBox.Models.Settings.Settings(Path.Combine(this.TestDirectories["0"], "media.conf"));
			// 存在しないファイルを指定時、ロードしてもデフォルト値のまま
			settings.Load();

			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.BingMapApiKey.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ExternalTools.Value);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.MapPinSize.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ImageExtensions);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.VideoExtensions);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailHeight.Value);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailWidth.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.ThumbnailDirectoryPath.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.DataBaseFilePath.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.FfmpegDirectoryPath.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.ScanSettings.ScanDirectories.Value);
		}

		[Test]
		public void 壊れたファイルのロード2() {
			this.設定変更後保存();

			File.AppendAllText(Path.Combine(this.TestDirectories["1"], "media.conf"), "noise");
			using var settings = new MediaBox.Models.Settings.Settings(Path.Combine(this.TestDirectories["1"], "media.conf"));
			// 存在しないファイルを指定時、ロードしてもデフォルト値のまま
			settings.Load();

			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.BingMapApiKey.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ExternalTools.Value);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.MapPinSize.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ImageExtensions);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.VideoExtensions);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailHeight.Value);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailWidth.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.ThumbnailDirectoryPath.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.DataBaseFilePath.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.FfmpegDirectoryPath.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.ScanSettings.ScanDirectories.Value);
		}

		[Test]
		public void 他の型のロード() {
			XamlServices.Save(Path.Combine(this.TestDirectories["1"], "otherType.conf"), this._defaultSettings.GeneralSettings);
			using var settings = new MediaBox.Models.Settings.Settings(Path.Combine(this.TestDirectories["1"], "otherType.conf"));
			// 存在していても他の型の場合はデフォルト値
			settings.Load();

			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.BingMapApiKey.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ExternalTools.Value);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.MapPinSize.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ImageExtensions);
			CollectionAreEqual(settings, this._defaultSettings, s => s.GeneralSettings.VideoExtensions);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailHeight.Value);
			AreEqual(settings, this._defaultSettings, s => s.GeneralSettings.ThumbnailWidth.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.ThumbnailDirectoryPath.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.DataBaseFilePath.Value);
			AreEqual(settings, this._defaultSettings, s => s.PathSettings.FfmpegDirectoryPath.Value);
			CollectionAreEqual(settings, this._defaultSettings, s => s.ScanSettings.ScanDirectories.Value);
		}


		private static void AreEqual<T>(ISettings settings1, ISettings settings2, Func<ISettings, T> selector) {
			selector(settings1).Is(selector(settings2));
		}
		private static void AreNotEqual<T>(ISettings settings1, ISettings settings2, Func<ISettings, T> selector) {
			selector(settings1).IsNot(selector(settings2));
		}

		private static void CollectionAreEqual<T>(ISettings settings1, ISettings settings2, Func<ISettings, T> selector) where T : IEnumerable {
			selector(settings1).Is(selector(settings2));
		}
		private static void CollectionAreNotEqual<T>(ISettings settings1, ISettings settings2, Func<ISettings, T> selector) where T : IEnumerable {
			selector(settings1).IsNot(selector(settings2));
		}
	}
}
