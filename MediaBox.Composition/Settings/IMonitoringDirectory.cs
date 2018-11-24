using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// 監視ディレクトリ
	/// </summary>
	public interface IMonitoringDirectory {
		IReactiveProperty<string> DirectoryPath {
			get;
			set;
		}

		IReactiveProperty<bool> Monitoring {
			get;
			set;
		}
	}
}
