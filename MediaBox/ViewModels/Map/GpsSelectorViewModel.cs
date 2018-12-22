using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Library.Extensions;
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

		/// <summary>
		/// 処理対象ファイル
		/// </summary>
		public ReactiveCollection<MediaFileViewModel> TargetFiles {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();

		/// <summary>
		/// GPS設定対象候補一覧
		/// </summary>
		public ReadOnlyReactiveCollection<MediaFileViewModel> CandidateMediaFiles {
			get;
		}

		/// <summary>
		/// マップ
		/// </summary>
		public ReadOnlyReactivePropertySlim<MapViewModel> Map{
			get;
		}

		public GpsSelectorViewModel(GpsSelector model) {
			this._model = model;
			this.Latitude = this._model.Latitude.ToReadOnlyReactivePropertySlim();
			this.Longitude = this._model.Longitude.ToReadOnlyReactivePropertySlim();
			this.CandidateMediaFiles = this._model.CandidateMediaFiles.ToReadOnlyReactiveCollection(x => Get.Instance<MediaFileViewModel>(x));
			this.Map = this._model.Map.Select(x => Get.Instance<MapViewModel>(x)).ToReadOnlyReactivePropertySlim();
			
			// 処理対象ファイル ViewModel→Model同期
			this.TargetFiles.SynchronizeTo(this._model.TargetFiles, x => x.Model);
		}

		/// <summary>
		/// GPS設定対象ファイル注入用
		/// </summary>
		/// <param name="mediaFileViewModels">対象ファイルリスト</param>
		public void SetCandidateMediaFiles(IEnumerable<MediaFileViewModel> mediaFileViewModels) {
			this._model.CandidateMediaFiles.AddRange(mediaFileViewModels.Select(x => x.Model));
		}
	}
}
