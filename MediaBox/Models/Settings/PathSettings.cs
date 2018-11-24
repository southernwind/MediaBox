using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Models.Settings
{
	/// <summary>
	/// パス設定
	/// </summary>
	public class PathSettings : NotificationObject, IPathSettings
	{
		/// <summary>
		/// データベースファイルパス
		/// </summary>
		public IReactiveProperty<string> DataBaseFilePath {
			get;
			set;
		} = new ReactiveProperty<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.db"));

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		public IReactiveProperty<string> ThumbnailDirectoryPath {
			get;
			set;
		} = new ReactiveProperty<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thumbs"));

		/// <summary>
		/// 監視ディレクトリ
		/// </summary>
		public ReactiveCollection<IMonitoringDirectory> MonitoringDirectories {
			get;
			set;
		} = new ReactiveCollection<IMonitoringDirectory>();

		public void Dispose() {
		}
	}
}
