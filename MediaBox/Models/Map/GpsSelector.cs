using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
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

		public ReactiveCollection<MediaFile> SelectedMediaFiles {
			get;
		} = new ReactiveCollection<MediaFile>();

		public ReactivePropertySlim<MediaGroup> TargetFiles {
			get;
		} = new ReactivePropertySlim<MediaGroup>();

		public ReactivePropertySlim<MapModel> Map {
			get;
		} = new ReactivePropertySlim<MapModel>(Get.Instance<MapModel>());

		public GpsSelector() {
			this.SelectedMediaFiles
				.ToCollectionChanged()
				.Subscribe(_ => {
					if (this.SelectedMediaFiles.Count == 0) {
						return;
					}
					this.TargetFiles.Value = Get.Instance<MediaGroup>(
						this.SelectedMediaFiles.First(), default(Rectangle));
					foreach (var item in this.SelectedMediaFiles.Skip(1)) {
						this.TargetFiles.Value.Items.Add(item);
					}
				});
		}

		public void SetGps() {
			foreach (var item in this.TargetFiles.Value.Items) {
				item.SetGps(this.Latitude.Value, this.Longitude.Value);
			}
			this.TargetFiles.Value = null;
		}
	}
}
