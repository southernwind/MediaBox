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
using SandBeige.MediaBox.Library.Expressions;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
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
		public RegisteredAlbum(IFilterSetter filter, ISortSetter sort) : base(new ObservableSynchronizedCollection<IMediaFileModel>(), filter, sort) {
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

			Get.Instance<AlbumContainer>()
				.AlbumUpdated
				.Where(x => x == this.AlbumId.Value)
				.Subscribe(x => {
					this.LoadFromDataBase(x);
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
				this.Directories.Clear();
				this.Directories.AddRange(album.Directories);
			}

			this.LoadMediaFiles();
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

			var mfs = mediaFiles.ToArray();
			this.PriorityTaskQueue.AddTask(
				new TaskAction(
					$"アルバムへファイル追加",
					() => {
						// データ登録
						lock (this.DataBase) {
							this.DataBase.AlbumMediaFiles.AddRange(mfs.Select(x => new AlbumMediaFile {
								AlbumId = this.AlbumId.Value,
								MediaFileId = x.MediaFileId.Value
							}));
							this.DataBase.SaveChanges();
						}

						Get.Instance<AlbumContainer>().OnAlbumUpdated(this.AlbumId.Value);
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
				var mfs = this.DataBase.AlbumMediaFiles.Where(x => x.AlbumId == this.AlbumId.Value && mediaFiles.Any(m => m.MediaFileId == x.MediaFileId));
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
			// 普通に書くと↓で良い。
			// return mediaFile => mediaFile.AlbumMediaFiles.Any(x => x.AlbumId == this.AlbumId.Value) ||
			//	this.Directories
			//		.Any(x => mediaFile.DirectoryPath.StartsWith(x));
			// ただ、これだとthis.Dictionaries.Any(...)の部分が大量の小さなSQLに分割されてしまうので、パフォーマンスがかなり落ちる。
			// .Any(...)は普通だとIN句に変換されるんだけど、中でStartsWithをやってしまっているので、IN句にできなくて仕方なく複数SQLに変換している模様？
			// これを、
			// this.AlbumId IN AlbumMediaFiles.AlbumId OR
			// this.Directories[0].StartsWith(mediaFile.DirectoryPath) OR
			// this.Directories[1].StartsWith(mediaFile.DirectoryPath) OR
			// this.Directories[2].StartsWith(mediaFile.DirectoryPath)
			// ...というようなSQLに変換させるため、式木を組み立てる。

			// アルバムIDは絶対に条件に含むので、これをベースに組み立てる
			Expression<Func<MediaFile, bool>> exp1 = mediaFile => mediaFile.AlbumMediaFiles.Any(x => x.AlbumId == this.AlbumId.Value);
			var exp = exp1.Body;
			var visitor = new ParameterVisitor(exp1.Parameters);

			// ディレクトリ指定があれば
			foreach (var dir in this.Directories) {
				Expression<Func<MediaFile, bool>> exp2 = mediaFile => mediaFile.DirectoryPath.StartsWith(dir);
				exp = Expression.OrElse(exp, visitor.Visit(exp2.Body));
			}

			return Expression.Lambda<Func<MediaFile, bool>>(
				exp,
				visitor.Parameters
			);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
