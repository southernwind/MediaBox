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

using SandBeige.MediaBox.Models;
using Reactive.Bindings;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Repository;
using Unity;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using SandBeige.MediaBox.ViewModels.ValidationAttributes;
using SandBeige.MediaBox.Base;

namespace SandBeige.MediaBox.ViewModels.Media
{
	/// <summary>
	/// メディアファイルリストViewModel
	/// </summary>
	internal class MediaFileListViewModel : ViewModelBase {

		/// <summary>
		/// メディアファイルリストModel
		/// </summary>
		public MediaFileList Model {
			get;
		}

		/// <summary>
		/// ディレクトリパス
		/// </summary>
		[DirectoryExists]
		public ReactiveProperty<string> DirectoryPath {
			get;
		}

		/// <summary>
		/// メディアファイルViewModelリスト
		/// </summary>
		public ReadOnlyReactiveCollection<MediaFileViewModel> Items {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileListViewModel() {
			// メディアファイルリストModelの生成
			this.Model = UnityConfig.UnityContainer.Resolve<MediaFileList>();

			this.Items = this.Model.Items.ToReadOnlyReactiveCollection(x => UnityConfig.UnityContainer.Resolve<MediaFileViewModel>().Initialize(x)).AddTo(this.CompositeDisposable);
			
			// ディレクトリパス
			this.DirectoryPath =
				new ReactiveProperty<string>()
					.SetValidateAttribute(() => this.DirectoryPath)
					.AddTo(this.CompositeDisposable);
			this.DirectoryPath.Where(x => x != null).Subscribe(this.Model.Load);

		}
		public void Initialize()
        {
        }
    }
}
