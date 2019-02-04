﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Controls.Objects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Album.Filter {
	/// <summary>
	/// フィルターマネージャーViewModel
	/// </summary>
	internal class FilterDescriptionManagerViewModel : ViewModelBase {
		/// <summary>
		/// モデル
		/// </summary>
		private readonly FilterDescriptionManager _model;
		/// <summary>
		/// フィルター条件
		/// </summary>
		public ReadOnlyReactiveCollection<IFilterItemCreator> FilterItems {
			get;
		}

		/// <summary>
		/// タグフィルター追加コマンド
		/// </summary>
		public ReactiveCommand<string> AddTagFilterCommand {
			get;
		} = new ReactiveCommand<string>();


		/// <summary>
		/// ファイルパスフィルター追加コマンド
		/// </summary>
		public ReactiveCommand<string> AddFilePathFilterCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// 評価
		/// </summary>
		public IReactiveProperty<int?> Rate {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// 評価フィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddRateFilterCommand {
			get;
		}

		/// <summary>
		/// 解像度フィルター追加コマンド
		/// </summary>
		public ReactiveCommand AddResolutionFilterCommand {
			get;
		}

		/// <summary>
		/// 解像度幅
		/// </summary>
		public IReactiveProperty<int?> ResolutionWidth {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// 解像度高さ
		/// </summary>
		public IReactiveProperty<int?> ResolutionHeight {
			get;
		} = new ReactivePropertySlim<int?>();


		/// <summary>
		/// メディアタイプフィルター追加コマンド
		/// </summary>
		public ReactiveCommand<Type> AddMediaTypeFilterCommand {
			get;
		} = new ReactiveCommand<Type>();

		/// <summary>
		/// メディアタイプ候補
		/// </summary>
		public IEnumerable<BindingItem<Type>> MediaTypeList {
			get;
		} = new[] {
			new BindingItem<Type>("画像",typeof(ImageFileModel)),
			new BindingItem<Type>("動画",typeof(VideoFileModel))
		};

		/// <summary>
		/// フィルター削除コマンド
		/// </summary>
		public ReactiveCommand<IFilterItemCreator> RemoveFilterCommand {
			get;
		} = new ReactiveCommand<IFilterItemCreator>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManagerViewModel() {
			this._model = Get.Instance<FilterDescriptionManager>();
			this.ModelForToString = this._model;
			this.FilterItems = this.States.AlbumStates.FilterItemCreators.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			// タグ
			this.AddTagFilterCommand.Subscribe(this._model.AddTagFilter).AddTo(this.CompositeDisposable);
			// ファイルパス
			this.AddFilePathFilterCommand.Subscribe(this._model.AddFilePathFilter).AddTo(this.CompositeDisposable);
			// 評価
			this.AddRateFilterCommand = this.Rate.Select(x => x.HasValue).ToReactiveCommand();
			this.AddRateFilterCommand
				.Subscribe(_ => {
					if (this.Rate.Value is int r) {
						this._model.AddRateFilter(r);
					}
					this.Rate.Value = null;
				})
				.AddTo(this.CompositeDisposable);
			// 解像度
			this.AddResolutionFilterCommand =
				this.ResolutionWidth
					.CombineLatest(this.ResolutionHeight, (x, y) => x.HasValue && y.HasValue)
					.ToReactiveCommand();
			this.AddResolutionFilterCommand
				.Subscribe(_ => {
					if (this.ResolutionWidth.Value is int w && this.ResolutionHeight.Value is int h) {
						this._model.AddResolutionFilter(w, h);
					}
					this.ResolutionWidth.Value = null;
					this.ResolutionHeight.Value = null;
				})
				.AddTo(this.CompositeDisposable);
			// メディアタイプ
			this.AddMediaTypeFilterCommand.Subscribe(this._model.AddMediaTypeFilter);
			// 削除
			this.RemoveFilterCommand.Subscribe(this._model.RemoveFilter).AddTo(this.CompositeDisposable);
		}
	}
}
