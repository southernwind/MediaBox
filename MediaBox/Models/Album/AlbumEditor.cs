using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;
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
		/// コンストラクタ
		/// </summary>
		public AlbumEditor() {
			this._albumContainer = Get.Instance<AlbumContainer>();
			this.AlbumSelector = new AlbumSelector().AddTo(this.CompositeDisposable);
			this.AlbumSelector.SetName("editor");
			this.AlbumBoxId.Subscribe(x => {
				lock (this.Rdb) {
					var currentRecord = this.Rdb.AlbumBoxes.FirstOrDefault(ab => ab.AlbumBoxId == x);
					var result = new List<string>();
					while (currentRecord != null) {
						result.Add(currentRecord.Name);
						currentRecord = currentRecord.Parent;
					}
					result.Reverse();
					this.AlbumBoxTitle.Value = result.ToArray();
				}
			});
		}

		/// <summary>
		/// アルバム新規作成
		/// </summary>
		public void CreateAlbum() {
			this._album = new RegisteredAlbum(this.AlbumSelector).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// アルバム編集
		/// </summary>
		/// <param name="album">編集するアルバム</param>
		public void EditAlbum(RegisteredAlbum album) {
			this._album = album;
		}

		/// <summary>
		/// アルバムを読み込み
		/// </summary>
		public void Load() {
			this.AlbumBoxId.Value = this._album.AlbumBoxId.Value;
			this.Title.Value = this._album.Title.Value;
			this.MonitoringDirectories.Clear();
			this.MonitoringDirectories.AddRange(this._album.Directories);
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
