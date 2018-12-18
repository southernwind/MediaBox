using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
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

		public GpsSelector() {
			this.CandidateMediaFiles
				.ToCollectionChanged()
				.Subscribe(x => {
					if (x.Action == NotifyCollectionChangedAction.Add) {
						this.Map.Value.Items.Add(x.Value);
					} else if (x.Action == NotifyCollectionChangedAction.Remove) {
						this.Map.Value.Items.Remove(x.Value);
					}
				});

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

			this.TargetFiles.SynchronizeTo(this.Map.Value.IgnoreMediaFiles);

			this.Latitude.Subscribe(x => {
				this.Map.Value.PointerLatitude.Value = x;
			});
			this.Longitude.Subscribe(x => {
				this.Map.Value.PointerLongitude.Value = x;
			});

			var map = this.Map.Value.MapControl.Value;
			map.MouseMove += (sender, e) => {
				var vp = map.ViewportPointToLocation(e.GetPosition(map));
				this.Latitude.Value = vp.Latitude;
				this.Longitude.Value = vp.Longitude;
			};

			map.MouseDoubleClick += (sender, e) => {
				this.SetGps();
				e.Handled = true;
			};
		}

		public void SetGps() {
			foreach (var item in this.TargetFiles) {
				item.SetGps(this.Latitude.Value, this.Longitude.Value);
			}
			this.TargetFiles.Clear();
		}
	}
}
