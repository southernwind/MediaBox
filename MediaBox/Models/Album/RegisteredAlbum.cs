using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

using Livet;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// 登録アルバム
	/// </summary>
	/// <remarks>
	/// ユーザーが作成したデータベースに登録するアルバム。
	/// 作成後、データベースに登録されるまでは<see cref="AlbumId"/>が0で、登録後に自動採番された値が入る。
	/// 
	/// </remarks>
	internal class RegisteredAlbum : AlbumModel {
		/// <summary>
		/// アルバムID
		/// (subscribe時初期値配信なし)
		/// </summary>
		public IReactiveProperty<int> AlbumId {
			get;
		} = new ReactiveProperty<int>(mode: ReactivePropertyMode.DistinctUntilChanged);

		/// <summary>
		/// アルバム格納パス
		/// </summary>
		/// <remarks>
		/// "/picture/sea/shark"のような感じ。
		/// これをもとに<see cref="AlbumBox"/>が作られる。
		/// </remarks>
		public IReactiveProperty<string> AlbumPath {
			get;
		} = new ReactiveProperty<string>("");

		/// <summary>
		/// 読み込み対象ディレクトリ
		/// </summary>
		public ReactiveCollection<string> Directories {
			get;
		} = new ReactiveCollection<string>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegisteredAlbum() : base(new ObservableSynchronizedCollection<IMediaFileModel>()) {
			var mfm = Get.Instance<MediaFileManager>();
			mfm
				.OnRegisteredMediaFiles
				.Subscribe(x => {
					this.UpdateBeforeFilteringCount();
					// TODO : 非同期で行わないとDataBaseロックとデッドロックの可能性？
					lock (this.Items.SyncRoot) {
						this.Items.AddRange(x.Where(m => this.Directories.Any(d => m.FilePath.StartsWith(d))));
					}
				});
		}

		/// <summary>
		/// 新規アルバム作成
		/// </summary>
		public void Create() {
			var album = new DataBase.Tables.Album();
			lock (this.DataBase) {
				this.DataBase.Add(album);
				this.DataBase.SaveChanges();
			}
			this.AlbumId.Value = album.AlbumId;
		}

		/// <summary>
		/// データベースから登録済み情報の読み込み
		/// </summary>
		public void LoadFromDataBase(int albumId) {
			this.AlbumId.Value = albumId;
			lock (this.DataBase) {
				var album =
					this.DataBase
						.Albums
						.Include(x => x.AlbumDirectories)
						.Where(x => x.AlbumId == this.AlbumId.Value)
						.Select(x => new { x.Title, x.Path, Directories = x.AlbumDirectories.Select(d => d.Directory) })
						.Single();

				this.Title.Value = album.Title;
				this.AlbumPath.Value = album.Path;
				this.Directories.AddRange(album.Directories);
			}

			this.Load();
		}

		/// <summary>
		/// アルバムプロパティ項目の編集をデータベースに反映する
		/// </summary>
		public void ReflectToDataBase() {
			lock (this.DataBase) {
				var album = this.DataBase.Albums.Include(a => a.AlbumDirectories).Single(a => a.AlbumId == this.AlbumId.Value);
				album.Title = this.Title.Value;
				album.Path = this.AlbumPath.Value;
				album.AlbumDirectories.Clear();
				album.AlbumDirectories.AddRange(this.Directories.Select(x =>
					new AlbumDirectory {
						Directory = x
					}));
				this.DataBase.SaveChanges();
			}
		}

		/// <summary>
		/// アルバムへファイル追加
		/// </summary>
		public void AddFiles(IEnumerable<IMediaFileModel> mediaFiles) {
			if (mediaFiles == null) {
				throw new ArgumentNullException();
			}
			this.PriorityTaskQueue.AddTask(
				new TaskAction(
					$"アルバムへファイル追加",
					() => {
						// データ登録
						lock (this.DataBase) {
							this.DataBase.AlbumMediaFiles.AddRange(mediaFiles.Select(x => new AlbumMediaFile {
								AlbumId = this.AlbumId.Value,
								MediaFileId = x.MediaFileId.Value
							}));
							this.DataBase.SaveChanges();
						}

						// データ登録が終わったらこのアルバムのインスタンスに追加
						lock (this.Items.SyncRoot) {
							this.Items.AddRange(mediaFiles);
						}
						this.UpdateBeforeFilteringCount();
					},
					Priority.LoadRegisteredAlbumOnRegister,
					this.CancellationToken
				)
			);
		}

		/// <summary>
		/// アルバムからファイル削除
		/// </summary>
		/// <param name="mediaFiles"></param>
		public void RemoveFiles(IEnumerable<IMediaFileModel> mediaFiles) {
			if (mediaFiles == null) {
				throw new ArgumentNullException();
			}
			lock (this.DataBase) {
				var mfs = this.DataBase.AlbumMediaFiles.Single(x => x.AlbumId == this.AlbumId.Value && mediaFiles.Select(m => m.MediaFileId).Contains(x.MediaFileId));
				this.DataBase.AlbumMediaFiles.RemoveRange(mfs);
				this.DataBase.SaveChanges();
				this.UpdateBeforeFilteringCount();
			}
			this.Items.RemoveRange(mediaFiles);

		}

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			return mediaFile => mediaFile.AlbumMediaFiles.Any(x => x.AlbumId == this.AlbumId.Value) ||
				this.Directories
					.Any(x => mediaFile.DirectoryPath.StartsWith(x));
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
