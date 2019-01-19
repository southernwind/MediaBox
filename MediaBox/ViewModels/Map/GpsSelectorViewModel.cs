using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Map {
	internal class GpsSelectorViewModel : ViewModelBase {
		private readonly GpsSelector _model;

		/// <summary>
		/// 緯度
		/// </summary>
		public IReadOnlyReactiveProperty<double> Latitude {
			get;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public IReadOnlyReactiveProperty<double> Longitude {
			get;
		}

		/// <summary>
		/// 処理対象ファイル
		/// </summary>
		public IReactiveProperty<IEnumerable<MediaFileViewModel>> TargetFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<MediaFileViewModel>>(Array.Empty<MediaFileViewModel>());

		/// <summary>
		/// GPS設定対象候補一覧
		/// </summary>
		public ReadOnlyReactiveCollection<MediaFileViewModel> CandidateMediaFiles {
			get;
		}

		/// <summary>
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapViewModel> Map {
			get;
		}

		public GpsSelectorViewModel(GpsSelector model) {
			this._model = model;
			this.Latitude = this._model.Latitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Longitude = this._model.Longitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.CandidateMediaFiles =
				this._model
					.CandidateMediaFiles
					.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create, disposeElement: false)
					.AddTo(this.CompositeDisposable);
			this.Map =
				this._model
					.Map
					.Select(this.ViewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			// 処理対象ファイル ViewModel⇔Model間双方向同期
			this.TargetFiles.TwoWaySynchronize(
				this._model.TargetFiles,
				x => x.Select(vm => vm.Model),
				x => x.Select(this.ViewModelFactory.Create))
				.AddTo(this.CompositeDisposable);

			// モデル破棄時にこのインスタンスも破棄
			this.AddTo(this._model.CompositeDisposable);
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
