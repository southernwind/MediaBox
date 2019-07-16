using System;
using System.Collections.Generic;
using System.Linq;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Map {
	/// <summary>
	/// GPS座標選択
	/// </summary>
	/// <remarks>
	/// 保持している<see cref="Map"/>を使いGPS座標の選択を行う。
	/// 選択したGPS座標は、<see cref="TargetFiles"/>の<see cref="IMediaFileModel.Location"/>と、それに紐づくデータベース情報に登録される。
	/// </remarks>
	internal class GpsSelector : ModelBase {
		/// <summary>
		/// 座標
		/// </summary>
		public IReactiveProperty<GpsLocation> Location {
			get;
		} = new ReactivePropertySlim<GpsLocation>();

		/// <summary>
		/// GPS設定対象候補リスト
		/// </summary>
		public ReactiveCollection<IMediaFileModel> CandidateMediaFiles {
			get;
		} = new ReactiveCollection<IMediaFileModel>();

		/// <summary>
		/// GPS設定対象ファイルリスト
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> TargetFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());

		/// <summary>
		/// マップモデル
		/// </summary>
		public IReactiveProperty<MapModel> Map {
			get;
		} = new ReactivePropertySlim<MapModel>(Get.Instance<MapModel>());

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GpsSelector() {
			// 設定候補リスト→マップモデルアイテム片方向同期
			this.CandidateMediaFiles
				.SynchronizeTo<IMediaFileModel, ReactiveCollection<IMediaFileModel>, ObservableSynchronizedCollection<IMediaFileModel>>(this.Map.Value.Items)
				.AddTo(this.CompositeDisposable);

			// 設定対象アイテム→マップポインター
			this.TargetFiles
				.Subscribe(x => {
					if (!this.TargetFiles.Value.Any()) {
						this.Map.Value.Pointer.Value = null;
						return;
					}
					var mg = Get.Instance<MapPin>(this.TargetFiles.Value.First(), default(Rectangle));
					foreach (var item in this.TargetFiles.Value.Skip(1).ToArray()) {
						mg.Items.Add(item);
					}

					this.Map.Value.Pointer.Value = mg;
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

			// マップ上での選択変更
			this.Map.Value.OnSelect.Subscribe(x => {
				// 対象ファイルが有る状態での選択変更は無効
				if (this.TargetFiles.Value.Any()) {
					return;
				}
				this.TargetFiles.Value = x;
			});
		}

		/// <summary>
		/// GPS設定
		/// </summary>
		public void SetGps() {
			var targetArray = this.TargetFiles.Value.Where(x => x.MediaFileId.HasValue).ToArray();

			if (!targetArray.Any()) {
				return;
			}

			lock (this.DataBase) {
				using var tran = this.DataBase.Database.BeginTransaction();

				if (!this.DataBase.Positions.Any(x => x.Latitude == this.Location.Value.Latitude && x.Longitude == this.Location.Value.Longitude)) {
					this.DataBase.Positions.Add(new Position() {
						Latitude = this.Location.Value.Latitude,
						Longitude = this.Location.Value.Longitude
					});
				}

				var mfs =
					this.DataBase
						.MediaFiles
						.Where(x => targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId))
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
				this.DataBase.UpdateRange(mfs);

				this.DataBase.SaveChanges();
				tran.Commit();
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
