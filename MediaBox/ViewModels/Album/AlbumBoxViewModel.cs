﻿using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;

namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムボックスViewModel
	/// </summary>
	internal class AlbumBoxViewModel : ViewModelBase {
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
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public AlbumBoxViewModel(AlbumBox model) {
			this.ModelForToString = model;
			this.Title = model.Title.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Children = model.Children.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create).AddTo(this.CompositeDisposable);
			this.Albums = model.Albums.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create).AddTo(this.CompositeDisposable);

			// 配下のアルバム、アルバムボックスが更新されたときにUnionも作り直す。
			this.Children.ToCollectionChanged().ToUnit()
				.Merge(this.Albums.ToCollectionChanged().ToUnit())
				.Merge(Observable.Return(Unit.Default))
				.Subscribe(_ => {
					this.Union.Clear();
					this.Union.AddRange(this.Children.Union<object>(this.Albums));
				}).AddTo(this.CompositeDisposable);
		}
	}
}
