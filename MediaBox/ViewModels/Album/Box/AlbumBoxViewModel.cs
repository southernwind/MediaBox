using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Box;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.Views.Dialog;

namespace SandBeige.MediaBox.ViewModels.Album.Box {
	/// <summary>
	/// アルバムボックスViewModel
	/// </summary>
	public class AlbumBoxViewModel : ViewModelBase {
		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReadOnlyReactiveProperty<int?> AlbumBoxId {
			get;
		}

		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReadOnlyReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// 子アルバムボックス
		/// </summary>
		public ReadOnlyReactiveCollection<AlbumBoxViewModel> Children {
			get;
		}

		/// <summary>
		/// 直下アルバム
		/// 子アルバムボックスのアルバムはここには含まれない
		/// </summary>
		public ReadOnlyReactiveCollection<AlbumForBoxViewModel> Albums {
			get;
		}

		/// <summary>
		/// 子を結合したもの(直下アルバム+子アルバムボックス)
		/// </summary>
		public ReactiveCollection<object> Union {
			get;
		} = new ReactiveCollection<object>();

		/// <summary>
		/// 子アルバムボックス追加コマンド
		/// </summary>
		public ReactiveCommand AddChildCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// アルバムボックス削除コマンド
		/// </summary>
		public ReactiveCommand RemoveCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 名前変更コマンド
		/// </summary>
		public ReactiveCommand RenameCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public AlbumBoxViewModel(AlbumBox model, IDialogService dialogService, ViewModelFactory viewModelFactory) {
			this.ModelForToString = model;
			this.AlbumBoxId = model.AlbumBoxId.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Title = model.Title.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Children = model.Children.ToReadOnlyReactiveCollection(viewModelFactory.Create).AddTo(this.CompositeDisposable);
			this.Albums = model.Albums.ToReadOnlyReactiveCollection(model.Albums.ToCollectionChanged<AlbumForBoxModel>(), viewModelFactory.Create).AddTo(this.CompositeDisposable);

			// 配下のアルバム、アルバムボックスが更新されたときにUnionも作り直す。
			this.Children.ToCollectionChanged().ToUnit()
				.Merge(this.Albums.ToCollectionChanged().ToUnit())
				.Merge(Observable.Return(Unit.Default))
				.Subscribe(_ => {
					this.Union.Clear();
					this.Union.AddRange(this.Children.Union<object>(this.Albums));
				}).AddTo(this.CompositeDisposable);

			this.AddChildCommand.Subscribe(_ => model.AddChild(null));

			this.RemoveCommand.Subscribe(model.Remove);

			this.RenameCommand.Subscribe(_ => {
				var param = new DialogParameters() {
						{RenameWindowViewModel.ParameterNameTitle ,"リネーム" },
						{RenameWindowViewModel.ParameterNameMessage ,"新しい名前を入力してください。"},
						{RenameWindowViewModel.ParameterNameInitialText ,this.Title.Value},
					};
				dialogService.ShowDialog(nameof(RenameWindow), param, result => {
					if (result.Result == ButtonResult.OK) {
						model.Rename(result.Parameters.GetValue<string>(RenameWindowViewModel.ResultParameterNameText));
					}
				});
			});
		}
	}
}
