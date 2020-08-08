
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.ViewModels.Album.Sort {
	/// <summary>
	/// ソート条件ViewModel
	/// </summary>
	public class SortConditionViewModel : ViewModelBase {
		/// <summary>
		/// モデル
		/// </summary>
		public SortCondition Model {
			get;
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
		}

		/// <summary>
		/// ソート条件
		/// </summary>
		public ReadOnlyReactiveCollection<ISortItemCreator> SortItemCreators {
			get;
		}

		/// <summary>
		///　設定用ソート項目リスト
		/// </summary>
		public IReactiveProperty<ISortItemCreator[]> CandidateSortItemCreators {
			get;
		} = new ReactiveProperty<ISortItemCreator[]>();

		/// <summary>
		/// 設定用選択中ソート項目
		/// </summary>
		public IReactiveProperty<ISortItemCreator> SelectedSortItemCreator {
			get;
		} = new ReactivePropertySlim<ISortItemCreator>();

		/// <summary>
		/// 設定用ソート方向
		/// </summary>
		public IReactiveProperty<ListSortDirection> Direction {
			get;
		} = new ReactivePropertySlim<ListSortDirection>();

		/// <summary>
		/// ソート条件追加コマンド
		/// </summary>
		public ReactiveCommand<ISortItemCreator> AddSortItemCommand {
			get;
		}

		/// <summary>
		/// ソート条件削除コマンド
		/// </summary>
		public ReactiveCommand<ISortItemCreator> RemoveSortItemCommand {
			get;
		} = new ReactiveCommand<ISortItemCreator>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model"></param>
		public SortConditionViewModel(SortCondition model) {
			this.Model = model;
			this.ModelForToString = this.Model;

			this.DisplayName = this.Model.DisplayName.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.SortItemCreators = this.Model.SortItemCreators.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.SortItemCreators.ToCollectionChanged().ToUnit()
				.Merge(this.Model.CandidateSortItemCreators.ToCollectionChanged().ToUnit())
				.Merge(Observable.Return(Unit.Default))
				.Subscribe(_ => {
					this.CandidateSortItemCreators.Value = this.Model.CandidateSortItemCreators.Where(x => !this.Model.SortItemCreators.Select(sic => sic.SortItemKey).Contains(x.SortItemKey)).ToArray();
				});

			this.AddSortItemCommand = this.SelectedSortItemCreator.Select(x => x != null).ToReactiveCommand<ISortItemCreator>().AddTo(this.CompositeDisposable);
			this.AddSortItemCommand.Subscribe(_ => {
				if (this.SelectedSortItemCreator.Value == null) {
					return;
				}
				this.SelectedSortItemCreator.Value.Direction = this.Direction.Value;
				model.AddSortItem(this.SelectedSortItemCreator.Value);
			});
			this.RemoveSortItemCommand.Subscribe(model.RemoveSortItem);
		}
	}
}
