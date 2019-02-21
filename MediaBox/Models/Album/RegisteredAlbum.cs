﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.Media;
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
		/// データベース登録キュー
		/// subjectのOnNextで発火してitemsの中身をすべて登録する
		/// </summary>
		/// <remarks>
		/// バックグラウンドで遅延実行するためのプロパティ
		/// キューに入った<see cref="IMediaFileModel"/>は順次ファイル情報やサムネイルの取得が行われ、データベースに登録されていく。
		/// データベースに登録されたものから<see cref="MediaFileCollection.Items"/>に追加されていく。
		/// </remarks>
		private (Subject<Unit> subject, ConcurrentQueue<IMediaFileModel> items) QueueOfRegisterToItems {
			get;
		} = (new Subject<Unit>(), new ConcurrentQueue<IMediaFileModel>());

		/// <summary>
		/// ファイル情報取得キュー
		/// subjectのOnNextで発火してitemsの中身をすべて登録する
		/// </summary>
		/// <remarks>
		/// バックグラウンドで遅延実行するためのプロパティ
		/// キューに入った<see cref="IMediaFileModel"/>は順次ファイル情報やサムネイルの取得が行われる。
		/// 登録されているファイルをデータベースから読み込んだあと、Exif情報の取得などを行うため。
		/// </remarks>
		private (Subject<Unit> subject, ConcurrentQueue<IMediaFileModel> items) QueueOfLoad {
			get;
		} = (new Subject<Unit>(), new ConcurrentQueue<IMediaFileModel>());


		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RegisteredAlbum() {
			this.QueueOfRegisterToItems
				.subject
				.CombineLatest(this.AlbumId, (x, y) => x)
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Synchronize()
				.Subscribe(_ => {
					try {
						Parallel.For(
							0,
							this.QueueOfRegisterToItems.items.Count,
							new ParallelOptions {
								CancellationToken = this.CancellationToken,
								MaxDegreeOfParallelism = Environment.ProcessorCount
							}, __ => {
								this.QueueOfRegisterToItems.items.TryDequeue(out var mediaFile);
								if (this.CancellationToken.IsCancellationRequested) {
									return;
								}
								this.RegisterToDataBase(mediaFile);
								// 登録が終わったら追加
								lock (this.Items.SyncRoot) {
									this.Items.Add(mediaFile);
								}
							});
					} catch (Exception ex) when (ex is OperationCanceledException) {
						this.Logging.Log("アルバムデータ登録キャンセル", LogLevel.Debug, ex);
					}
					this.OnInitializedSubject.OnNext(Unit.Default);
				}).AddTo(this.CompositeDisposable);

			this.QueueOfLoad
				.subject
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Synchronize()
				.Subscribe(_ => {
					try {
						Parallel.For(
							0,
							this.QueueOfLoad.items.Count,
							new ParallelOptions {
								CancellationToken = this.CancellationToken,
								MaxDegreeOfParallelism = Environment.ProcessorCount
							}, __ => {
								this.QueueOfLoad.items.TryDequeue(out var mediaFile);
								if (this.CancellationToken.IsCancellationRequested) {
									return;
								}
								mediaFile.GetFileInfoIfNotLoaded();
							});
					} catch (Exception ex) when (ex is OperationCanceledException) {
						this.Logging.Log("アルバム内画像情報読み込みキャンセル", LogLevel.Debug, ex);
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
			var album =
				this.DataBase
					.Albums
					.Include(x => x.AlbumDirectories)
					.Where(x => x.AlbumId == this.AlbumId.Value)
					.Select(x => new { x.Title, x.Path, Directories = x.AlbumDirectories.Select(d => d.Directory) })
					.Single();

			lock (this.Items.SyncRoot) {
				this.Items.AddRange(
					this.DataBase
						.MediaFiles
						.Where(mf => mf.AlbumMediaFiles.Any(amf => amf.AlbumId == this.AlbumId.Value))
						.Include(mf => mf.MediaFileTags)
						.ThenInclude(mft => mft.Tag)
						.Include(mf => mf.ImageFile)
						.Include(mf => mf.VideoFile)
						.AsEnumerable()
						.Select(x => {
							var m = this.MediaFactory.Create(x.FilePath);
							m.LoadFromDataBase(x);
							return m;
						}).ToList()
				);
			}
			this.Title.Value = album.Title;
			this.AlbumPath.Value = album.Path;
			this.MonitoringDirectories.AddRange(album.Directories);

			this.QueueOfLoad.items.EnqueueRange(this.Items);
			this.QueueOfLoad.subject.OnNext(Unit.Default);
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
				album.AlbumDirectories.AddRange(this.MonitoringDirectories.Select(x =>
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
			this.QueueOfRegisterToItems.items.EnqueueRange(mediaFiles);
			this.QueueOfRegisterToItems.subject.OnNext(Unit.Default);
		}

		/// <summary>
		/// アルバムからファイル削除
		/// </summary>
		/// <param name="mediaFiles"></param>
		public void RemoveFiles(IEnumerable<IMediaFileModel> mediaFiles) {
			if (mediaFiles == null) {
				throw new ArgumentNullException();
			}
			foreach (var file in mediaFiles.ToArray()) {
				this.RemoveFromDataBase(file);
				this.Items.Remove(file);
			}
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="cancellationToken">キャンセルトークン</param>
		protected override void LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken) {
			var newItems = DirectoryEx
				.EnumerateFiles(directoryPath)
				.Where(x => x.IsTargetExtension())
				.Where(x => this.Items.Union(this.QueueOfRegisterToItems.items).All(m => m.FilePath != x))
				.Select(x => this.MediaFactory.Create(x));
			foreach (var item in newItems) {
				if (cancellationToken.IsCancellationRequested) {
					return;
				}
				this.QueueOfRegisterToItems.items.Enqueue(item);
			}
			this.QueueOfRegisterToItems.subject.OnNext(Unit.Default);
		}

		/// <summary>
		/// ファイルシステムイベント
		/// </summary>
		/// <param name="e">作成・更新・改名・削除などのイベント情報</param>
		protected override void OnFileSystemEvent(FileSystemEventArgs e) {
			if (!e.FullPath.IsTargetExtension()) {
				return;
			}

			switch (e.ChangeType) {
				case WatcherChangeTypes.Created:
					this.QueueOfRegisterToItems.items.Enqueue(this.MediaFactory.Create(e.FullPath));
					this.QueueOfRegisterToItems.subject.OnNext(Unit.Default);
					break;
				case WatcherChangeTypes.Deleted:
					// TODO : 作成後すぐに削除されると、登録キューに入っていてItemsにはまだ入っていない可能性がある
					var target = this.Items.Single(i => i.FilePath == e.FullPath);
					this.RemoveFromDataBase(target);
					this.Items.Remove(target);
					break;
			}
		}

		/// <summary>
		/// DBに登録
		/// </summary>
		/// <param name="mediaFile">登録ファイル</param>
		private void RegisterToDataBase(IMediaFileModel mediaFile) {
			mediaFile.GetFileInfoIfNotLoaded();
			mediaFile.CreateThumbnailIfNotExists();
			lock (this.DataBase) {
				var mf =
					this.DataBase
						.MediaFiles
						.Include(x => x.AlbumMediaFiles)
						.SingleOrDefault(x => x.FilePath == mediaFile.FilePath) ??
					mediaFile.RegisterToDataBase();

				if (mf.AlbumMediaFiles?.All(x => x.AlbumId != this.AlbumId.Value) ?? true) {
					this.DataBase.AlbumMediaFiles.Add(new AlbumMediaFile {
						AlbumId = this.AlbumId.Value, // nullにはならない
						MediaFile = mf
					});
				}
				this.DataBase.SaveChanges();
			}
		}

		/// <summary>
		/// DBから削除
		/// </summary>
		/// <param name="mediaFile">削除対象ファイル</param>
		private void RemoveFromDataBase(IMediaFileModel mediaFile) {
			lock (this.DataBase) {
				var mf = this.DataBase.AlbumMediaFiles.Single(x => x.AlbumId == this.AlbumId.Value && x.MediaFileId == mediaFile.MediaFileId);
				this.DataBase.AlbumMediaFiles.Remove(mf);
				this.DataBase.SaveChanges();
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
