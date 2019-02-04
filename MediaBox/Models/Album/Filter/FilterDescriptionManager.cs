using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Filter {
	/// <summary>
	/// フィルターマネージャー
	/// DIコンテナによってSingletonとして扱われる。
	/// Add***Filterメソッドでフィルタークリエイターを<see cref="States.AlbumStates.FilterItemCreators"/>に追加し、
	/// <see cref="RemoveFilter(IFilterItemCreator)"/>メソッドで削除する。
	/// 追加されたフィルタークリエイターはフィルターを生成し、内部に保持する。
	/// フィルター条件に合致しているかの判断には<see cref="Filter(MediaFileModel)"/>を使う。
	/// </summary>
	internal class FilterDescriptionManager : ModelBase {
		/// <summary>
		/// フィルター条件
		/// </summary>
		private readonly ReadOnlyReactiveCollection<FilterItem> _filterItems;

		/// <summary>
		/// フィルター条件変更通知Subject
		/// </summary>
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
		/// フィルタリング更新トリガープロパティ名
		/// </summary>
		public string[] Properties {
			get {
				return this._filterItems.SelectMany(x => x.Properties).ToArray();
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FilterDescriptionManager() {
			this._filterItems =
				this.States.AlbumStates.FilterItemCreators
					.ToReadOnlyReactiveCollection(x => x.Create() as FilterItem)
					.AddTo(this.CompositeDisposable);

			this._filterItems.CollectionChangedAsObservable().Subscribe(x => {
				this._onUpdateFilteringConditions.OnNext(Unit.Default);
			});
		}

		/// <summary>
		/// フィルター条件に合致しているか判定する
		/// </summary>
		/// <param name="mediaFile">メディアファイルインスタンス</param>
		/// <returns>結果</returns>
		public bool Filter(MediaFileModel mediaFile) {
			return this._filterItems.All(x => x.Condition(mediaFile));
		}

		/// <summary>
		/// タグフィルター追加
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public void AddTagFilter(string tagName) {
			this.States.AlbumStates.FilterItemCreators.Add(
				new TagFilterItemCreator(tagName)
			);
		}

		/// <summary>
		/// ファイルパスフィルター追加
		/// </summary>
		/// <param name="text">ファイルパスに含まれる文字列</param>
		public void AddFilePathFilter(string text) {
			this.States.AlbumStates.FilterItemCreators.Add(
				new FilePathFilterItemCreator(text)
			);
		}

		/// <summary>
		/// 評価フィルター追加
		/// </summary>
		/// <param name="rate">評価</param>
		public void AddRateFilter(int rate) {
			this.States.AlbumStates.FilterItemCreators.Add(
				new RateFilterItemCreator(rate)
			);
		}

		/// <summary>
		/// 解像度フィルター追加
		/// </summary>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		public void AddResolutionFilter(int width, int height) {
			this.States.AlbumStates.FilterItemCreators.Add(
				new ResolutionFilterItemCreator(new ComparableSize(width, height))
			);
		}

		/// <summary>
		/// メディアタイプフィルター追加
		/// </summary>
		/// <param name="type">型</param>
		public void AddMediaTypeFilter(Type type) {
			this.States.AlbumStates.FilterItemCreators.Add(
				new MediaTypeFilterItemCreator(type)
			);
		}

		/// <summary>
		/// フィルター削除
		/// </summary>
		/// <param name="item">削除対象フィルタークリエイター</param>
		public void RemoveFilter(IFilterItemCreator filterItemCreator) {
			this.States.AlbumStates.FilterItemCreators.Remove(filterItemCreator);
		}
	}
}
