using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SandBeige.MediaBox.Composition.Logging;
using Unity;
using Unity.Attributes;
using SandBeige.MediaBox.ViewModels.Media;
using SandBeige.MediaBox.Repository;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// メインウィンドウViewModel
	/// </summary>
	internal class MainWindowViewModel : ViewModelBase {
		/// <summary>
		/// メディアファイルリストViewModel
		/// </summary>
		public MediaFileListViewModel MediaListViewModel {
			get;
		}
		/// <summary>
		/// ナビゲーションメニューViewModel
		/// </summary>
		public NavigationMenuViewModel NavigationMenuViewModel {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel() {
			this.MediaListViewModel = UnityConfig.UnityContainer.Resolve<MediaFileListViewModel>().Initialize().AddTo(this.CompositeDisposable);
			this.NavigationMenuViewModel = UnityConfig.UnityContainer.Resolve<NavigationMenuViewModel>().AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// 初期処理
		/// </summary>
		public void Initialize() {
		}
	}
}
