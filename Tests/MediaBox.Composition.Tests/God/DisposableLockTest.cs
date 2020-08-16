using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.God;

namespace SandBeige.MediaBox.Composition.Tests.God {
	[TestFixture]
	internal class DisposableLockTest {
		[Test]
		public async Task 読み取り専用ロックによる同時実行() {
			using var dl = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
			var result = new List<string>();
			var read1 = Task.Run(() => {
				Thread.Sleep(300);
				using var _ = dl.DisposableEnterReadLock();
				result.Add("1-1");
				Thread.Sleep(100);
				result.Add("1-2");
			});
			var read2 = Task.Run(() => {
				Thread.Sleep(200);
				using var _ = dl.DisposableEnterReadLock();
				result.Add("2-1");
				Thread.Sleep(400);
				result.Add("2-2");
			});
			var read3 = Task.Run(() => {
				Thread.Sleep(100);
				using var _ = dl.DisposableEnterReadLock();
				result.Add("3-1");
				Thread.Sleep(400);
				result.Add("3-2");
			});

			await read1;
			await read2;
			await read3;

			result.Should().Equal(new[] { "3-1", "2-1", "1-1", "1-2", "3-2", "2-2" });
		}

		[Test]
		public async Task 書き込みロックによる排他制御() {
			using var dl = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
			var result = new List<string>();
			var write1 = Task.Run(() => {
				Thread.Sleep(550);
				using var _ = dl.DisposableEnterWriteLock();
				result.Add("1-1");
				Thread.Sleep(100);
				result.Add("1-2");
			});
			var write2 = Task.Run(() => {
				Thread.Sleep(200);
				using var _ = dl.DisposableEnterWriteLock();
				result.Add("2-1");
				Thread.Sleep(400);
				result.Add("2-2");
			});
			var write3 = Task.Run(() => {
				Thread.Sleep(100);
				using var _ = dl.DisposableEnterWriteLock();
				result.Add("3-1");
				Thread.Sleep(400);
				result.Add("3-2");
			});

			await write1;
			await write2;
			await write3;

			result.Should().Equal(new[] { "3-1", "3-2", "2-1", "2-2", "1-1", "1-2" });
		}

		[Test]
		public async Task 読み込みロック中の書き込みロック取得不可() {
			using var dl = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
			var result = new List<string>();
			var read1 = Task.Run(() => {
				Thread.Sleep(100);
				using var _ = dl.DisposableEnterReadLock();
				result.Add("r1-1");
				Thread.Sleep(300);
				result.Add("r1-2");
			});
			var write1 = Task.Run(() => {
				Thread.Sleep(200);
				using var _ = dl.DisposableEnterWriteLock();
				result.Add("w1-1");
				Thread.Sleep(100);
				result.Add("w1-2");
			});

			await read1;
			await write1;

			result.Should().Equal(new[] { "r1-1", "r1-2", "w1-1", "w1-2" });
		}

		[Test]
		public async Task 書き込みロック中の読み込みロック取得不可() {
			using var dl = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
			var result = new List<string>();
			var read1 = Task.Run(() => {
				Thread.Sleep(200);
				using var _ = dl.DisposableEnterReadLock();
				result.Add("r1-1");
				Thread.Sleep(100);
				result.Add("r1-2");
			});
			var write1 = Task.Run(() => {
				Thread.Sleep(10);
				using var _ = dl.DisposableEnterWriteLock();
				result.Add("w1-1");
				Thread.Sleep(300);
				result.Add("w1-2");
			});

			await read1;
			await write1;

			result.Should().Equal(new[] { "w1-1", "w1-2", "r1-1", "r1-2" });
		}

		[Test]
		public async Task Disposeによるロックの解除() {
			using var dl = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
			IDisposable lockObject = null;
			var result = new List<string>();

			lockObject = dl.DisposableEnterReadLock();
			result.Add("w1");

			var write2 = Task.Run(() => {
				using var _ = dl.DisposableEnterWriteLock();
				result.Add("w2");
			});

			Thread.Sleep(100);
			result.Add("dispose");
			lockObject.Dispose();

			await write2;

			result.Should().Equal(new[] { "w1", "dispose", "w2" });
		}

		[Test]
		public void EnterReadLock() {
			using var dl = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
#pragma warning disable 612
			Action act = () => dl.EnterReadLock();
#pragma warning restore 612
			act.Should().ThrowExactly<NotSupportedException>();

		}

		[Test]
		public void EnterWriteLock() {
			using var dl = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
#pragma warning disable 612
			Action act = () => dl.EnterWriteLock();
#pragma warning restore 612
			act.Should().ThrowExactly<NotSupportedException>();
		}

		[Test]
		public async Task Dispose() {
			var dl = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
			dl.Dispose();

			// Dispose後に取得したロックは排他制御を行わない
			await Task.Run(() => {
				_ = dl.DisposableEnterWriteLock();
			});
			await Task.Run(() => {
				_ = dl.DisposableEnterWriteLock();
			});
			await Task.Run(() => {
				_ = dl.DisposableEnterReadLock();
			});
			await Task.Run(() => {
				_ = dl.DisposableEnterWriteLock();
			});
			await Task.Run(() => {
				_ = dl.DisposableEnterReadLock();
			});
			await Task.Run(() => {
				_ = dl.DisposableEnterWriteLock();
			});
		}
	}
}
