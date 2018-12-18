using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Map {
	internal class GpsSelectorViewModel : ViewModelBase {
		private readonly GpsSelector _model;

		/// <summary>
		/// 緯度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double> Latitude {
			get;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double> Longitude {
			get;
		}

		public ReactiveCollection<MediaFileViewModel> TargetFiles {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();

		/// <summary>
		/// GPS設定対象候補一覧
		/// </summary>
		public ReadOnlyReactiveCollection<MediaFileViewModel> CandidateMediaFiles {
			get;
		}

		public ReadOnlyReactivePropertySlim<MapViewModel> Map{
			get;
		}

		public GpsSelectorViewModel(GpsSelector model) {
			this._model = model;
			this.Latitude = this._model.Latitude.ToReadOnlyReactivePropertySlim();
			this.Longitude = this._model.Longitude.ToReadOnlyReactivePropertySlim();
			this.CandidateMediaFiles = this._model.CandidateMediaFiles.ToReadOnlyReactiveCollection(x => Get.Instance<MediaFileViewModel>(x));
			this.Map = this._model.Map.Select(x => Get.Instance<MapViewModel>(x)).ToReadOnlyReactivePropertySlim();
			this.TargetFiles.SynchronizeTo(this._model.TargetFiles, x => x.Model);
		}

		public void SetCandidateMediaFiles(IEnumerable<MediaFileViewModel> mediaFileViewModels) {
			this._model.CandidateMediaFiles.AddRange(mediaFileViewModels.Select(x => x.Model));
		}
	}
}
