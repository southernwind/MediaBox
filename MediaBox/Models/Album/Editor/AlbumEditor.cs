using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.Editor {
	/// <summary>
	/// アルバムクリエイター
	/// </summary>
	/// <remarks>
	/// <see cref="RegisteredAlbum"/>の作成または更新を行う。
	/// <see cref="Load"/>でこのクラスのプロパティに値を読み込み、<see cref="Save"/>で保存する。
	/// </remarks>
	public class AlbumEditor : ModelBase {
		private readonly IAlbumContainer _albumContainer;

		public IAlbumSelector AlbumSelector {
			get;
		}

		/// <summary>
		/// 作成/編集するアルバム
		/// </summary>
		private readonly AlbumForEditorModel _albumForEditorModel;

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
		public AlbumEditor(
			IAlbumSelectorProvider albumSelectorProvider,
			IMediaBoxDbContext rdb,
			AlbumForEditorModel albumForEditorModel) {
			this._albumForEditorModel = albumForEditorModel;
			var albumSelector = albumSelectorProvider.Create("editor");
			this.AlbumSelector = albumSelector.AddTo(this.CompositeDisposable);
			this.AlbumBoxId.Subscribe(x => {
				lock (rdb) {
					var currentRecord = rdb.AlbumBoxes.FirstOrDefault(ab => ab.AlbumBoxId == x);
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

		}

		/// <summary>
		/// アルバム編集
		/// </summary>
		/// <param name="albumObject">編集するアルバム</param>
		public void EditAlbum(RegisteredAlbumObject albumObject) {
			this._albumForEditorModel.SetAlbumObject(albumObject);
		}

		/// <summary>
		/// アルバムを読み込み
		/// </summary>
		public void Load() {
			this.AlbumBoxId.Value = this._albumForEditorModel.AlbumBoxId.Value;
			this.Title.Value = this._albumForEditorModel.Title.Value;
			this.MonitoringDirectories.Clear();
			this.MonitoringDirectories.AddRange(this._albumForEditorModel.Directories);
		}

		/// <summary>
		/// アルバムへ保存
		/// </summary>
		public void Save() {
			// TODO : この判定は如何なものか
			// 未登録のアルバムであれば登録してから保存する
			var createFlag = false;
			if (this._albumForEditorModel.AlbumId.Value == default) {
				this._albumForEditorModel.Create();
				createFlag = true;
			}
			this._albumForEditorModel.AlbumBoxId.Value = this.AlbumBoxId.Value;
			this._albumForEditorModel.Title.Value = this.Title.Value;
			this._albumForEditorModel.Directories.RemoveRange(this._albumForEditorModel.Directories.Except(this.MonitoringDirectories));
			this._albumForEditorModel.Directories.AddRange(this.MonitoringDirectories.Except(this._albumForEditorModel.Directories));

			this._albumForEditorModel.ReflectToDataBase();

			// 作成していた場合はコンテナに追加する
			if (createFlag) {
				this._albumContainer.AddAlbum(this._albumForEditorModel.AlbumId.Value);
			}
			this._albumContainer.OnAlbumUpdated(this._albumForEditorModel.AlbumId.Value);
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
