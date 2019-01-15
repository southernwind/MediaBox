using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Filter {
	internal class FilterDescriptionManager : ModelBase {
		private readonly ReactiveCollection<FilterItem> _filterItems;

		private readonly Subject<Unit> _onUpdateFilteringConditions = new Subject<Unit>();
		public IObservable<Unit> OnUpdateFilteringConditions {
			get {
				return this._onUpdateFilteringConditions.AsObservable();
			}
		}

		/// <summary>
		/// フィルター条件
		/// </summary>
		public ReadOnlyReactiveCollection<FilterItem> FilterItems {
			get;
		}


		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManager() {
			this._filterItems = new ReactiveCollection<FilterItem>();
			this._filterItems.CollectionChangedAsObservable().Subscribe(x => {
				this._onUpdateFilteringConditions.OnNext(Unit.Default);
			});
			this.FilterItems = this._filterItems.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// フィルター条件に合致しているか判定する
		/// </summary>
		/// <param name="mediaFile">メディアファイルインスタンス</param>
		/// <returns>結果</returns>
		public bool Filter(MediaFile mediaFile) {
			return this._filterItems.All(x => x.Condition(mediaFile));
		}

		/// <summary>
		/// タグフィルター追加
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public void AddTagFilter(string tagName) {
			this._filterItems.Add(
				new FilterItem(
					x => x.Tags.Contains(tagName),
					$"{tagName}をタグに含む"
				)
			);
		}

		/// <summary>
		/// ファイルパスフィルター追加
		/// </summary>
		/// <param name="text">ファイルパスに含まれる文字列</param>
		public void AddFilePathFilter(string text) {
			this._filterItems.Add(
				new FilterItem(
					x => x.FilePath.Contains(text),
					$"{text}をファイルパスに含む"
				)
			);
		}

		/// <summary>
		/// フィルター削除
		/// </summary>
		/// <param name="item"></param>
		public void RemoveFilter(FilterItem item) {
			this._filterItems.Remove(item);
		}
	}
}
