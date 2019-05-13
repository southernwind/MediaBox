using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルコレクション
	/// </summary>
	/// <remarks>
	/// 件数とファイルリストを持つ
	/// </remarks>
	internal class MediaFileCollection : ModelBase {
		/// <summary>
		/// メディアファイルリストの内部リスト
		/// </summary>
		private readonly List<IMediaFileModel> _innerList;
		/// <summary>
		/// メディアファイルリストの変更通知用メソッド情報
		/// </summary>
		private readonly MethodInfo _onCollectionChangedMethodInfo;

		/// <summary>
		/// 件数
		/// </summary>
		public IReactiveProperty<int> Count {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// メディアファイルリスト
		/// VM作成中にコレクションが変化する可能性がある場合は必ずSyncRootでロックすること。
		/// </summary>
		public ObservableSynchronizedCollection<IMediaFileModel> Items {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="items">他所で生成したメディアファイルリスト</param>
		public MediaFileCollection(ObservableSynchronizedCollection<IMediaFileModel> items) {
			this.Items = items;
			this.Items
				.ToCollectionChanged<IMediaFileModel>().ToUnit()
				.Merge(Observable.Return(Unit.Default))
				.Subscribe(_ => {
					this.Count.Value = this.Items.Count;
				}).AddTo(this.CompositeDisposable);

			// リフレクションで取得してキャッシュしておく。
			this._innerList = (List<IMediaFileModel>)this.Items.GetType().GetField("_list", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this.Items);
			this._onCollectionChangedMethodInfo = this.Items.GetType().GetMethod("OnCollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		/// <summary>
		/// <see cref="Items"></see>を引数のコレクションの内容に置き換える
		/// </summary>
		/// <param name="newItems">新しいメディアリスト</param>
		protected void ItemsReset(IEnumerable<IMediaFileModel> newItems) {
			lock (this.Items.SyncRoot) {
				this._innerList.Clear();
				this._innerList.AddRange(newItems);
				this._onCollectionChangedMethodInfo.Invoke(this.Items, new object[] {
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
				});
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Items.FirstOrDefault()} ({this.Count.Value})>";
		}
	}
}
