using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NUnit.Framework;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Library.Tests.EventAsObservable {
	[TestFixture]
	public class FileSystemWatcherExTest {

		private static string _testDir;
		private static string _testSubDir;

		[SetUp]
		public void SetUp() {
			_testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir");
			_testSubDir = Path.Combine(_testDir, "sub");
			if (Directory.Exists(_testDir)) {
				DirectoryUtility.DirectoryDelete(_testDir);
			}
			Directory.CreateDirectory(_testDir);
			Directory.CreateDirectory(_testSubDir);
		}

		[TearDown]
		public void TearDown() {
			DirectoryUtility.DirectoryDelete(_testDir);
		}

		[Test]
		public void Created() {
			var args = new List<FileSystemEventArgs>();
			using (var fsw = new FileSystemWatcher {
				Path = _testDir,
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			}) {
				var path = Path.Combine(_testDir, "create");

				using (fsw.CreatedAsObservable().Subscribe(args.Add)) {
					Assert.AreEqual(0, args.Count);

					using (File.Create(path)) {
					}
					Thread.Sleep(100);
					Assert.AreEqual(1, args.Count);
					Assert.AreEqual(args[0].FullPath, path);
					Assert.AreEqual(args[0].ChangeType, WatcherChangeTypes.Created);

					File.AppendAllText(path, "refactoring");
					File.Move(path, path + "2");
					File.Delete(path + "2");
					Thread.Sleep(100);
				}
				using (File.Create(path)) {
				}
				Thread.Sleep(100);
			}
			Assert.AreEqual(1, args.Count);
		}

		[Test]
		public void Changed() {
			var args = new List<FileSystemEventArgs>();
			using (var fsw = new FileSystemWatcher {
				Path = _testDir,
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			}) {
				var path = Path.Combine(_testDir, "changed");

				using (File.Create(path)) {
				}
				using (fsw.ChangedAsObservable().Subscribe(args.Add)) {

					Assert.AreEqual(0, args.Count);

					File.AppendAllText(path, "refactoring");

					Thread.Sleep(100);
					Assert.AreEqual(1, args.Count);
					Assert.AreEqual(args[0].FullPath, path);
					Assert.AreEqual(args[0].ChangeType, WatcherChangeTypes.Changed);

					File.Move(path, path + "2");
					File.Delete(path + "2");
					using (File.Create(path)) {
					}
					Thread.Sleep(100);
				}
				File.AppendAllText(path, "refactoring");
				Thread.Sleep(100);
			}
			Assert.AreEqual(1, args.Count);
		}

		[Test]
		public void Renamed() {
			var args = new List<FileSystemEventArgs>();
			using (var fsw = new FileSystemWatcher {
				Path = _testDir,
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			}) {
				var path = Path.Combine(_testDir, "renamed");

				using (File.Create(path)) {
				}
				using (fsw.RenamedAsObservable().Subscribe(args.Add)) {

					Assert.AreEqual(0, args.Count);

					File.Move(path, path + "2");

					Thread.Sleep(100);
					Assert.AreEqual(1, args.Count);
					Assert.AreEqual(args[0].FullPath, path + "2");
					Assert.AreEqual(args[0].ChangeType, WatcherChangeTypes.Renamed);

					File.AppendAllText(path + "2", "refactoring");
					File.Delete(path + "2");
					using (File.Create(path)) {
					}
					Thread.Sleep(100);
				}
				File.Move(path, path + "2");
				Thread.Sleep(100);
			}
			Assert.AreEqual(1, args.Count);
		}

		[Test]
		public void Deleted() {
			var args = new List<FileSystemEventArgs>();
			using (var fsw = new FileSystemWatcher {
				Path = _testDir,
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			}) {
				var path = Path.Combine(_testDir, "deleted");

				using (File.Create(path)) {
				}
				using (fsw.DeletedAsObservable().Subscribe(args.Add)) {

					Assert.AreEqual(0, args.Count);

					File.Delete(path);

					Thread.Sleep(100);
					Assert.AreEqual(1, args.Count);
					Assert.AreEqual(args[0].FullPath, path);
					Assert.AreEqual(args[0].ChangeType, WatcherChangeTypes.Deleted);

					using (File.Create(path)) {
					}
					File.Move(path, path + "2");
					File.AppendAllText(path + "2", "refactoring");
					Thread.Sleep(100);
				}
				File.Delete(path + "2");
				Thread.Sleep(100);
			}
			Assert.AreEqual(1, args.Count);
		}

		[Test]
		public void Disposed() {
			var args = new List<EventArgs>();
			using (var fsw = new FileSystemWatcher {
				Path = _testDir,
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			}) {
				var path = Path.Combine(_testDir, "disposed");

				fsw.DisposedAsObservable().Subscribe(args.Add);

				using (File.Create(path)) {
				}
				File.AppendAllText(path, "refactoring");
				File.Move(path, path + "2");
				File.Delete(path + "2");
				Thread.Sleep(100);
				Assert.AreEqual(0, args.Count);
			}
			Assert.AreEqual(1, args.Count);

			using (var fsw = new FileSystemWatcher {
				Path = _testDir,
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			}) {
				using (fsw.DisposedAsObservable().Subscribe(args.Add)) {

				}
			}
			Assert.AreEqual(1, args.Count);
		}
	}
}
