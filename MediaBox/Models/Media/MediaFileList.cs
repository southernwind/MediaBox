using Livet;
using Reactive.Bindings;
using SandBeige.MediaBox.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SandBeige.MediaBox.Models.Media
{
	/// <summary>
	/// メディアファイルリストクラス
	/// </summary>
    class MediaFileList:NotificationObject
    {
		/// <summary>
		/// メディアファイルリスト
		/// </summary>
		public ReactiveCollection<MediaFile> Items {
			get;
		} = new ReactiveCollection<MediaFile>();

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="path">ディレクトリパス</param>
		public void Load(string path) {
			if (!Directory.Exists(path)) {
				return;
			}
			this.Items.ClearOnScheduler();
			this.Items.AddRangeOnScheduler(
				Directory
					.EnumerateFiles(path)
					.Where(x => new[] { ".png", ".jpg" }.Contains(Path.GetExtension(x).ToLower()))
					.Select(x => UnityConfig.UnityContainer.Resolve<MediaFile>().Initialize(x))
					.ToList());
		}
    }
}
