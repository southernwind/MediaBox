using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet.Messaging.IO;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Utilities;


namespace SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages
{
	class PathSettingsViewModel : ViewModelBase, ISettingsViewModel {
		public string Name {
			get;
		}

		/// <summary>
		/// 監視ディレクトリ
		/// </summary>
		public ReactiveCollection<IMonitoringDirectory> MonitoringDirectories {
			get {
				return this.Settings.PathSettings.MonitoringDirectories;
			}
		}

		public ReactiveCommand<FolderSelectionMessage> AddMonitoringDirectoryCommand {
			get;
		} = new ReactiveCommand<FolderSelectionMessage>();

		/// <summary>
		/// 監視ディレクトリ削除
		/// </summary>
		public ReactiveCommand<IMonitoringDirectory> RemoveMonitoringDirectoryCommand {
			get;
		} = new ReactiveCommand<IMonitoringDirectory>();

		/// <summary>
		/// 監視ディレクトリドロップ
		/// </summary>
		public ReactiveCommand<IEnumerable<string>> DirectoryDragAndDrop {
			get;
		} = new ReactiveCommand<IEnumerable<string>>();


		public PathSettingsViewModel() {
			this.Name = "パス設定";

			// 監視ディレクトリドロップ
			this.DirectoryDragAndDrop.Subscribe(x => {
				this.AddMonioringDirectory(x);
			});

			// 監視ディレクトリ削除
			this.RemoveMonitoringDirectoryCommand.Subscribe(x => {
				if (x == null) {
					return;
				}
				this.MonitoringDirectories.RemoveOnScheduler(x);
			});

			this.AddMonitoringDirectoryCommand.Subscribe(x => {
				if (x.Response == null) {
					return;
				}
				this.AddMonioringDirectory(new[] { x.Response });
			});
		}

		public PathSettingsViewModel Initialize() {
			return this;
		}

		public void AddMonioringDirectory(IEnumerable<string> pathes) {
			this.MonitoringDirectories.AddRange(
				pathes.Select(
					x => {
						var md = Get.Instance<IMonitoringDirectory>();
						md.DirectoryPath.Value = x;
						md.Monitoring.Value = true;
						return md;
					}
				)
			);
		}
	}
}
