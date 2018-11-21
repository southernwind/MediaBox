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
using System.Collections.Specialized;

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
		[ExistsDirectory]
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
		/// GPS情報を含むメディアファイルViewModelリスト
		/// </summary>
		public ReactiveCollection<MediaFileViewModel> ItemsContainsGps {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();

		/// <summary>
		/// 選択中メディアファイル
		/// </summary>
		public ReactivePropertySlim<MediaFileViewModel> CurrentItem {
			get;
		} = new ReactivePropertySlim<MediaFileViewModel>();

		/// <summary>
		/// 表示モード
		/// </summary>
		public ReactivePropertySlim<DisplayMode> DisplayMode {
			get;
		} = new ReactivePropertySlim<DisplayMode>(Media.DisplayMode.Library);

		/// <summary>
		/// 表示モード変更コマンド
		/// </summary>
		public ReactiveCommand<DisplayMode> ChangeDisplayModeCommand {
			get;
		} = new ReactiveCommand<DisplayMode>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileListViewModel() {
			// メディアファイルリストModelの生成
			this.Model = UnityConfig.UnityContainer.Resolve<MediaFileList>();

			this.Model.Load();

			this.Items = this.Model.Items.ToReadOnlyReactiveCollection(x => UnityConfig.UnityContainer.Resolve<MediaFileViewModel>().Initialize(x)).AddTo(this.CompositeDisposable);
			this.Items
				.ToCollectionChanged()
				.ObserveOnUIDispatcher()
				.Subscribe(x => {
					if (x.Value.Latitude.Value == null || x.Value.Longitude.Value == null) {
						return;
					}
					if (x.Action == NotifyCollectionChangedAction.Add) {
						this.ItemsContainsGps.Add(x.Value);
					}
					if (x.Action == NotifyCollectionChangedAction.Remove) {
						this.ItemsContainsGps.Remove(x.Value);
					}
				});

			// ディレクトリパス
			this.DirectoryPath =
				new ReactiveProperty<string>()
					.SetValidateAttribute(() => this.DirectoryPath)
					.AddTo(this.CompositeDisposable);
			this.DirectoryPath.Where(x => x != null).Subscribe(this.Model.Load);

			// 表示モード変更コマンド
			this.ChangeDisplayModeCommand.Subscribe(x => {
				this.DisplayMode.Value = x;
			});
		}

		public void Initialize()
		{
			
		}
	}

	/// <summary>
	/// 表示モード
	/// </summary>
	internal enum DisplayMode {
		/// <summary>
		/// ライブラリ表示
		/// </summary>
		Library,
		/// <summary>
		/// 詳細表示
		/// </summary>
		Detail,
		/// <summary>
		/// マップ表示
		/// </summary>
		Map
	}

}
