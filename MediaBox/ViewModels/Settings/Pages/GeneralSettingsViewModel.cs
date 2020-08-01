using System;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.ViewModels.Settings.Pages {
	/// <summary>
	/// 一般設定ViewModel
	/// </summary>
	public class GeneralSettingsViewModel : ViewModelBase, ISettingsViewModel {
		/// <summary>
		/// 設定名
		/// </summary>
		public string Name {
			get;
		}

		/// <summary>
		/// 画像拡張子
		/// </summary>

		public ReadOnlyReactiveCollection<string> ImageExtensions {
			get;
		}

		/// <summary>
		/// 動画拡張子
		/// </summary>
		public ReadOnlyReactiveCollection<string> VideoExtensions {
			get;
		}

		/// <summary>
		/// 入力された画像拡張子
		/// </summary>
		public IReactiveProperty<string> InputImageExtension {
			get;
		}

		/// <summary>
		/// 入力された動画拡張子
		/// </summary>
		public IReactiveProperty<string> InputVideoExtension {
			get;
		}

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		public IReactiveProperty<string> BingMapApiKey {
			get;
		}

		/// <summary>
		/// サムネイル幅
		/// </summary>
		public IReactiveProperty<int> ThumbnailWidth {
			get;
			set;
		}

		/// <summary>
		/// サムネイル高さ
		/// </summary>
		public IReactiveProperty<int> ThumbnailHeight {
			get;
			set;
		}

		/// <summary>
		/// 動画サムネイル枚数
		/// </summary>
		public IReactiveProperty<int> NumberOfVideoThumbnail {
			get;
		}

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		public IReactiveProperty<int> MapPinSize {
			get;
			set;
		}

		/// <summary>
		/// 画像拡張子追加コマンド
		/// </summary>
		public ReactiveCommand AddImageExtensionCommand {
			get;
		}

		/// <summary>
		/// 画像拡張子削除コマンド
		/// </summary>
		public ReactiveCommand<string> RemoveImageExtensionCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// 動画拡張子追加コマンド
		/// </summary>
		public ReactiveCommand AddVideoExtensionCommand {
			get;
		}

		/// <summary>
		/// 動画拡張子追加コマンド
		/// </summary>
		public ReactiveCommand<string> RemoveVideoExtensionCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GeneralSettingsViewModel(ISettings settings) {
			this.Name = "一般設定";
			this.ImageExtensions = settings.GeneralSettings.ImageExtensions.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this.VideoExtensions = settings.GeneralSettings.VideoExtensions.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this.BingMapApiKey = settings.GeneralSettings.BingMapApiKey.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.ThumbnailWidth = settings.GeneralSettings.ThumbnailWidth.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.ThumbnailHeight = settings.GeneralSettings.ThumbnailHeight.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.NumberOfVideoThumbnail = settings.GeneralSettings.NumberOfVideoThumbnail.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.MapPinSize = settings.GeneralSettings.MapPinSize.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			// 画像拡張子
			this.InputImageExtension =
				new ReactiveProperty<string>("")
					.SetValidateNotifyError(x => Regex.IsMatch(x, @"^$|^\.[a-z0-1]+$") ? null : "不正な形式です。");
			this.AddImageExtensionCommand =
				new[] {
					this.InputImageExtension.ObserveHasErrors,
					this.InputImageExtension.Select(x => x == "" || this.ImageExtensions.Contains(x))
				}.CombineLatestValuesAreAllFalse()
				.ToReactiveCommand()
				.AddTo(this.CompositeDisposable);
			this.AddImageExtensionCommand
				.Subscribe(() => {
					settings.GeneralSettings.ImageExtensions.Add(this.InputImageExtension.Value);
					this.InputImageExtension.Value = "";
				})
				.AddTo(this.CompositeDisposable);
			this.RemoveImageExtensionCommand
				.Subscribe(x => {
					settings.GeneralSettings.ImageExtensions.Remove(x);
				}).AddTo(this.CompositeDisposable);

			// 動画拡張子
			this.InputVideoExtension =
				new ReactiveProperty<string>("")
					.SetValidateNotifyError(x => Regex.IsMatch(x, @"^$|^\.[a-z0-1]+$") ? null : "不正な形式です。");
			this.AddVideoExtensionCommand =
				new[] {
					this.InputVideoExtension.ObserveHasErrors,
					this.InputVideoExtension.Select(x => x == "" || this.VideoExtensions.Contains(x))
				}.CombineLatestValuesAreAllFalse()
				.ToReactiveCommand()
				.AddTo(this.CompositeDisposable);
			this.AddVideoExtensionCommand
				.Subscribe(() => {
					settings.GeneralSettings.VideoExtensions.Add(this.InputVideoExtension.Value);
					this.InputVideoExtension.Value = "";
				})
				.AddTo(this.CompositeDisposable);
			this.RemoveVideoExtensionCommand
				.Subscribe(x =>
					settings.GeneralSettings.VideoExtensions.Remove(x)
				).AddTo(this.CompositeDisposable);
		}
	}
}
