using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Gesture;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.God;

namespace SandBeige.MediaBox.Models.Map {
	/// <summary>
	/// GPS座標選択
	/// </summary>
	/// <remarks>
	/// 保持している<see cref="Map"/>を使いGPS座標の選択を行う。
	/// 選択したGPS座標は、<see cref="TargetFiles"/>の<see cref="IMediaFileModel.Location"/>と、それに紐づくデータベース情報に登録される。
	/// </remarks>
	public class GpsSelector : ModelBase, IGpsSelector {
		private readonly IMediaBoxDbContext _rdb;
		private readonly IDocumentDb _documentDb;
		/// <summary>
		/// 操作受信
		/// </summary>
		public IGestureReceiver GestureReceiver {
			get;
		}

		/// <summary>
		/// 座標
		/// </summary>
		public IReactiveProperty<GpsLocation> Location {
			get;
		} = new ReactivePropertySlim<GpsLocation>();

		/// <summary>
		/// GPS設定対象候補リスト
		/// </summary>
		public ObservableSynchronizedCollection<IMediaFileModel> CandidateMediaFiles {
			get;
		} = new ObservableSynchronizedCollection<IMediaFileModel>();

		/// <summary>
		/// GPS設定対象ファイルリスト
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> TargetFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());

		/// <summary>
		/// マップモデル
		/// </summary>
		public IReactiveProperty<IMapModel> Map {
			get;
		}

		/// <summary>
		/// 一覧ズームレベル
		/// </summary>
		public IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GpsSelector(IMediaBoxDbContext rdb, IDocumentDb documentDb, ISettings settings, IGestureReceiver gestureReceiver, IMapControl mapControl) {
			this._rdb = rdb;
			this._documentDb = documentDb;
			this.GestureReceiver = gestureReceiver;
			this.Map =
				new ReactivePropertySlim<IMapModel>(
					new MapModel(
						this.CandidateMediaFiles,
						this.TargetFiles,
						settings,
						mapControl
					)
				);

			// 設定対象アイテム→マップポインター
			this.TargetFiles
				.Subscribe(x => {
					if (!this.TargetFiles.Value.Any()) {
						this.Map.Value.Pointer.Value = null;
						return;
					}
					var mp = new MapPointer(this.TargetFiles.Value.First());
					foreach (var item in this.TargetFiles.Value.Skip(1).ToArray()) {
						mp.Items.Add(item);
					}

					this.Map.Value.Pointer.Value = mp;
				}).AddTo(this.CompositeDisposable);

			// 設定対象アイテム→マップ無視ファイル片方向同期
			// 設定対象はポインターとして表示するので、マップ上では非表示にする
			this.TargetFiles.Subscribe(x => {
				this.Map.Value.IgnoreMediaFiles.Value = x;
			}).AddTo(this.CompositeDisposable);

			// GPS座標移動
			this.Map.Value.OnMove.Subscribe(e => this.Location.Value = e).AddTo(this.CompositeDisposable);

			// GPS座標決定
			this.Map.Value.OnDecide.Subscribe(_ => {
				this.SetGps();
			}).AddTo(this.CompositeDisposable);

			this.ZoomLevel = this.GestureReceiver
				.MouseWheelEvent
				.Where(_ => this.GestureReceiver.IsControlKeyPressed)
				.ToZoomLevel()
				.AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// GPS設定
		/// </summary>
		public void SetGps() {
			var targetArray = this.TargetFiles.Value.Where(x => x.MediaFileId.HasValue).ToArray();

			if (!targetArray.Any()) {
				return;
			}

			var targetIds = targetArray.Select(m => m.MediaFileId!.Value).ToArray();

			lock (this._rdb) {
				var positions = this._documentDb.GetPositionsCollection();

				if (!positions.Query().Where(x => x.Latitude == this.Location.Value.Latitude && x.Longitude == this.Location.Value.Longitude).Exists()) {
					positions.Insert(new Position() {
						Latitude = this.Location.Value.Latitude,
						Longitude = this.Location.Value.Longitude
					});
				}

				var mediaFiles = this._documentDb.GetMediaFilesCollection();
				var mfs =
					mediaFiles
						.Query()
						.Where(x => targetIds.Contains(x.MediaFileId))
						.ToList();

				foreach (var mf in mfs) {
					mf.Latitude = this.Location.Value.Latitude;
					mf.Longitude = this.Location.Value.Longitude;
				}

				// 1件ずつUPDATEするSQLが発行される。
				// Executed DbCommand (0ms) [Parameters=[@p6='?', @p0='?' (Size = 31), @p1='?' (Size = 12), @p2='?', @p3='?', @p4='?', @p5='?' (Size = 68)], CommandType='Text', CommandTimeout='30']
				// UPDATE "MediaFiles" SET "DirectoryPath" = @p0, "FileName" = @p1, "Latitude" = @p2, "Longitude" = @p3, "Orientation" = @p4, "ThumbnailFileName" = @p5
				// WHERE "MediaFileId" = @p6
				// SQL1文で全件更新するSQLも書けるけど保守性優先でこれで。
				mediaFiles.Update(mfs);
			}

			foreach (var item in targetArray) {
				item.Location = this.Location.Value;
			}

			this.TargetFiles.Value = Array.Empty<IMediaFileModel>();
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.CandidateMediaFiles.FirstOrDefault()} Candidate={this.CandidateMediaFiles.Count} Target={this.TargetFiles.Value.Count()}>";
		}
	}
}
