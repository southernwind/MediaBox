using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using Livet;

using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.Composition.Interfaces.Services.MediaFileServices;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.TaskQueue;

namespace SandBeige.MediaBox.Services.MediaFileServices {
	public class GeoCodingService : ModelBase, IGeoCodingService {

		/// <summary>
		/// 待機中アイテム
		/// </summary>
		private readonly ObservableSynchronizedCollection<GpsLocation> _waitingItems = new ObservableSynchronizedCollection<GpsLocation>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GeoCodingService(IDocumentDb documentDb, IMediaBoxDbContext rdb, ILogging logging, IPriorityTaskQueue priorityTaskQueue) {
			var cancellationTokenSource = new CancellationTokenSource().AddTo(this.CompositeDisposable);
			var cta = new ContinuousTaskAction(
				"座標情報の取得",
				async state => {
					await Task.Run(() => {
						var gc = new GeoCoding();
						var positions = documentDb.GetPositionsCollection();
						while (true) {
							if (state.CancellationToken.IsCancellationRequested) {
								return;
							}
							var item = this._waitingItems.FirstOrDefault();
							if (item == null) {
								break;
							}
							state.TaskName.Value = $"座標情報の取得[{this._waitingItems.Count}]";
							try {
								Position position;
								lock (rdb) {
									position = positions.Query().Where(x => x.Latitude == item.Latitude && x.Longitude == item.Longitude).First();
									if (position.IsAcquired) {
										// 登録済みの場合
										this._waitingItems.Remove(item);
										continue;
									}
								}
								var pd = gc.Reverse(item).Result;
								if (pd.DisplayName != null) {
									position.DisplayName = pd.DisplayName;
									position.Addresses = pd.Address?.Select((x, i) => new PositionAddress {
										Type = x.Key,
										Name = x.Value,
										SequenceNumber = i
									}).ToArray();
									position.NameDetails = pd.NameDetails?.Select(x => new PositionNameDetail {
										Desc = x.Key,
										Name = x.Value
									}).ToArray();
									position.BoundingBoxLeft = pd.BoundingBox![0];
									position.BoundingBoxRight = pd.BoundingBox[1];
									position.BoundingBoxTop = pd.BoundingBox[2];
									position.BoundingBoxBottom = pd.BoundingBox[3];
								}
								position.IsAcquired = true;
								lock (rdb) {
									positions.Update(position);
								}
								this._waitingItems.Remove(item);
							} catch (Exception ex) {
								this._waitingItems.Remove(item);
								logging.Log("位置情報詳細取得失敗", LogLevel.Warning, ex);
							}
						}
					});
				}, Priority.ReverseGeoCoding,
				cancellationTokenSource
			);
			priorityTaskQueue.AddTask(cta);

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
	}
}
