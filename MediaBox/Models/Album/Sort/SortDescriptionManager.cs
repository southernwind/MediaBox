using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// ソート管理
	/// </summary>
	internal class SortDescriptionManager : ModelBase {
		/// <summary>
		/// ソート条件
		/// </summary>
		public ReactiveCollection<SortItem> SortItems {
			get;
		} = new ReactiveCollection<SortItem>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SortDescriptionManager() {
			this.SortItems.AddRange(new[] {
				new SortItem(nameof(MediaFile.FileName), "ファイル名"),
				new SortItem(nameof(MediaFile.FilePath), "ファイルパス"),
				new SortItem(nameof(MediaFile.Date), "日付時刻"),
				new SortItem(nameof(MediaFile.FileSize), "ファイルサイズ"),
				new SortItem(nameof(MediaFile.Latitude), "緯度"),
				new SortItem(nameof(MediaFile.Longitude), "経度")
			});

			// 設定値初回値読み込み
			foreach (var sdp in this.Settings.GeneralSettings.SortDescriptions.Value) {
				var si = this.SortItems.SingleOrDefault(x => x.PropertyName == sdp.PropertyName);
				if (si != null) {
					si.Enabled = true;
					si.Direction = sdp.Direction;
				}
			}
			var collectionView = CollectionViewSource.GetDefaultView(this.SortItems);

			// 更新
			this.SortItems.ObserveElementProperty(x => x.Enabled).ToUnit()
				.Merge(this.SortItems.ObserveElementProperty(x => x.Direction).Where(x => x.Instance.Enabled).ToUnit())
				.Subscribe(x => {
					collectionView.Refresh();
					this.Settings.GeneralSettings.SortDescriptions.Value =
						this.SortItems
							.Where(si => si.Enabled)
							.OrderBy(si => si.UpdateTime)
							.Select(si => new SortDescriptionParams(si.PropertyName, si.Direction))
							.ToArray();
				});

			collectionView.SortDescriptions.Clear();
			collectionView.SortDescriptions.Add(new SortDescription(nameof(SortItem.Enabled), ListSortDirection.Descending));
			collectionView.SortDescriptions.Add(new SortDescription(nameof(SortItem.UpdateTime), ListSortDirection.Ascending));
		}
	}
}
