using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// ソートマネージャー
	/// </summary>
	internal class SortDescriptionManager : ModelBase, ISortSetter {
		/// <summary>
		/// カレントソート条件
		/// </summary>
		public IReactiveProperty<SortCondition> CurrentSortCondition {
			get;
		} = new ReactivePropertySlim<SortCondition>();

		/// <summary>
		/// ソート条件リスト
		/// </summary>
		public ReadOnlyReactiveCollection<SortCondition> SortConditions {
			get;
		}

		/// <summary>
		/// ソート方向
		/// </summary>
		public IReactiveProperty<ListSortDirection> Direction {
			get;
		} = new ReactivePropertySlim<ListSortDirection>();

		/// <summary>
		/// ソート条件変更通知Subject
		/// </summary>
		private readonly Subject<Unit> _onUpdateSortConditionChanged = new Subject<Unit>();

		/// <summary>
		/// フィルター条件変更通知
		/// </summary>
		public IObservable<Unit> OnSortConditionChanged {
			get {
				return this._onUpdateSortConditionChanged.AsObservable();
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">一意な名前 フィルター条件の復元に使用する。</param>
		public SortDescriptionManager(string name) {
			// 設定値初回値読み込み
			this.SortConditions = this.States.AlbumStates.SortConditions.ToReadOnlyReactiveCollection(x => new SortCondition(x)).AddTo(this.CompositeDisposable);
			this.CurrentSortCondition.Value = this.SortConditions.FirstOrDefault(x => x.RestorableSortObject.Equals(this.States.AlbumStates.CurrentSortCondition[name]));

			// 更新
			this.CurrentSortCondition.ToUnit()
				.Merge(this.Direction.ToUnit())
				.Subscribe(_ => {
					this._onUpdateSortConditionChanged.OnNext(Unit.Default);
				});

			IDisposable before = null;
			this.CurrentSortCondition.Subscribe(x => {
				before?.Dispose();
				before = x?.OnUpdateSortConditions.Subscribe(_ => this._onUpdateSortConditionChanged.OnNext(Unit.Default));
			});


			this.CurrentSortCondition.Subscribe(x => {
				this.States.AlbumStates.CurrentSortCondition[name] = x?.RestorableSortObject;
			});
		}

		/// <summary>
		/// ソート条件適用
		/// </summary>
		/// <param name="array">ソート対象の配列</param>
		/// <returns>ソート済み配列</returns>
		public IEnumerable<IMediaFileModel> SetSortConditions(IEnumerable<IMediaFileModel> array) {
			return this.CurrentSortCondition.Value?.ApplySort(array, this.Direction.Value == ListSortDirection.Descending) ?? array;
		}

		/// <summary>
		/// ソート条件追加
		/// </summary>
		public void AddCondition() {
			var sc = new SortCondition(new RestorableSortObject());
			this.States.AlbumStates.SortConditions.Add(sc.RestorableSortObject);
			this.CurrentSortCondition.Value = sc;
		}

		/// <summary>
		/// ソート条件削除
		/// </summary>
		/// <param name="sortCondition">削除するフィルタリング条件</param>
		public void RemoveCondition(SortCondition sortCondition) {
			this.States.AlbumStates.SortConditions.Remove(sortCondition.RestorableSortObject);
		}
	}
}
