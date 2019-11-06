using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Album.Filter {
	/// <summary>
	/// フィルターマネージャー
	/// </summary>
	internal class FilterDescriptionManager : ModelBase, IFilterSetter {

		/// <summary>
		/// カレント条件
		/// </summary>
		public IReactiveProperty<FilteringCondition> CurrentFilteringCondition {
			get;
		} = new ReactivePropertySlim<FilteringCondition>(mode: ReactivePropertyMode.DistinctUntilChanged);

		/// <summary>
		/// フィルター条件変更通知Subject
		/// </summary>
		private readonly Subject<Unit> _onUpdateFilteringChanged = new Subject<Unit>();

		/// <summary>
		/// フィルター条件変更通知
		/// </summary>
		public IObservable<Unit> OnFilteringConditionChanged {
			get {
				return this._onUpdateFilteringChanged.AsObservable();
			}
		}

		/// <summary>
		/// フィルター条件リスト
		/// </summary>
		public ReadOnlyReactiveCollection<FilteringCondition> FilteringConditions {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">一意な名前 フィルター条件の復元に使用する。</param>
		public FilterDescriptionManager(string name) {
			IDisposable beforeCurrent = null;
			this.CurrentFilteringCondition
				.Subscribe(x => {
					this._onUpdateFilteringChanged.OnNext(Unit.Default);
					beforeCurrent?.Dispose();
					beforeCurrent = x?.OnUpdateFilteringConditions
						.Subscribe(_ =>
							this._onUpdateFilteringChanged.OnNext(Unit.Default));
					this.States.AlbumStates.CurrentFilteringCondition[name] = x?.RestorableFilterObject;
				})
				.AddTo(this.CompositeDisposable);

			this.FilteringConditions =
				this.States
					.AlbumStates
					.FilteringConditions
					.ToReadOnlyReactiveCollection(x => new FilteringCondition(x), ImmediateScheduler.Instance);

			// 初期カレント値読み込み
			this.CurrentFilteringCondition.Value = this.FilteringConditions.FirstOrDefault(x => x.RestorableFilterObject == this.States.AlbumStates.CurrentFilteringCondition[name]);
		}

		/// <summary>
		/// フィルターマネージャーで選択したフィルターを引数に渡されたクエリに適用して返却する。
		/// </summary>
		/// <param name="query">絞り込みクエリを適用するクエリ</param>
		/// <returns>フィルター適用後クエリ</returns>
		public IEnumerable<MediaFile> SetFilterConditions(IQueryable<MediaFile> query) {
			return this.CurrentFilteringCondition.Value?.SetFilterConditions(query) ?? query;
		}

		/// <summary>
		/// フィルターマネージャーで選択したフィルターを引数に渡されたシーケンスに適用して返却する。
		/// </summary>
		/// <param name="query">絞り込みを適用するシーケンス</param>
		/// <returns>フィルター適用後シーケンス</returns>
		public IEnumerable<IMediaFileModel> SetFilterConditions(IEnumerable<IMediaFileModel> files) {
			return this.CurrentFilteringCondition.Value?.SetFilterConditions(files) ?? files;
		}

		/// <summary>
		/// フィルタリング条件追加
		/// </summary>
		public void AddCondition() {
			var rfa = new RestorableFilterObject();
			this.States.AlbumStates.FilteringConditions.Add(rfa);
			this.CurrentFilteringCondition.Value = new FilteringCondition(rfa);
		}

		/// <summary>
		/// フィルタリング条件削除
		/// </summary>
		/// <param name="filteringCondition">削除するフィルタリング条件</param>
		public void RemoveCondition(FilteringCondition filteringCondition) {
			this.States.AlbumStates.FilteringConditions.Remove(filteringCondition.RestorableFilterObject);
		}
	}
}
