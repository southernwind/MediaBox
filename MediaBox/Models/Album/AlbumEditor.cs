using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Gesture;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムクリエイター
	/// </summary>
	/// <remarks>
	/// <see cref="RegisteredAlbum"/>の作成または更新を行う。
	/// <see cref="Load"/>でこのクラスのプロパティに値を読み込み、<see cref="Save"/>で保存する。
	/// </remarks>
	internal class AlbumEditor : ModelBase {
		private readonly AlbumContainer _albumContainer;
		public AlbumSelector AlbumSelector {
			get;
		}

		/// <summary>
		/// 作成/編集するアルバム
		/// </summary>
		private RegisteredAlbum _album;

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReactiveProperty<int?> AlbumBoxId {
			get;
		} = new ReactiveProperty<int?>();

		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReactiveProperty<string[]> AlbumBoxTitle {
			get;
		} = new ReactivePropertySlim<string[]>();

		/// <summary>
		/// タイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactiveProperty<string>("");

		/// <summary>
		/// 監視ディレクトリ
		/// </summary>
		public ReactiveCollection<string> MonitoringDirectories {
			get;
		} = new ReactiveCollection<string>();

		/// <summary>
		/// ファイルリスト
		/// </summary>
		public ReactiveCollection<IMediaFileModel> Items {
			get;
		} = new ReactiveCollection<IMediaFileModel>();

		/// <summary>
		/// 候補一覧ズームレベル
		/// </summary>
		public IReadOnlyReactiveProperty<int> CandidateZoomLevel {
			get;
		}

		/// <summary>
		/// 候補操作受信
		/// </summary>
		public GestureReceiver CandidateGestureReceiver {
			get;
		}


		/// <summary>
		/// 一覧ズームレベル
		/// </summary>
		public IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		/// <summary>
		/// 操作受信
		/// </summary>
		public GestureReceiver GestureReceiver {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumEditor() {
			this._albumContainer = Get.Instance<AlbumContainer>();
			this.CandidateGestureReceiver = Get.Instance<GestureReceiver>();
			this.GestureReceiver = Get.Instance<GestureReceiver>();
			this.AlbumSelector = Get.Instance<AlbumSelector>("editor").AddTo(this.CompositeDisposable);

			this.AlbumBoxId.Subscribe(x => {
				lock (this.DataBase) {
					var currentRecord = this.DataBase.AlbumBoxes.FirstOrDefault(ab => ab.AlbumBoxId == x);
					var result = new List<string>();
					while (currentRecord != null) {
						result.Add(currentRecord.Name);
						currentRecord = currentRecord.Parent;
					}
					result.Reverse();
					this.AlbumBoxTitle.Value = result.ToArray();
				}
			});

			this.ZoomLevel = this.GestureReceiver
				.MouseWheelEvent
				.Where(_ => this.GestureReceiver.IsControlKeyPressed)
				.ToZoomLevel()
				.AddTo(this.CompositeDisposable);

			this.CandidateZoomLevel = this.CandidateGestureReceiver
				.MouseWheelEvent
				.Where(_ => this.CandidateGestureReceiver.IsControlKeyPressed)
				.ToZoomLevel()
				.AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// アルバム新規作成
		/// </summary>
		public void CreateAlbum() {
			this._album = Get.Instance<RegisteredAlbum>(this.AlbumSelector).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// アルバム編集
		/// </summary>
		/// <param name="album">編集するアルバム</param>
		public void EditAlbum(RegisteredAlbum album) {
			this._album = album;
		}

		/// <summary>
		/// ファイル追加
		/// </summary>
		/// <param name="mediaFiles"></param>
		public void AddFiles(IEnumerable<IMediaFileModel> mediaFiles) {
			this.Items.AddRange(mediaFiles);
		}

		/// <summary>
		/// ファイル削除
		/// </summary>
		/// <param name="mediaFiles"></param>
		public void RemoveFiles(IEnumerable<IMediaFileModel> mediaFiles) {
			this.Items.RemoveRange(mediaFiles);
		}

		/// <summary>
		/// アルバムを読み込み
		/// </summary>
		public void Load() {
			this.AlbumBoxId.Value = this._album.AlbumBoxId.Value;
			this.Title.Value = this._album.Title.Value;
			this.MonitoringDirectories.Clear();
			this.MonitoringDirectories.AddRange(this._album.Directories);
			this.Items.Clear();
			this.Items.AddRange(this._album.Items);
		}

		/// <summary>
		/// アルバムへ保存
		/// </summary>
		public void Save() {
			// TODO : この判定は如何なものか
			// 未登録のアルバムであれば登録してから保存する
			var createFlag = false;
			if (this._album.AlbumId.Value == default) {
				this._album.Create();
				createFlag = true;
			}
			this._album.AlbumBoxId.Value = this.AlbumBoxId.Value;
			this._album.Title.Value = this.Title.Value;
			this._album.Directories.RemoveRange(this._album.Directories.Except(this.MonitoringDirectories));
			this._album.Directories.AddRange(this.MonitoringDirectories.Except(this._album.Directories));

			this._album.ReflectToDataBase();

			this._album.RemoveFiles(this._album.Items.Except(this.Items));
			this._album.AddFiles(this.Items.Except(this._album.Items));

			// 作成していた場合はコンテナに追加する
			if (createFlag) {
				this._albumContainer.AddAlbum(this._album.AlbumId.Value);
			}
			this._albumContainer.OnAlbumUpdated(this._album.AlbumId.Value);
		}

		/// <summary>
		/// 監視ディレクトリ追加
		/// </summary>
		/// <param name="path">追加するディレクトリパス</param>
		public void AddDirectory(string path) {
			if (this.MonitoringDirectories.Contains(path)) {
				return;
			}
			this.MonitoringDirectories.Add(path);
		}

		/// <summary>
		/// 監視ディレクトリ削除
		/// </summary>
		/// <param name="path">削除するディレクトリパス</param>
		public void RemoveDirectory(string path) {
			this.MonitoringDirectories.Remove(path);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
