
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.ContextMenu;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.Composition.Interfaces.Models.Tool;
using SandBeige.MediaBox.Composition.Interfaces.Services.AlbumServices;
using SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.TaskQueue;

namespace SandBeige.MediaBox.Models.ContextMenu {
	public class MediaFileListContextMenuModel : ModelBase, IMediaFileListContextMenuModel {
		private readonly IMediaFilePropertiesService _mediaFilePropertiesService;
		private readonly IPriorityTaskQueue _priorityTaskQueue;
		private readonly ILogging _logging;
		private readonly IMediaFileManager _mediaFileManager;
		private readonly IExternalToolsFactory _externalToolsFactory;
		private readonly IAlbumEditorService _albumEditorService;


		/// <summary>
		/// 対象メディアファイルViewModelリスト
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> TargetFiles {
			get;
		}

		public IReactiveProperty<IAlbumObject> TargetAlbum {
			get;
		}

		public IReadOnlyReactiveProperty<IAlbumBox> Shelf {
			get;
		}

		public IReadOnlyReactiveProperty<bool> IsRegisteredAlbum {
			get;
		}

		/// <summary>
		/// 対象外部ツール
		/// </summary>
		public ReadOnlyReactiveCollection<IExternalTool> ExternalTools {
			get {
				return
					this._externalToolsFactory
						.Create(this.TargetFiles.Value.First().Extension)
						.ToReadOnlyReactiveCollection();
			}
		}

		public MediaFileListContextMenuModel(
			IMediaFilePropertiesService mediaFilePropertiesService,
			IPriorityTaskQueue priorityTaskQueue,
			ILogging logging,
			IMediaFileManager mediaFileManager,
			IExternalToolsFactory externalToolsFactory,
			IAlbumEditorService albumEditorService,
			IAlbumContainer albumContainer) {
			this._mediaFilePropertiesService = mediaFilePropertiesService;
			this._priorityTaskQueue = priorityTaskQueue;
			this._logging = logging;
			this._mediaFileManager = mediaFileManager;
			this._externalToolsFactory = externalToolsFactory;
			this._albumEditorService = albumEditorService;
			this.TargetFiles = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());
			this.TargetAlbum = new ReactivePropertySlim<IAlbumObject>();
			this.IsRegisteredAlbum = this.TargetAlbum.Select(x => x is RegisteredAlbumObject).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Shelf = albumContainer.Shelf.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable)!;
		}

		/// <summary>
		/// 対象ファイルすべての評価設定
		/// </summary>
		/// <param name="rate"></param>
		public void SetRate(int rate) {
			var targetArray = this.TargetFiles.Value.Select(x => x.MediaFileId).Where(x => x.HasValue).OfType<long>().ToArray();
			if (!targetArray.Any()) {
				return;
			}
			this._mediaFilePropertiesService.SetRate(targetArray, rate);
		}
		/// <summary>
		/// サムネイルの作成
		/// </summary>
		public void CreateThumbnail() {
			// タスクはここで発生させる。ただしこのインスタンスが破棄されても動き続ける。
			var files = this.TargetFiles.Value.ToArray();
			this._priorityTaskQueue.AddTask(new TaskAction("サムネイル作成", x => Task.Run(() => {
				x.ProgressMax.Value = files.Length;
				foreach (var item in files) {
					if (x.CancellationToken.IsCancellationRequested) {
						return;
					}
					item.CreateThumbnail();
					x.ProgressValue.Value++;
				}
			}), Priority.CreateThumbnail, new CancellationTokenSource()));
		}

		/// <summary>
		/// ディレクトリオープン
		/// </summary>
		public void OpenDirectory() {
			var filePath = this.TargetFiles.Value.FirstOrDefault()?.FilePath;
			try {
				Process.Start("explorer.exe", $"/select,\"{filePath}\"");
			} catch (Exception ex) {
				this._logging.Log($"ディレクトリオープンに失敗しました。[{filePath}]", LogLevel.Error, ex);
			}
		}

		/// <summary>
		/// 登録から削除
		/// </summary>
		public void DeleteFileFromRegistry() {
			this._mediaFileManager.DeleteItems(this.TargetFiles.Value);
		}

		/// <summary>
		/// アルバムからファイル削除
		/// </summary>
		public void RemoveMediaFileFromAlbum() {
			if (this.TargetAlbum.Value is not RegisteredAlbumObject rao) {
				return;
			}

			this._albumEditorService.RemoveFiles(rao.AlbumId, this.TargetFiles.Value.Select(x => x.MediaFileId).OfType<long>().ToArray());
		}

		/// <summary>
		/// アルバム
		/// </summary>
		public void AddMediaFileToOtherAlbum(int albumId) {
			this._albumEditorService.AddFiles(albumId, this.TargetFiles.Value.Select(x => x.MediaFileId).OfType<long>().ToArray());
		}
	}
}
