using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;

namespace SandBeige.MediaBox.Models.Album.Filter {
	/// <summary>
	/// フィルタリング条件
	/// </summary>
	/// <remarks>
	/// Add***Filterメソッドでフィルタークリエイターを<see cref="FilterItemCreators"/>に追加し、
	/// <see cref="RemoveFilter(IFilterItemCreator)"/>メソッドで削除する。
	/// 追加されたフィルタークリエイターはフィルターを生成し、内部に保持する。
	/// </remarks>
	internal class FilteringCondition : ModelBase {
		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
		}

		/// <summary>
		/// フィルター条件
		/// </summary>
		private readonly ReadOnlyReactiveCollection<FilterItem> _filterItems;

		/// <summary>
		/// フィルター条件クリエイター
		/// </summary>
		public ReadOnlyReactiveCollection<IFilterItemCreator> FilterItemCreators {
			get;
		}

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
		/// フィルター保存用オブジェクト
		/// </summary>
		public RestorableFilterObject RestorableFilterObject {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filterObject">復元用フィルターオブジェクト</param>
		public FilteringCondition(RestorableFilterObject filterObject) {
			this.RestorableFilterObject = filterObject;
			this.DisplayName = filterObject.DisplayName.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.FilterItemCreators = this.RestorableFilterObject.FilterItemCreators.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this._filterItems =
				this.FilterItemCreators
					.ToReadOnlyReactiveCollection(x => x.Create() as FilterItem, Scheduler.Immediate)
					.AddTo(this.CompositeDisposable);

			this._filterItems.CollectionChangedAsObservable().Subscribe(x => {
				this._onUpdateFilteringConditions.OnNext(Unit.Default);
			}).AddTo(this.CompositeDisposable);

		}

		/// <summary>
		/// フィルター条件に合致しているか判定する
		/// </summary>
		/// <param name="query">絞り込みクエリを適用するクエリ</param>
		/// <returns>結果</returns>
		public IEnumerable<MediaFile> SetFilterConditions(IQueryable<MediaFile> query) {
			foreach (var filter in this._filterItems.Where(x => x.IncludeSql)) {
				query = query.Where(filter.Condition);
			}

			var mfs = query.AsEnumerable();
			foreach (var filter in this._filterItems.Where(x => !x.IncludeSql)) {
				mfs = mfs.Where(filter.Condition.Compile());
			}
			return mfs;
		}

		/// <summary>
		/// フィルター条件に合致しているか判定する
		/// </summary>
		/// <param name="files">絞り込みを適用するモデルシーケンス</param>
		/// <returns>絞り込み後シーケンス</returns>
		public IEnumerable<IMediaFileModel> SetFilterConditions(IEnumerable<IMediaFileModel> files) {
			foreach (var filter in this._filterItems) {
				files = files.Where(filter.ConditionForModel);
			}
			return files;
		}

		/// <summary>
		/// タグフィルター追加
		/// </summary>
		/// <param name="tagName">タグ名</param>
		/// <param name="searchType">検索タイプ</param>
		public void AddTagFilter(string tagName, SearchTypeInclude searchType) {
			this.RestorableFilterObject.FilterItemCreators.Add(
				new TagFilterItemCreator(tagName, searchType)
			);
		}

		/// <summary>
		/// ファイルパスフィルター追加
		/// </summary>
		/// <param name="text">ファイルパスに含まれる文字列</param>
		/// <param name="searchType">検索タイプ</param>
		public void AddFilePathFilter(string text, SearchTypeInclude searchType) {
			this.RestorableFilterObject.FilterItemCreators.Add(
				new FilePathFilterItemCreator(text, searchType)
			);
		}

		/// <summary>
		/// 評価フィルター追加
		/// </summary>
		/// <param name="rate">評価</param>
		/// <param name="searchType">検索タイプ</param>
		public void AddRateFilter(int rate, SearchTypeComparison searchType) {
			this.RestorableFilterObject.FilterItemCreators.Add(
				new RateFilterItemCreator(rate, searchType)
			);
		}

		/// <summary>
		/// 解像度フィルター追加
		/// </summary>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		/// <param name="searchType">検索タイプ</param>
		public void AddResolutionFilter(int? width, int? height, SearchTypeComparison searchType) {
			IFilterItemCreator filterItemCreator;
			if (width is { } w && height is { } h) {
				filterItemCreator = new ResolutionFilterItemCreator(new ComparableSize(w, h), searchType);
			} else {
				filterItemCreator = new ResolutionFilterItemCreator(width, height, searchType);
			}
			this.RestorableFilterObject.FilterItemCreators.Add(filterItemCreator);
		}

		/// <summary>
		/// メディアタイプフィルター追加
		/// </summary>
		/// <param name="isVideo">動画か否か</param>
		public void AddMediaTypeFilter(bool isVideo) {
			this.RestorableFilterObject.FilterItemCreators.Add(
				new MediaTypeFilterItemCreator(isVideo)
			);
		}

		/// <summary>
		/// 座標フィルター追加
		/// </summary>
		/// <param name="hasLocation">座標情報を含むか否か</param>
		public void AddLocationFilter(bool hasLocation) {
			this.RestorableFilterObject.FilterItemCreators.Add(
				new LocationFilterItemCreator(hasLocation)
			);
		}

		/// <summary>
		///ファイル存在フィルター追加
		/// </summary>
		/// <param name="exists">ファイルが存在するか否か</param>
		public void AddExistsFilter(bool exists) {
			this.RestorableFilterObject.FilterItemCreators.Add(
				new ExistsFilterItemCreator(exists)
			);
		}

		/// <summary>
		/// フィルター削除
		/// </summary>
		/// <param name="filterItemCreator">削除対象フィルタークリエイター</param>
		public void RemoveFilter(IFilterItemCreator filterItemCreator) {
			this.RestorableFilterObject.FilterItemCreators.Remove(filterItemCreator);
		}
	}

	/// <summary>
	/// フィルター設定復元用オブジェクト
	/// </summary>
	public class RestorableFilterObject {
		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// フィルター条件クリエイター
		/// </summary>
		public ReactiveCollection<IFilterItemCreator> FilterItemCreators {
			get;
			set;
		} = new ReactiveCollection<IFilterItemCreator>();
	}
}
