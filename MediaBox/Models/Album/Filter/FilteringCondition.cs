using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Xaml;
using System.Xml;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;

namespace SandBeige.MediaBox.Models.Album.Filter {
	/// <summary>
	/// フィルタリング条件
	/// </summary>
	/// <remarks>
	/// Add***Filterメソッドでフィルタークリエイターを<see cref="FilterItemCreators"/>に追加し、
	/// <see cref="RemoveFilter(IFilterItemCreator)"/>メソッドで削除する。
	/// 追加されたフィルタークリエイターはフィルターを生成し、内部に保持する。
	/// フィルター条件に合致しているかの判断には<see cref="Filter(IMediaFileModel)"/>を使う。
	/// </remarks>
	internal class FilteringCondition : ModelBase {
		/// <summary>
		/// フィルターID
		/// </summary>
		public int FilterId {
			get;
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// フィルター条件
		/// </summary>
		private readonly ReadOnlyReactiveCollection<FilterItem> _filterItems;

		/// <summary>
		/// フィルター条件クリエイター
		/// </summary>
		public ReactiveCollection<IFilterItemCreator> FilterItemCreators {
			get;
		} = new ReactiveCollection<IFilterItemCreator>();

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
		/// コンストラクタ
		/// </summary>
		public FilteringCondition(int filterId) {
			this._filterItems =
				this.FilterItemCreators
					.ToReadOnlyReactiveCollection(x => x.Create() as FilterItem, Scheduler.Immediate)
					.AddTo(this.CompositeDisposable);

			this._filterItems.CollectionChangedAsObservable().Subscribe(x => {
				this._onUpdateFilteringConditions.OnNext(Unit.Default);
			}).AddTo(this.CompositeDisposable);

			this.FilterItemCreators
				.ToCollectionChanged()
				.ToUnit()
				.Merge(this.DisplayName.ToUnit())
				.Throttle(TimeSpan.FromSeconds(1))
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Subscribe(_ => {
					this.Save();
				});

			this.FilterId = filterId;
		}

		/// <summary>
		/// フィルター読み込み
		/// </summary>
		public void Load() {
			var filePath = Path.Combine(this.Settings.PathSettings.FilterDirectoryPath.Value, this.FilterId.ToString());
			if (File.Exists(filePath)) {
				try {
					if (XamlServices.Load(filePath) is RestorableFilterObject rfa) {
						this.DisplayName.Value = rfa.DisplayName;
						this.FilterItemCreators.AddRange(rfa.FilterItemCreators);
					}
				} catch (XmlException ex) {
					this.Logging.Log("フィルターファイル読み込み失敗", LogLevel.Warning, ex);
				}
			}
		}

		/// <summary>
		/// フィルター保存
		/// </summary>
		public void Save() {
			var filePath = Path.Combine(this.Settings.PathSettings.FilterDirectoryPath.Value, this.FilterId.ToString());
			try {
				XamlServices.Save(filePath, new RestorableFilterObject() {
					DisplayName = this.DisplayName.Value,
					FilterItemCreators = this.FilterItemCreators
				});
			} catch (IOException ex) {
				this.Logging.Log("フィルターファイル保存失敗", LogLevel.Warning, ex);
			}
		}

		/// <summary>
		/// フィルター条件に合致しているか判定する
		/// </summary>
		/// <param name="mediaFile">メディアファイルインスタンス</param>
		/// <returns>結果</returns>
		public IQueryable<MediaFile> SetFilterConditions(IQueryable<MediaFile> query) {
			foreach (var filter in this._filterItems) {
				query = query.Where(filter.Condition);
			}
			return query;
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
		/// 評価フィルター追加
		/// </summary>
		/// <param name="rate">評価</param>
		public void AddRateFilter(int rate) {
			this.FilterItemCreators.Add(
				new RateFilterItemCreator(rate)
			);
		}

		/// <summary>
		/// 解像度フィルター追加
		/// </summary>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		public void AddResolutionFilter(int width, int height) {
			this.FilterItemCreators.Add(
				new ResolutionFilterItemCreator(new ComparableSize(width, height))
			);
		}

		/// <summary>
		/// メディアタイプフィルター追加
		/// </summary>
		/// <param name="type">型</param>
		public void AddMediaTypeFilter(bool isVideo) {
			this.FilterItemCreators.Add(
				new MediaTypeFilterItemCreator(isVideo)
			);
		}

		/// <summary>
		/// フィルター削除
		/// </summary>
		/// <param name="filterItemCreator">削除対象フィルタークリエイター</param>
		public void RemoveFilter(IFilterItemCreator filterItemCreator) {
			this.FilterItemCreators.Remove(filterItemCreator);
		}
	}

	/// <summary>
	/// フィルター設定復元用オブジェクト
	/// </summary>
	public class RestorableFilterObject {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get;
			set;
		}
		/// <summary>
		/// フィルター条件クリエイター
		/// </summary>
		public IEnumerable<IFilterItemCreator> FilterItemCreators {
			get;
			set;
		}
	}
}
