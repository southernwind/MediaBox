using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.ViewModels.Dialog;

namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムボックスViewModel
	/// </summary>
	internal class AlbumBoxViewModel : ViewModelBase {
		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReadOnlyReactiveProperty<int?> AlbumBoxId {
			get;
		}

		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
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
		public ReadOnlyReactiveCollection<AlbumViewModel> Albums {
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
		public AlbumBoxViewModel(AlbumBox model) {
			this.ModelForToString = model;
			this.AlbumBoxId = model.AlbumBoxId.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Title = model.Title.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.Children = model.Children.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create).AddTo(this.CompositeDisposable);
			this.Albums = model.Albums.ToReadOnlyReactiveCollection(model.Albums.ToCollectionChanged<RegisteredAlbum>(), this.ViewModelFactory.Create).AddTo(this.CompositeDisposable);

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
				using var vm = new RenameViewModel("リネーム", "新しい名前を入力してください。", this.Title.Value);
				var message = new TransitionMessage(vm, "ShowRenameDialog");
				this.Messenger.Raise(message);
				if (vm.Completed) {
					model.Rename(vm.Text.Value);
				}
			});
		}
	}
}
