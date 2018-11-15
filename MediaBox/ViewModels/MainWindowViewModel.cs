﻿using System;
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

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// メインウィンドウViewModel
	/// </summary>
	internal class MainWindowViewModel : ViewModel {
		[Dependency]
		public ILogging _logger { get; set; }

		/// <summary>
		/// メディアファイルリストViewModel
		/// </summary>
		public MediaFileListViewModel MediaListViewModel {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel() {
			this.MediaListViewModel = UnityConfig.UnityContainer.Resolve<MediaFileListViewModel>().AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// 初期処理
		/// </summary>
		public void Initialize() {
		}
	}
}
