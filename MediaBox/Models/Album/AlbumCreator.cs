using System.Collections.Generic;
using System.Linq;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	internal class AlbumCreator : ModelBase {
		private readonly AlbumContainer _albumContainer;

		/// <summary>
		/// 作成/編集するアルバム
		/// </summary>
		private RegisteredAlbum _album;

		/// <summary>
		/// タイトル
		/// </summary>
		public ReactiveProperty<string> Title {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// 監視ディレクトリ
		/// </summary>
		public ReactiveCollection<string> MonitoringDirectories {
			get;
		} = new ReactiveCollection<string>();

		/// <summary>
		/// パス
		/// </summary>
		public ReactiveProperty<string> AlbumPath {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// ファイルリスト
		/// </summary>
		public ReactiveCollection<MediaFile> Items {
			get;
		} = new ReactiveCollection<MediaFile>();

		public AlbumCreator() {
			this._albumContainer = Get.Instance<AlbumContainer>();
		}

		/// <summary>
		/// アルバム新規作成
		/// </summary>
		public void CreateAlbum() {
			this._album = Get.Instance<RegisteredAlbum>();
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
		public void AddFiles(IEnumerable<MediaFile> mediaFiles) {
			this.Items.AddRange(mediaFiles);
		}

		/// <summary>
		/// ファイル削除
		/// </summary>
		/// <param name="mediaFiles"></param>
		public void RemoveFiles(IEnumerable<MediaFile> mediaFiles) {
			this.Items.RemoveRange(mediaFiles);
		}

		/// <summary>
		/// アルバムを読み込み
		/// </summary>
		public void Load() {
			this.Title.Value = this._album.Title.Value;
			this.AlbumPath.Value = this._album.AlbumPath.Value;
			this.MonitoringDirectories.Clear();
			this.MonitoringDirectories.AddRange(this._album.MonitoringDirectories);
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
			this._album.Title.Value = this.Title.Value;
			this._album.AlbumPath.Value = this.AlbumPath.Value;
			this._album.MonitoringDirectories.RemoveRange(this._album.MonitoringDirectories.Except(this.MonitoringDirectories));
			this._album.MonitoringDirectories.AddRange(this.MonitoringDirectories.Except(this._album.MonitoringDirectories));

			this._album.ReflectToDataBase();

			this._album.RemoveFiles(this._album.Items.Except(this.Items));
			this._album.AddFiles(this.Items.Except(this._album.Items));

			// 作成していた場合はコンテナに追加する
			if (createFlag) {
				this._albumContainer.AddAlbum(this._album);
			}
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
	}
}
