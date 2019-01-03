using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Collection;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Map {
	internal class GpsSelector : ModelBase {

		private readonly Subject<Unit> _onGpsSet = new Subject<Unit>();
		/// <summary>
		/// GPS登録完了通知
		/// </summary>
		public IObservable<Unit> OnGpsSet {
			get {
				return this._onGpsSet.AsObservable();
			}
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public ReactivePropertySlim<double> Latitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// 経度
		/// </summary>
		public ReactivePropertySlim<double> Longitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// GPS設定対象候補一覧
		/// </summary>
		public ReactiveCollection<MediaFile> CandidateMediaFiles {
			get;
		} = new ReactiveCollection<MediaFile>();

		/// <summary>
		/// GPS設定対象ファイル一覧
		/// </summary>
		public TwoWaySynchronizeReactiveCollection<MediaFile> TargetFiles {
			get;
		} = new TwoWaySynchronizeReactiveCollection<MediaFile>();

		/// <summary>
		/// マップモデル
		/// </summary>
		public ReactivePropertySlim<MapModel> Map {
			get;
		} = new ReactivePropertySlim<MapModel>(Get.Instance<MapModel>());

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GpsSelector() {
			// 設定候補一覧→マップモデルアイテム片方向同期
			this.CandidateMediaFiles.SynchronizeTo(this.Map.Value.Items).AddTo(this.CompositeDisposable);

			// 設定対象アイテム→マップポインター
			this.TargetFiles
				.ToCollectionChanged()
				.Subscribe(x => {
					if (this.TargetFiles.Count == 0) {
						this.Map.Value.Pointer.Value = null;
						return;
					}
					var mg = Get.Instance<MediaGroup>(this.TargetFiles[0], default(Rectangle));
					foreach (var item in this.TargetFiles.Skip(1).ToArray()) {
						mg.Items.Add(item);
					}

					this.Map.Value.Pointer.Value = mg;
				}).AddTo(this.CompositeDisposable);

			// 設定対象アイテム→マップ無視ファイル
			this.TargetFiles.SynchronizeTo(this.Map.Value.IgnoreMediaFiles).AddTo(this.CompositeDisposable);

			// 緯度→ポインタ座標片方向同期
			this.Latitude.Subscribe(x => {
				this.Map.Value.PointerLatitude.Value = x;
			}).AddTo(this.CompositeDisposable);

			// 経度→ポインタ座標片方向同期
			this.Longitude.Subscribe(x => {
				this.Map.Value.PointerLongitude.Value = x;
			}).AddTo(this.CompositeDisposable);

			// マップイベント
			var map = this.Map.Value.MapControl.Value;

			// マウス移動
			Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
				h => (sender, e) => h(e),
				h => map.MouseMove += h,
				h => map.MouseMove -= h
				).Subscribe(e => {
					var vp = map.ViewportPointToLocation(e.GetPosition(map));
					this.Latitude.Value = vp.Latitude;
					this.Longitude.Value = vp.Longitude;
				}).AddTo(this.CompositeDisposable);

			// マウスダブルクリック
			Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
				h => (sender, e) => h(e),
				h => map.MouseDoubleClick += h,
				h => map.MouseDoubleClick -= h
				).Subscribe(e => {
					this.SetGps();
					e.Handled = true;
				}).AddTo(this.CompositeDisposable);

			// マップ上での選択変更
			this.Map.Value.OnSelect.Subscribe(x => {
				// 対象ファイルが有る状態での選択変更は無効
				if (this.TargetFiles.Any()) {
					return;
				}
				this.TargetFiles.RemoveRequest(this.TargetFiles);
				this.TargetFiles.AddRequest(x);
			});
		}

		/// <summary>
		/// GPS設定
		/// </summary>
		public void SetGps() {
			var targetArray = this.TargetFiles.Where(x => x.MediaFileId.HasValue).ToArray();

			if (!targetArray.Any()) {
				return;
			}

			using (var tran = this.DataBase.Database.BeginTransaction()) {
				var mfs =
					this.DataBase
						.MediaFiles
						.Where(x => targetArray.Select(m => m.MediaFileId.Value).Contains(x.MediaFileId))
						.ToList();

				foreach (var mf in mfs) {
					mf.Latitude = this.Latitude.Value;
					mf.Longitude = this.Longitude.Value;
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
				item.Latitude.Value = this.Latitude.Value;
				item.Longitude.Value = this.Longitude.Value;
			}

			this._onGpsSet.OnNext(Unit.Default);
		}
	}
}
