using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Editor;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Gesture;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Album.Editor {
	public class AlbumForEditorModel : ModelBase, IAlbumForEditorModel {
		private readonly IMediaBoxDbContext _rdb;
		private IAlbumLoader? _albumLoader;
		private readonly IAlbumLoaderFactory _albumLoaderFactory;
		/// <summary>
		/// アルバムID
		/// (subscribe時初期値配信なし)
		/// </summary>
		public IReactiveProperty<int> AlbumId {
			get;
		} = new ReactiveProperty<int>(mode: ReactivePropertyMode.DistinctUntilChanged);

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReactiveProperty<int?> AlbumBoxId {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// 読み込み対象ディレクトリ
		/// </summary>
		public ReactiveCollection<string> Directories {
			get;
		} = new ReactiveCollection<string>();

		public ReactiveCollection<IMediaFileModel> Items {
			get;
		} = new();

		/// <summary>
		/// カレントのメディアファイル(単一)
		/// </summary>
		public IReactiveProperty<IMediaFileModel?> CurrentMediaFile {
			get;
		} = new ReactivePropertySlim<IMediaFileModel?>();

		/// <summary>
		/// カレントのメディアファイル(複数)
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> CurrentMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());

		/// <summary>
		/// 一覧ズームレベル
		/// </summary>
		public IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		/// <summary>
		/// 操作受信
		/// </summary>
		public IGestureReceiver GestureReceiver {
			get;
		}

		public AlbumForEditorModel(IMediaBoxDbContext rdb, IAlbumLoaderFactory albumLoaderFactory, ISettings settings, IGestureReceiver gestureReceiver) {
			this._rdb = rdb;
			this.GestureReceiver = gestureReceiver;
			this._albumLoaderFactory = albumLoaderFactory;
			this.ZoomLevel = this.GestureReceiver
				.MouseWheelEvent
				.Where(_ => this.GestureReceiver.IsControlKeyPressed)
				.ToZoomLevel(settings.GeneralSettings.ZoomLevel)
				.AddTo(this.CompositeDisposable);

			this.CurrentMediaFiles.Select(x => x.ToArray()).Subscribe(x => {
				// 代表ファイルの設定
				this.CurrentMediaFile.Value = x.FirstOrDefault();
			}).AddTo(this.CompositeDisposable);
		}

		public void SetAlbumObject(IEditableAlbumObject editableAlbumObject, IFilterDescriptionManager filterSetter, ISortDescriptionManager sortSetter) {
			this._albumLoader = this._albumLoaderFactory.Create(editableAlbumObject, filterSetter, sortSetter);
			this.AlbumId.Value = editableAlbumObject.AlbumId;
		}

		/// <summary>
		/// 新規アルバム作成
		/// </summary>
		public void Create(IEditableAlbumObject editableAlbumObject, IFilterDescriptionManager filterSetter, ISortDescriptionManager sortSetter) {
			this._albumLoader = this._albumLoaderFactory.Create(editableAlbumObject, filterSetter, sortSetter);
			var album = new DataBase.Tables.Album();
			lock (this._rdb) {
				this._rdb.Add(album);
				this._rdb.SaveChanges();
			}
			this.AlbumId.Value = album.AlbumId;
		}

		/// <summary>
		/// データベースから登録済み情報の読み込み
		/// </summary>
		public async Task LoadFromDataBase() {
			if (this._albumLoader == null) {
				throw new InvalidOperationException();
			}
			lock (this._rdb) {
				var album =
					this._rdb
						.Albums
						.Include(x => x.AlbumScanDirectories)
						.Where(x => x.AlbumId == this.AlbumId.Value)
						.Select(x => new { x.Title, x.AlbumBoxId, Directories = x.AlbumScanDirectories!.Select(d => d.Directory) })
						.Single();

				this.Title.Value = album.Title;
				this.AlbumBoxId.Value = album.AlbumBoxId;
				this.Directories.Clear();
				this.Directories.AddRange(album.Directories);
			}

			var items = await this._albumLoader.LoadMediaFiles(
				new TaskActionState(new ReactivePropertySlim<string>("アルバム読み込み"), new ReactivePropertySlim<double?>(), new ReactivePropertySlim<double>(), CancellationToken.None));
			this.Items.Clear();
			if (items == null) {
				return;
			}
			this.Items.AddRange(items);
		}

		/// <summary>
		/// アルバムプロパティ項目の編集をデータベースに反映する
		/// </summary>
		public void ReflectToDataBase() {
			lock (this._rdb) {
				var album = this._rdb.Albums.Include(a => a.AlbumScanDirectories).Single(a => a.AlbumId == this.AlbumId.Value);
				album.Title = this.Title.Value;
				album.AlbumBoxId = this.AlbumBoxId.Value;
				album.AlbumScanDirectories!.Clear();
				album.AlbumScanDirectories.AddRange(this.Directories.Select(x =>
					new AlbumScanDirectory {
						Directory = x
					}));
				this._rdb.SaveChanges();
			}
		}

		/// <summary>
		/// アルバムへファイル追加
		/// </summary>
		public void AddFiles(IEnumerable<IMediaFileModel> mediaFiles) {
			var mfs = mediaFiles.ToArray();
			// データ登録
			lock (this._rdb) {
				var mediaFileIds = this._rdb.AlbumMediaFiles.Where(x => x.AlbumId == this.AlbumId.Value).Select(x => x.MediaFileId).AsEnumerable().ToArray();
				this._rdb.AlbumMediaFiles.AddRange(mfs.Where(x => x.MediaFileId is { } id && !mediaFileIds.Contains(id)).Select(x => new AlbumMediaFile {
					AlbumId = this.AlbumId.Value,
					MediaFileId = x.MediaFileId!.Value
				}));
				this._rdb.SaveChanges();
			}
		}

		/// <summary>
		/// アルバムからファイル削除
		/// </summary>
		/// <param name="mediaFiles"></param>
		public void RemoveFiles(IEnumerable<IMediaFileModel> mediaFiles) {
			lock (this._rdb) {
				var mfs = this._rdb.AlbumMediaFiles.Where(x => x.AlbumId == this.AlbumId.Value && mediaFiles.Any(m => m.MediaFileId == x.MediaFileId));
				this._rdb.AlbumMediaFiles.RemoveRange(mfs);
				this._rdb.SaveChanges();
			}
		}


	}
}
