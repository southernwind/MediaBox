using System;
using System.Linq;
using System.Linq.Expressions;

using Livet;

using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// フォルダアルバム
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class FolderAlbum : AlbumModel {
		public string DirectoryPath {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path">対象のフォルダパス</param>
		/// <param name="selector">このクラスを保有しているアルバムセレクター</param>
		public FolderAlbum(
			string path,
			IAlbumSelector selector,
			ISettings settings,
			ILogging logging,
			IGestureReceiver gestureReceiver,
			MediaBoxDbContext rdb,
			MediaFactory mediaFactory,
			DocumentDb documentDb,
			NotificationManager notificationManager,
			ViewModelFactory viewModelFactory,
			PriorityTaskQueue priorityTaskQueue,
			MediaFileManager mediaFileManager,
			AlbumViewerManager albumViewerManager,
			GeoCodingManager geoCodingManager) :
			base(new ObservableSynchronizedCollection<IMediaFileModel>(),
				selector,
				settings,
				gestureReceiver,
				rdb,
				mediaFactory,
				documentDb,
				notificationManager,
				logging,
				viewModelFactory,
				priorityTaskQueue,
				mediaFileManager,
				albumViewerManager,
				geoCodingManager) {
			this.Title.Value = path;
			this.DirectoryPath = path;
			this.LoadMediaFiles();

			mediaFileManager
				.OnRegisteredMediaFiles
				.Subscribe(x => {
					this.UpdateBeforeFilteringCount();
					lock (this.Items) {
						this.Items.AddRange(
							x.Where(m => m.FilePath.StartsWith($@"{this.DirectoryPath}")).Where(selector.FilterSetter)
						);
					}
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			return mediaFile => mediaFile.DirectoryPath.StartsWith(this.DirectoryPath);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}

	}
}
