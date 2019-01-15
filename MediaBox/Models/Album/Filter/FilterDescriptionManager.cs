using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Filter {
	internal class FilterDescriptionManager : ModelBase {

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		public ReactiveCollection<IFilterItemCreator> FilterItemCreators {
			get;
		} = new ReactiveCollection<IFilterItemCreator>();

		/// <summary>
		/// フィルター条件
		/// </summary>
		private readonly ReadOnlyReactiveCollection<FilterItem> _filterItems;

		private readonly Subject<Unit> _onUpdateFilteringConditions = new Subject<Unit>();

		/// <summary>
		/// フィルター条件変更通知
		/// </summary>
		public IObservable<Unit> OnUpdateFilteringConditions {
			get {
				return this._onUpdateFilteringConditions.AsObservable();
			}
		}


		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManager() {
			this._filterItems =
				this.FilterItemCreators
					.ToReadOnlyReactiveCollection(x => x.Create() as FilterItem)
					.AddTo(this.CompositeDisposable);

			this._filterItems.CollectionChangedAsObservable().Subscribe(x => {
				this._onUpdateFilteringConditions.OnNext(Unit.Default);
			});

			// ここからしか変更しないのでこれでOK
			this.FilterItemCreators.AddRange(this.States.AlbumStates.FilterItemCreators);
			this.FilterItemCreators.SynchronizeTo(this.States.AlbumStates.FilterItemCreators);
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
			this.FilterItemCreators.Add(
				new TagFilterItemCreator(tagName)
			);
		}

		/// <summary>
		/// ファイルパスフィルター追加
		/// </summary>
		/// <param name="text">ファイルパスに含まれる文字列</param>
		public void AddFilePathFilter(string text) {
			this.FilterItemCreators.Add(
				new FilePathFilterItemCreator(text)
			);
		}

		/// <summary>
		/// フィルター削除
		/// </summary>
		/// <param name="item"></param>
		public void RemoveFilter(IFilterItemCreator filterItemCreator) {
			this.FilterItemCreators.Remove(filterItemCreator);
		}
	}
}
