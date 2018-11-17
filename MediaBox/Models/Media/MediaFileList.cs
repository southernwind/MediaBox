using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Repository;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Threading;
using Unity;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルリストクラス
	/// </summary>
	internal class MediaFileList : ModelBase {
		/// <summary>
		/// メディアファイルリスト
		/// </summary>
		public ReactiveCollection<MediaFile> Items {
			get;
		} = new ReactiveCollection<MediaFile>(UIDispatcherScheduler.Default);

		public ReactiveCollection<MediaFile> Queue {
			get;
		} = new ReactiveCollection<MediaFile>();

		public MediaFileList() {
			this.Queue
				.ToCollectionChanged()
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => {
					if (x.Action == NotifyCollectionChangedAction.Add) {
						x.Value.CreateThumbnail();
						Thread.Sleep(1000);
						this.Queue.RemoveAt(x.Index);
						this.Items.Add(x.Value);
					}
				});
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="path">ディレクトリパス</param>
		public void Load(string path) {
			if (!Directory.Exists(path)) {
				return;
			}
			this.Queue.AddRangeOnScheduler(
				Directory
					.EnumerateFiles(path)
					.Where(x => new[] { ".png", ".jpg" }.Contains(Path.GetExtension(x).ToLower()))
					.Select(x => UnityConfig.UnityContainer.Resolve<MediaFile>().Initialize(x))
					.ToList());
		}
	}
}
