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
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.Views.Dialog;
using SandBeige.MediaBox.Views.Map;

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
		public ReadOnlyReactiveCollection<IMediaFileViewModel> Files {
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
		public IReadOnlyReactiveProperty<IMediaFileViewModel?> RepresentativeMediaFile {
			get;
		}

		/// <summary>
		/// プロパティ
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<IMediaFileProperty>> Properties {
			get;
		}

		/// <summary>
		/// メタデータ
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<IMediaFileProperty>> Metadata {
			get;
		}

		/// <summary>
		/// GPS座標
		/// </summary>
		public IReadOnlyReactiveProperty<IAddress?> Positions {
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
		public IReactiveProperty<string?> TagText {
			get;
		} = new ReactiveProperty<string?>();

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
		/// リバースジオコーディングコマンド
		/// </summary>
		public ReactiveCommand ReverseGeoCodingCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public MediaFileInformationPanelViewModel(IMediaFileInformation model, IDialogService dialogService, ViewModelFactory viewModelFactory) {
			this.FilesCount = model.FilesCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Files = model.Files.ToReadOnlyReactiveCollection(viewModelFactory.Create).AddTo(this.CompositeDisposable);
			this.Tags = model.Tags.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.RepresentativeMediaFile = model.RepresentativeMediaFile.Select(x => x == null ? null : viewModelFactory.Create(x)).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Properties = model.Properties.ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
			this.Metadata = model.Metadata.ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
			this.Positions = model.Positions.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AverageRate = model.AverageRate.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AddTagCommand = this.TagText.Select(x => !string.IsNullOrEmpty(x)).ToReactiveCommand();
			this.AddTagCommand.Where(x => !string.IsNullOrEmpty(this.TagText.Value)).Subscribe(_ => {
				model.AddTag(this.TagText.Value!);
				this.TagText.Value = null;
			}).AddTo(this.CompositeDisposable);
			this.RemoveTagCommand.Subscribe(x => {
				var param = new DialogParameters() {
					{CommonDialogWindowViewModel.ParameterNameTitle ,"確認" },
					{CommonDialogWindowViewModel.ParameterNameMessage ,$"{this.Files.Count} 件のメディアファイルからタグ [{x}] を削除します。" },
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
					{GpsSelectorWindowViewModel.ParameterNameTargetFiles ,this.Files }
				};
				dialogService.ShowDialog(nameof(GpsSelectorWindow), param, null);
			}).AddTo(this.CompositeDisposable);

			this.ReverseGeoCodingCommand.Subscribe(model.ReverseGeoCoding).AddTo(this.CompositeDisposable);

		}
	}
}
