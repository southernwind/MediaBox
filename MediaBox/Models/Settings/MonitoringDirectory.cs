using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Models.Settings
{
	public class MonitoringDirectory : IMonitoringDirectory {
		/// <summary>
		/// ディレクトリパス
		/// </summary>
		public IReactiveProperty<string> DirectoryPath {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// 監視する/しない
		/// </summary>
		public IReactiveProperty<bool> Monitoring {
			get;
			set;
		} = new ReactiveProperty<bool>();
	}
}
