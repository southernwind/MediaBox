using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Livet;

using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Map {
	internal class GeoCodingManager : ModelBase {
		/// <summary>
		/// タスク処理キュー
		/// </summary>
		private readonly PriorityTaskQueue _priorityTaskQueue = Get.Instance<PriorityTaskQueue>();

		/// <summary>
		/// キャンセルトークン Dispose時にキャンセルされる。
		/// </summary>
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		/// <summary>
		/// 待機中アイテム
		/// </summary>
		private readonly ObservableSynchronizedCollection<GpsLocation> _waitingItems = new ObservableSynchronizedCollection<GpsLocation>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GeoCodingManager() {
			var cta = new ContinuousTaskAction(
				"座標情報の取得",
				async state => {
					await Task.Run(() => {
						var gc = new GeoCoding();
						while (true) {
							var item = this._waitingItems.FirstOrDefault();
							if (item == null) {
								break;
							}
							state.TaskName.Value = $"座標情報の取得[{this._waitingItems.Count}]";
							try {
								Position position;
								lock (this.DataBase) {
									position = this.DataBase.Positions.First(x => x.Latitude == item.Latitude && x.Longitude == item.Longitude);
									if (position.DisplayName != null) {
										// 登録済みの場合
										this._waitingItems.Remove(item);
										continue;
									}
								}
								var pd = gc.Reverse(item).Result;
								position.DisplayName = pd.DisplayName;
								position.Addresses = pd.Address.Select((x, i) => new PositionAddress {
									Latitude = item.Latitude,
									Longitude = item.Longitude,
									Type = x.Key,
									Name = x.Value,
									SequenceNumber = i
								}).ToList();
								position.NameDetails = pd.NameDetails.Select(x => new PositionNameDetail {
									Latitude = item.Latitude,
									Longitude = item.Longitude,
									Desc = x.Key,
									Name = x.Value
								}).ToList();
								position.BoundingBoxLeft = pd.BoundingBox[0];
								position.BoundingBoxRight = pd.BoundingBox[1];
								position.BoundingBoxTop = pd.BoundingBox[2];
								position.BoundingBoxBottom = pd.BoundingBox[3];
								lock (this.DataBase) {
									this.DataBase.SaveChanges();
								}
								this._waitingItems.Remove(item);
							} catch (Exception ex) {
								this.Logging.Log("位置情報詳細取得失敗", LogLevel.Warning, ex);
							}
						}
					});
				}, Priority.ReverseGeoCoding,
				this._cancellationTokenSource.Token
			);
			this._priorityTaskQueue.AddTask(cta);

			this._waitingItems
				.CollectionChangedAsObservable()
				.Where(x => x.Action == NotifyCollectionChangedAction.Add)
				.Subscribe(x => {
					cta.Restart();
				});
		}

		/// <summary>
		/// 逆ジオコーディング
		/// </summary>
		/// <remarks>
		/// 逆ジオコーディングのキューに加える。そのうち実行される。
		/// </remarks>
		/// <param name="location">座標</param>
		public void Reverse(GpsLocation location) {
			if (this._waitingItems.Contains(location)) {
				return;
			}
			this._waitingItems.Add(location);
		}

		/// <summary>
		/// Dispose
		/// </summary>
		/// <param name="disposing">マネージドリソースの破棄を行うかどうか</param>
		protected override void Dispose(bool disposing) {
			this._cancellationTokenSource.Cancel();
			base.Dispose(disposing);
		}
	}
}
