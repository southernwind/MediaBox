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
		public ReactivePropertySlim<double> Latitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// 経度
		/// </summary>
		public ReactivePropertySlim<double> Longitude {
			get;
		} = new ReactivePropertySlim<double>();

		public ReadOnlyReactivePropertySlim<MediaGroupViewModel> TargetFiles {
			get;
		}
		
		public ReactiveCommand SetGpsCommand {
			get;
		} = new ReactiveCommand();

		public ReactiveProperty<MapViewModel> Map{
			get;
		}

		public GpsSelectorViewModel(GpsSelector model) {
			this._model = model;
			this.SetGpsCommand.Subscribe(this._model.SetGps);
			this.TargetFiles = this._model.TargetFiles.Where(x => x != null).Select(x => Get.Instance<MediaGroupViewModel>(x)).ToReadOnlyReactivePropertySlim();
		}
	}
}
