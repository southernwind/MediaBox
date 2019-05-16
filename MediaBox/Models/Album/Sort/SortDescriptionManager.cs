using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// ソートマネージャー
	/// </summary>
	/// <remarks>
	/// ソート条件(<see cref="SortItems"/>)の<see cref="SortItem.Enabled"/>と<see cref="SortItem.Direction"/>プロパティが変化するたびに
	/// <see cref="Settings.GeneralSettings.SortDescriptions"/>を更新する。
	/// </remarks>
	internal class SortDescriptionManager : ModelBase {
		/// <summary>
		/// ソート条件候補
		/// </summary>
		public ReactiveCollection<ISortItem> SortItems {
			get;
		} = new ReactiveCollection<ISortItem>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SortDescriptionManager() {
			this.SortItems.AddRange(new ISortItem[] {
				new SortItem<string>(nameof(IMediaFileModel.FileName),x =>x.FileName, "ファイル名"),
				new SortItem<string>(nameof(IMediaFileModel.FilePath),x=>x.FilePath, "ファイルパス"),
				new SortItem<DateTime>(nameof(IMediaFileModel.CreationTime),x=>x.CreationTime, "作成日時"),
				new SortItem<DateTime>(nameof(IMediaFileModel.ModifiedTime),x=>x.ModifiedTime, "編集日時"),
				new SortItem<DateTime>(nameof(IMediaFileModel.LastAccessTime),x=>x.LastAccessTime, "最終アクセス日時"),
				new SortItem<long>(nameof(IMediaFileModel.FileSize),x=>x.FileSize, "ファイルサイズ"),
				new SortItem<GpsLocation>(nameof(IMediaFileModel.Location),x=>x.Location, "座標"),
				new SortItem<int>(nameof(IMediaFileModel.Rate),x=>x.Rate, "評価"),
				new SortItem<ComparableSize?>(nameof(IMediaFileModel.Resolution),x=>x.Resolution, "解像度")
			});

			// 設定値初回値読み込み
			foreach (var sdp in this.Settings.GeneralSettings.SortDescriptions.Value) {
				var si = this.SortItems.SingleOrDefault(x => x.Key == sdp.PropertyName);
				if (si != null) {
					si.Enabled = true;
					si.Direction = sdp.Direction;
				}
			}
			// 更新
			this.SortItems.ObserveElementProperty(x => x.Enabled).ToUnit()
				.Merge(this.SortItems.ObserveElementProperty(x => x.Direction).Where(x => x.Instance.Enabled).ToUnit())
				.Subscribe(x => {
					this.Settings.GeneralSettings.SortDescriptions.Value =
						this.SortItems
							.Where(si => si.Enabled)
							.OrderBy(si => si.UpdateTime)
							.Select(si => new SortDescriptionParams(si.Key, si.Direction))
							.ToArray();
				});
		}

		/// <summary>
		/// ソート条件適用
		/// </summary>
		/// <param name="array">ソート対象の配列</param>
		/// <returns>ソート済み配列</returns>
		public IEnumerable<IMediaFileModel> SetSortConditions(IEnumerable<IMediaFileModel> array) {
			foreach (var si in this.SortItems.Where(si => si.Enabled).OrderBy(si => si.UpdateTime)) {
				switch (si) {
					case SortItem<string> sis:
						array = this.Order(array, sis.KeySelector, si.Direction);
						break;
					case SortItem<DateTime> sid:
						array = this.Order(array, sid.KeySelector, si.Direction);
						break;
					case SortItem<long> sil:
						array = this.Order(array, sil.KeySelector, si.Direction);
						break;
					case SortItem<GpsLocation> sig:
						array = this.Order(array, sig.KeySelector, si.Direction);
						break;
					case SortItem<int> sii:
						array = this.Order(array, sii.KeySelector, si.Direction);
						break;
					case SortItem<ComparableSize?> sic:
						array = this.Order(array, sic.KeySelector, si.Direction);
						break;
					default:
						throw new FormatException();
				}
			}
			return array;
		}

		/// <summary>
		/// ソートの適用をする
		/// </summary>
		/// <typeparam name="TKey">ソートキーの型</typeparam>
		/// <param name="array">ソート対象の配列</param>
		/// <param name="keySelector">ソートキー</param>
		/// <param name="direction">ソート方向</param>
		/// <returns>ソート済み配列</returns>
		private IOrderedEnumerable<IMediaFileModel> Order<TKey>(
			IEnumerable<IMediaFileModel> array,
			Func<IMediaFileModel, TKey> keySelector,
			ListSortDirection direction) {
			if (array is IOrderedEnumerable<IMediaFileModel> ordered) {
				if (direction == ListSortDirection.Ascending) {
					return ordered.ThenBy(keySelector);
				} else {
					return ordered.ThenByDescending(keySelector);
				}
			} else {
				if (direction == ListSortDirection.Ascending) {
					return array.OrderBy(keySelector);
				} else {
					return array.OrderByDescending(keySelector);
				}
			}
		}
	}
}
