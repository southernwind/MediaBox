using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.ViewModels.Map {
	/// <summary>
	/// GPS選択ViewModel
	/// </summary>
	internal class GpsSelectorViewModel : ViewModelBase {
		/// <summary>
		/// モデル
		/// </summary>
		private readonly GpsSelector _model;

		/// <summary>
		/// 座標
		/// </summary>
		public IReadOnlyReactiveProperty<GpsLocation> Location {
			get;
		}

		/// <summary>
		/// 処理対象ファイル
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileViewModel>> TargetFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileViewModel>>(Array.Empty<IMediaFileViewModel>());

		/// <summary>
		/// GPS設定対象候補リスト
		/// </summary>
		public ReadOnlyReactiveCollection<IMediaFileViewModel> CandidateMediaFiles {
			get;
		}

		/// <summary>
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapViewModel> Map {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public GpsSelectorViewModel(GpsSelector model) {
			this._model = model;
			this.Location = this._model.Location.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
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
		public void SetCandidateMediaFiles(IEnumerable<IMediaFileViewModel> mediaFileViewModels) {
			this._model.CandidateMediaFiles.AddRange(mediaFileViewModels.Select(x => x.Model));
		}
	}
}
