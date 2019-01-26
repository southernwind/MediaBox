using System;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.ViewModels.Media;

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
				new SortItem(nameof(IMediaFileViewModel.FileName), "ファイル名"),
				new SortItem(nameof(IMediaFileViewModel.FilePath), "ファイルパス"),
				new SortItem(nameof(IMediaFileViewModel.CreationTime), "作成日時"),
				new SortItem(nameof(IMediaFileViewModel.ModifiedTime), "編集日時"),
				new SortItem(nameof(IMediaFileViewModel.LastAccessTime), "最終アクセス日時"),
				new SortItem(nameof(IMediaFileViewModel.FileSize), "ファイルサイズ"),
				new SortItem(nameof(IMediaFileViewModel.Latitude), "緯度"),
				new SortItem(nameof(IMediaFileViewModel.Longitude), "経度"),
				new SortItem(nameof(IMediaFileViewModel.Rate), "評価")
			});

			// 設定値初回値読み込み
			foreach (var sdp in this.Settings.GeneralSettings.SortDescriptions.Value) {
				var si = this.SortItems.SingleOrDefault(x => x.PropertyName == sdp.PropertyName);
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
							.Select(si => new SortDescriptionParams(si.PropertyName, si.Direction))
							.ToArray();
				});
		}
	}
}
