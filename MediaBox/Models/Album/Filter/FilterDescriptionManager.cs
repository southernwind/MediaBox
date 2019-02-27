﻿using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album.Filter {
	/// <summary>
	/// フィルターマネージャー
	/// </summary>
	/// <remarks>
	/// DIコンテナによってSingletonとして扱われる。
	/// </remarks>
	internal class FilterDescriptionManager : ModelBase {

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
		/// フィルタリング更新トリガープロパティ名
		/// </summary>
		public string[] Properties {
			get {
				return this.CurrentFilteringCondition.Value?.Properties ?? Array.Empty<string>();
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManager() {
			IDisposable beforeCurrent = null;
			this.CurrentFilteringCondition
				.Subscribe(x => {
					this._onUpdateFilteringChanged.OnNext(Unit.Default);
					beforeCurrent?.Dispose();
					beforeCurrent = x?.OnUpdateFilteringConditions
						.Subscribe(_ =>
							this._onUpdateFilteringChanged.OnNext(Unit.Default));
					this.States.AlbumStates.CurrentFilteringCondition.Value = x?.FilterId;
				})
				.AddTo(this.CompositeDisposable);

			this.FilteringConditions =
				this.States
					.AlbumStates
					.FilteringConditions
					.ToReadOnlyReactiveCollection(
						x => {
							var fc = Get.Instance<FilteringCondition>(x);
							fc.Load();
							return fc;
						});

			this.CurrentFilteringCondition.Value =
				this.FilteringConditions
					.FirstOrDefault(x => x.FilterId == this.States.AlbumStates.CurrentFilteringCondition.Value);
		}

		/// <summary>
		/// カレントフィルター条件に合致しているか判定する
		/// </summary>
		/// <param name="mediaFile">メディアファイルインスタンス</param>
		/// <returns>結果</returns>
		public bool Filter(IMediaFileModel mediaFile) {
			return this.CurrentFilteringCondition.Value?.Filter(mediaFile) ?? true;
		}

		/// <summary>
		/// フィルタリング条件追加
		/// </summary>
		public void AddCondition() {
			var filterId = this.States.AlbumStates.FilteringConditions.Union(new[] { 0 }).Max() + 1;
			this.States.AlbumStates.FilteringConditions.Add(filterId);
		}

		/// <summary>
		/// フィルタリング条件削除
		/// </summary>
		/// <param name="filteringCondition">削除するフィルタリング条件</param>
		public void RemoveCondition(FilteringCondition filteringCondition) {
			this.States.AlbumStates.FilteringConditions.Remove(filteringCondition.FilterId);
		}
	}
}
