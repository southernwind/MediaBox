using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Services;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	/// <summary>
	/// スキャン設定ViewModel
	/// </summary>
	public class ScanSettingsViewModel : ViewModelBase, ISettingsViewModel {
		/// <summary>
		/// 設定名
		/// </summary>
		public string Name {
			get;
		}

		/// <summary>
		/// スキャン設定
		/// </summary>
		public ReadOnlyReactiveCollection<ScanDirectory> ScanDirectories {
			get;
		}

		/// <summary>
		/// 選択中スキャン設定
		/// </summary>
		public IReactiveProperty<ScanDirectory> SelectedScanDirectory {
			get;
		} = new ReactivePropertySlim<ScanDirectory>();

		/// <summary>
		/// スキャン設定追加
		/// </summary>
		public ReactiveCommand AddScanDirectoryCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// スキャン設定削除
		/// </summary>
		public ReactiveCommand<ScanDirectory> RemoveScanDirectoryCommand {
			get;
		} = new ReactiveCommand<ScanDirectory>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ScanSettingsViewModel(ISettings settings, IFolderSelectionDialogService folderSelectionDialogService) {
			this.Name = "スキャン設定";
			this.ScanDirectories = settings.ScanSettings.ScanDirectories.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.AddScanDirectoryCommand.Subscribe(() => {
				folderSelectionDialogService.Title = "スキャンディレクトリの選択";
				if (!folderSelectionDialogService.ShowDialog() || folderSelectionDialogService.FolderName == null) {
					return;
				}
				settings.ScanSettings.ScanDirectories.Add(
					new ScanDirectory(folderSelectionDialogService.FolderName, false, true)
				);
			});

			this.RemoveScanDirectoryCommand.Subscribe(x => {
				settings.ScanSettings.ScanDirectories.Remove(x);
			});
		}
	}
}
