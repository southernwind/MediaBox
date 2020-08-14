using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// ソートマネージャー
	/// </summary>
	public class SortDescriptionManager : ModelBase, ISortDescriptionManager {
		private readonly IStates _states;
		/// <summary>
		/// カレントソート条件
		/// </summary>
		public IReactiveProperty<ISortCondition> CurrentSortCondition {
			get;
		} = new ReactivePropertySlim<ISortCondition>();

		/// <summary>
		/// ソート条件リスト
		/// </summary>
		public ReadOnlyReactiveCollection<ISortCondition> SortConditions {
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
		/// 設定値保存用名前
		/// </summary>
		public IReactiveProperty<string> Name {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SortDescriptionManager(IStates states) {
			this._states = states;
			// 設定値初回値読み込み
			this.SortConditions = this._states.AlbumStates.SortConditions.ToReadOnlyReactiveCollection<ISortObject, ISortCondition>(x => new SortCondition(x)).AddTo(this.CompositeDisposable);
			this.Name.Where(x => x != null).Subscribe(name => {
				this.CurrentSortCondition.Value = this.SortConditions.FirstOrDefault(x => x.RestorableSortObject.Equals(this._states.AlbumStates.CurrentSortCondition[name]));
			});

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


			this.CurrentSortCondition.CombineLatest(this.Name.Where(x => x != null), (condition, name) => (condition, name)).Subscribe(x => {
				this._states.AlbumStates.CurrentSortCondition[x.name] = x.condition?.RestorableSortObject;
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
			this._states.AlbumStates.SortConditions.Add(sc.RestorableSortObject);
			this.CurrentSortCondition.Value = sc;
		}

		/// <summary>
		/// ソート条件削除
		/// </summary>
		/// <param name="sortCondition">削除するフィルタリング条件</param>
		public void RemoveCondition(ISortCondition sortCondition) {
			this._states.AlbumStates.SortConditions.Remove(sortCondition.RestorableSortObject);
		}
	}
}
