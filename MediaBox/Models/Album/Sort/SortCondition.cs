using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// ソート条件
	/// </summary>
	public class SortCondition : ModelBase, ISortCondition {
		private readonly ReadOnlyReactiveCollection<ISortItem> _sortItems;

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
		/// ソート条件変更通知Subject
		/// </summary>
		private readonly Subject<Unit> _onUpdateSortConditions = new Subject<Unit>();

		/// <summary>
		/// ソート条件変更通知
		/// </summary>
		public IObservable<Unit> OnUpdateSortConditions {
			get {
				return this._onUpdateSortConditions.AsObservable();
			}
		}

		/// <summary>
		/// ソート保存用オブジェクト
		/// </summary>
		public ISortObject RestorableSortObject {
			get;
		}

		/// <summary>
		///　設定用ソート項目リスト
		/// </summary>
		public ReactiveCollection<ISortItemCreator> CandidateSortItemCreators {
			get;
		} = new ReactiveCollection<ISortItemCreator>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sortObject">保存/復元用ソートオブジェクト</param>
		public SortCondition(ISortObject sortObject) {
			this.RestorableSortObject = sortObject;
			this.DisplayName = sortObject.DisplayName.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.SortItemCreators = this.RestorableSortObject.SortItemCreators.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this._sortItems = this.SortItemCreators.ToReadOnlyReactiveCollection(x => x.Create());

			this.SortItemCreators.CollectionChangedAsObservable()
				.Subscribe(_ => {
					this._onUpdateSortConditions.OnNext(Unit.Default);
				}).AddTo(this.CompositeDisposable);

			this.CandidateSortItemCreators.AddRange(Enum.GetValues(typeof(SortItemKeys)).OfType<SortItemKeys>().Select(x => new SortItemCreator(x)));
		}

		/// <summary>
		/// ソート設定に従ってアイテムを整列して返却する。
		/// </summary>
		/// <param name="items">ソートを適用するアイテムリスト</param>
		/// <param name="reverse">ソート方向の反転を行うか否か true:反転する false:反転しない</param>
		/// <returns>結果</returns>
		public IOrderedEnumerable<IMediaFileModel> ApplySort(IEnumerable<IMediaFileModel> items, bool reverse) {
			if (this._sortItems.Count == 0) {
				throw new InvalidOperationException();
			}
			IOrderedEnumerable<IMediaFileModel> sortedItems = null!;
			foreach (var si in this._sortItems) {
				if (sortedItems == null) {
					sortedItems = si.ApplySort(items, reverse);
					continue;
				}

				sortedItems = si.ApplyThenBySort(sortedItems, reverse);
			}
			return sortedItems;
		}

		/// <summary>
		/// ソートアイテムの追加
		/// </summary>
		/// <param name="sortItem"></param>
		public void AddSortItem(ISortItemCreator sortItem) {
			this.RestorableSortObject.SortItemCreators.Add(sortItem);
		}

		/// <summary>
		/// ソートアイテムの削除
		/// </summary>
		/// <param name="sortItem"></param>
		public void RemoveSortItem(ISortItemCreator sortItem) {
			this.RestorableSortObject.SortItemCreators.Remove(sortItem);
		}
	}
}
