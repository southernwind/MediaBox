using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using LiteDB;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Album.Filter {
	/// <summary>
	/// フィルターマネージャー
	/// </summary>
	public class FilterDescriptionManager : ModelBase, IFilterDescriptionManager {
		private readonly IStates _states;
		private readonly ISettings _settings;

		/// <summary>
		/// カレント条件
		/// </summary>
		public IReactiveProperty<IFilteringCondition> CurrentFilteringCondition {
			get;
		} = new ReactivePropertySlim<IFilteringCondition>(mode: ReactivePropertyMode.DistinctUntilChanged);

		/// <summary>
		/// フィルター条件変更通知Subject
		/// </summary>
		private readonly Subject<Unit> _onUpdateFilteringChanged = new Subject<Unit>();

		/// <summary>
		/// 設定値保存用名前
		/// </summary>
		public IReactiveProperty<string> Name {
			get;
		} = new ReactivePropertySlim<string>();

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
		public ReadOnlyReactiveCollection<IFilteringCondition> FilteringConditions {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManager(IStates states, ISettings settings) {
			this._states = states;
			this._settings = settings;
			IDisposable beforeCurrent = null;
			this.CurrentFilteringCondition.CombineLatest(this.Name.Where(x => x != null), (condition, name) => (condition, name))
				.Subscribe(x => {
					this._onUpdateFilteringChanged.OnNext(Unit.Default);
					beforeCurrent?.Dispose();
					beforeCurrent = x.condition?.OnUpdateFilteringConditions
						.Subscribe(_ =>
							this._onUpdateFilteringChanged.OnNext(Unit.Default));
					states.AlbumStates.CurrentFilteringCondition[x.name] = x.condition?.RestorableFilterObject;
				})
				.AddTo(this.CompositeDisposable);

			this.FilteringConditions =
				states
					.AlbumStates
					.FilteringConditions
					.ToReadOnlyReactiveCollection<IFilterObject, IFilteringCondition>(x => new FilteringCondition(x, this._settings), ImmediateScheduler.Instance);

			// 初期カレント値読み込み
			this.Name.Where(x => x != null).Subscribe(name => {
				this.CurrentFilteringCondition.Value = this.FilteringConditions.FirstOrDefault(x => x.RestorableFilterObject == states.AlbumStates.CurrentFilteringCondition[name]);
			}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// フィルターマネージャーで選択したフィルターを引数に渡されたクエリに適用して返却する。
		/// </summary>
		/// <param name="query">絞り込みクエリを適用するクエリ</param>
		/// <returns>フィルター適用後クエリ</returns>
		public IEnumerable<MediaFile> SetFilterConditions(ILiteQueryable<MediaFile> query) {
			return this.CurrentFilteringCondition.Value?.SetFilterConditions(query) ?? query.ToEnumerable();
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
			this._states.AlbumStates.FilteringConditions.Add(rfa);
			this.CurrentFilteringCondition.Value = new FilteringCondition(rfa, this._settings);
		}

		/// <summary>
		/// フィルタリング条件削除
		/// </summary>
		/// <param name="filteringCondition">削除するフィルタリング条件</param>
		public void RemoveCondition(IFilteringCondition filteringCondition) {
			this._states.AlbumStates.FilteringConditions.Remove(filteringCondition.RestorableFilterObject);
		}
	}
}
