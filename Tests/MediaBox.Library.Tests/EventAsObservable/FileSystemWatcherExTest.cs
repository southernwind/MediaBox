using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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
		public async Task Created() {
			var args = new List<FileSystemEventArgs>();
			using (var fsw = new FileSystemWatcher {
				Path = _testDir,
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			}) {
				var path = Path.Combine(_testDir, "create");

				using (fsw.CreatedAsObservable().Subscribe(args.Add)) {
					args.Count.Is(0);

					using (File.Create(path)) {
					}
					await Task.Delay(100);
					args.Count.Is(1);
					args[0].FullPath.Is(path);
					args[0].ChangeType.Is(WatcherChangeTypes.Created);

					File.AppendAllText(path, "refactoring");
					File.Move(path, path + "2");
					File.Delete(path + "2");
					await Task.Delay(100);
				}
				using (File.Create(path)) {
				}
				await Task.Delay(100);
			}
			args.Count.Is(1);
		}

		[Test]
		public async Task Changed() {
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

					args.Count.Is(0);

					File.AppendAllText(path, "refactoring");

					await Task.Delay(100);
					args.Count.Is(1);
					args[0].FullPath.Is(path);
					args[0].ChangeType.Is(WatcherChangeTypes.Changed);

					File.Move(path, path + "2");
					File.Delete(path + "2");
					using (File.Create(path)) {
					}
					await Task.Delay(100);
				}
				File.AppendAllText(path, "refactoring");
				await Task.Delay(100);
			}
			args.Count.Is(1);
		}

		[Test]
		public async Task Renamed() {
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

					args.Count.Is(0);

					File.Move(path, path + "2");

					await Task.Delay(100);
					args.Count.Is(1);
					args[0].FullPath.Is(path + "2");
					args[0].ChangeType.Is(WatcherChangeTypes.Renamed);

					File.AppendAllText(path + "2", "refactoring");
					File.Delete(path + "2");
					using (File.Create(path)) {
					}
					await Task.Delay(100);
				}
				File.Move(path, path + "2");
				await Task.Delay(100);
			}
			args.Count.Is(1);
		}

		[Test]
		public async Task Deleted() {
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

					args.Count.Is(0);

					File.Delete(path);

					await Task.Delay(100);
					args.Count.Is(1);
					args[0].FullPath.Is(path);
					args[0].ChangeType.Is(WatcherChangeTypes.Deleted);

					using (File.Create(path)) {
					}
					File.Move(path, path + "2");
					File.AppendAllText(path + "2", "refactoring");
					await Task.Delay(100);
				}
				File.Delete(path + "2");
				await Task.Delay(100);
			}
			args.Count.Is(1);
		}

		[Test]
		public async Task Disposed() {
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
				await Task.Delay(100);
				args.Count.Is(0);
			}
			args.Count.Is(1);

			using (var fsw = new FileSystemWatcher {
				Path = _testDir,
				EnableRaisingEvents = true,
				IncludeSubdirectories = true
			}) {
				using (fsw.DisposedAsObservable().Subscribe(args.Add)) {

				}
			}
			args.Count.Is(1);
		}
	}
}
