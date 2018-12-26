using System;
using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Map {
	internal class GpsSelector : ModelBase {
		/// <summary>
		/// 緯度
		/// </summary>
		public ReactivePropertySlim<double> Latitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// 経度
		/// </summary>
		public ReactivePropertySlim<double> Longitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// GPS設定対象候補一覧
		/// </summary>
		public ReactiveCollection<MediaFile> CandidateMediaFiles {
			get;
		} = new ReactiveCollection<MediaFile>();

		/// <summary>
		/// GPS設定対象ファイル一覧
		/// </summary>
		public ReactiveCollection<MediaFile> TargetFiles {
			get;
		} = new ReactiveCollection<MediaFile>();

		/// <summary>
		/// マップモデル
		/// </summary>
		public ReactivePropertySlim<MapModel> Map {
			get;
		} = new ReactivePropertySlim<MapModel>(Get.Instance<MapModel>());

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GpsSelector() {
			// 設定候補一覧→マップモデルアイテム片方向同期
			this.CandidateMediaFiles.SynchronizeTo(this.Map.Value.Items);

			// 設定対象アイテム→マップポインター
			this.TargetFiles
				.ToCollectionChanged()
				.Subscribe(x => {
					if (this.TargetFiles.Count == 0) {
						this.Map.Value.Pointer.Value = null;
						return;
					}
					var mg = Get.Instance<MediaGroup>(this.TargetFiles.First(), default(Rectangle));
					foreach (var item in this.TargetFiles.Skip(1)) {
						mg.Items.Add(item);
					}

					this.Map.Value.Pointer.Value = mg;
				});

			// 設定対象アイテム→マップ無視ファイル
			this.TargetFiles.SynchronizeTo(this.Map.Value.IgnoreMediaFiles);

			// 緯度→ポインタ座標片方向同期
			this.Latitude.Subscribe(x => {
				this.Map.Value.PointerLatitude.Value = x;
			});

			// 経度→ポインタ座標片方向同期
			this.Longitude.Subscribe(x => {
				this.Map.Value.PointerLongitude.Value = x;
			});

			// マップイベント
			var map = this.Map.Value.MapControl.Value;
			// マウス移動
			map.MouseMove += (sender, e) => {
				var vp = map.ViewportPointToLocation(e.GetPosition(map));
				this.Latitude.Value = vp.Latitude;
				this.Longitude.Value = vp.Longitude;
			};

			// マウスダブルクリック
			map.MouseDoubleClick += (sender, e) => {
				this.SetGps();
				e.Handled = true;
			};
		}

		/// <summary>
		/// GPS設定
		/// </summary>
		public void SetGps() {
			foreach (var item in this.TargetFiles) {
				item.SetGps(this.Latitude.Value, this.Longitude.Value);
			}
			this.TargetFiles.Clear();
		}
	}
}
