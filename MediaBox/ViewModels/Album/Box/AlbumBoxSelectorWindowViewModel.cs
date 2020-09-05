using System;
using System.Reactive.Linq;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;

namespace SandBeige.MediaBox.ViewModels.Album.Box {

	/// <summary>
	/// アルバムボックス選択ViewModel
	/// </summary>
	public class AlbumBoxSelectorWindowViewModel : DialogViewModelBase {
		public static string ParameterNameId = nameof(ParameterNameId);
		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReadOnlyReactiveProperty<int?> AlbumBoxId {
			get;
		}

		/// <summary>
		/// 階層表示用アルバム格納棚
		/// </summary>
		public IReadOnlyReactiveProperty<AlbumBoxViewModel> Shelf {
			get;
		}

		/// <summary>
		/// 選択中アルバムボックス
		/// </summary>
		public IReactiveProperty<AlbumBoxViewModel> SelectedAlbumBox {
			get;
		} = new ReactivePropertySlim<AlbumBoxViewModel>();

		/// <summary>
		/// キャンセルコマンド
		/// </summary>
		public ReactiveCommand CancelCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンプリートコマンド
		/// </summary>
		public ReactiveCommand CompleteCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// ウィンドウタイトル
		/// </summary>
		public override string? Title {
			get {
				return "アルバムボックス選択";
			}
			set {
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumBoxSelectorWindowViewModel(IAlbumBoxSelector albumBoxSelector, ViewModelFactory viewModelFactory) {
			var model = albumBoxSelector.AddTo(this.CompositeDisposable);
			this.Shelf = model.Shelf.Select(viewModelFactory.Create).ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);

			this.AlbumBoxId = this.SelectedAlbumBox.Select(x => x?.AlbumBoxId.Value).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.CompleteCommand.Subscribe(x => {
				var param = new DialogParameters {
					{ ParameterNameId, this.AlbumBoxId.Value }
				};
				this.CloseRequest(ButtonResult.OK, param);
			});

			this.CancelCommand.Subscribe(x => {
				this.CloseRequest(ButtonResult.Cancel);
			});
		}
	}
}
