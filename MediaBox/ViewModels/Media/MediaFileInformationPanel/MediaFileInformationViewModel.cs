using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media.ThumbnailCreator;
using SandBeige.MediaBox.Views.Dialog;
using SandBeige.MediaBox.Views.Map;
using SandBeige.MediaBox.Views.Media.ThumbnailCreator;

namespace SandBeige.MediaBox.ViewModels.Media.MediaFileInformationPanel {

	/// <summary>
	/// メディアファイル情報ViewModel
	/// 複数のメディアファイルの情報をまとめて閲覧できるようにする
	/// </summary>
	public class MediaFileInformationPanelViewModel : ViewModelBase {

		/// <summary>
		/// ファイル数
		/// </summary>
		public IReadOnlyReactiveProperty<int> FilesCount {
			get;
		}

		/// <summary>
		/// ファイルリスト
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<IMediaFileViewModel>> Files {
			get;
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		}

		/// <summary>
		/// 代表メディア
		/// </summary>
		public IReadOnlyReactiveProperty<IMediaFileViewModel> RepresentativeMediaFile {
			get;
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<MediaFileProperty>> Properties {
			get;
		}

		/// <summary>
		/// メタデータ
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<MediaFileProperty>> Metadata {
			get;
		}

		/// <summary>
		/// GPS座標
		/// </summary>
		public IReadOnlyReactiveProperty<Address> Positions {
			get;
		}

		/// <summary>
		/// 評価平均
		/// </summary>
		public IReadOnlyReactiveProperty<double> AverageRate {
			get;
		}

		/// <summary>
		/// 追加用タグテキスト
		/// </summary>
		public IReactiveProperty<string> TagText {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// タグ追加コマンド
		/// </summary>
		public ReactiveCommand AddTagCommand {
			get;
		}

		/// <summary>
		/// タグ削除コマンド
		/// </summary>
		public ReactiveCommand<string> RemoveTagCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// GPS設定ウィンドウオープン
		/// </summary>
		public ReactiveCommand OpenGpsSelectorWindowCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 評価更新コマンド
		/// </summary>
		public ReactiveCommand<int> SetRateCommand {
			get;
		} = new ReactiveCommand<int>();

		/// <summary>
		/// サムネイル再作成コマンド
		/// </summary>
		public ReactiveCommand RecreateThumbnailCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// サムネイル作成ウィンドウの起動コマンド
		/// </summary>
		public ReactiveCommand CreateVideoThumbnailWithSpecificSceneCommand {
			get;
		}

		/// <summary>
		/// リバースジオコーディングコマンド
		/// </summary>
		public ReactiveCommand ReverseGeoCodingCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// ディレクトリを開く
		/// </summary>
		public ReactiveCommand<string> OpenDirectoryCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// 登録から削除コマンド
		/// </summary>
		public ReactiveCommand DeleteFileFromRegistryCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public MediaFileInformationPanelViewModel(MediaFileInformation model, IDialogService dialogService, ViewModelFactory viewModelFactory) {
			this.FilesCount = model.FilesCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Files = model.Files.Select(x => x.Select(viewModelFactory.Create)).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Tags = model.Tags.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.RepresentativeMediaFile = model.RepresentativeMediaFile.Select(viewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Properties = model.Properties.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Metadata = model.Metadata.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Positions = model.Positions.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AverageRate = model.AverageRate.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AddTagCommand = this.TagText.Select(x => !string.IsNullOrEmpty(x)).ToReactiveCommand();
			this.AddTagCommand.Subscribe(_ => {
				model.AddTag(this.TagText.Value);
				this.TagText.Value = null;
			}).AddTo(this.CompositeDisposable);
			this.RemoveTagCommand.Subscribe(x => {
				var param = new DialogParameters() {
					{CommonDialogWindowViewModel.ParameterNameTitle ,"確認" },
					{CommonDialogWindowViewModel.ParameterNameMessage ,$"{this.Files.Value.Count()} 件のメディアファイルからタグ [{x}] を削除します。" },
					{CommonDialogWindowViewModel.ParameterNameButton ,MessageBoxButton.OKCancel },
					{CommonDialogWindowViewModel.ParameterNameDefaultButton ,MessageBoxResult.Cancel},
				};
				dialogService.ShowDialog(nameof(CommonDialogWindow), param, result => {
					if (result.Result == ButtonResult.OK) {
						model.RemoveTag(x);
					}
				});
			}).AddTo(this.CompositeDisposable);
			this.OpenGpsSelectorWindowCommand.Subscribe(x => {
				var param = new DialogParameters() {
					{GpsSelectorWindowViewModel.ParameterNameTargetFiles ,this.Files.Value }
				};
				dialogService.ShowDialog(nameof(GpsSelectorWindow), param, null);
			}).AddTo(this.CompositeDisposable);

			this.SetRateCommand.Subscribe(model.SetRate);

			this.RecreateThumbnailCommand.Subscribe(x => model.CreateThumbnail());

			this.CreateVideoThumbnailWithSpecificSceneCommand = this.Files.Select(x => x?.Any(m => m is VideoFileViewModel) ?? false).ToReactiveCommand();

			this.CreateVideoThumbnailWithSpecificSceneCommand.Subscribe(_ => {
				var param = new DialogParameters() {
					{ThumbnailCreatorWindowViewModel.ParameterNameFiles,this.Files.Value.OfType<VideoFileViewModel>() }
				};
				dialogService.Show(nameof(ThumbnailCreatorWindow), param, null);
			});

			this.ReverseGeoCodingCommand.Subscribe(model.ReverseGeoCoding).AddTo(this.CompositeDisposable);

			this.OpenDirectoryCommand.Subscribe(model.OpenDirectory).AddTo(this.CompositeDisposable);

			this.DeleteFileFromRegistryCommand.Subscribe(_ => {
				var param = new DialogParameters() {
					{CommonDialogWindowViewModel.ParameterNameTitle ,"確認" },
					{CommonDialogWindowViewModel.ParameterNameMessage ,$"{this.Files.Value.Count()} 件のメディアファイルを登録からを削除します。\n(実ファイルは削除されません。)" },
					{CommonDialogWindowViewModel.ParameterNameButton ,MessageBoxButton.OKCancel },
					{CommonDialogWindowViewModel.ParameterNameDefaultButton ,MessageBoxResult.Cancel},
				};
				dialogService.ShowDialog(nameof(CommonDialogWindow), param, result => {

					if (result.Result == ButtonResult.OK) {
						model.DeleteFileFromRegistry();
					}
				});
			}).AddTo(this.CompositeDisposable);
		}
	}
}
