
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Media;
namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// メディアファイルコレクションViewModel基底クラス
	/// </summary>
	/// <typeparam name="T">Model型</typeparam>
	internal abstract class MediaFileCollectionViewModel<T> : ViewModelBase
		where T : MediaFileCollection {
		/// <summary>
		/// メディアファイルコレクションModel
		/// </summary>
		public T Model {
			get;
		}

		/// <summary>
		/// 件数
		/// </summary>
		public IReadOnlyReactiveProperty<int> Count {
			get;
		}

		/// <summary>
		/// メディアファイルViewModelリスト
		/// </summary>
		public ReadOnlyReactiveCollection<IMediaFileViewModel> Items {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mediaFileCollection">モデルインスタンス</param>
		protected MediaFileCollectionViewModel(T mediaFileCollection) {
			this.Model = mediaFileCollection;
			this.ModelForToString = this.Model;

			this.Count = mediaFileCollection.Count.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			// このロック、複雑でわかりづらいけど重要で、ReadOnlyReactiveCollectionは内部で以下のようなことをやっていて特定のシチュエーションでロックされていないと問題が起こる
			// 1. ToReadOnlyReactiveCollectionでReadOnlyReactiveCollectionのインスタンスを作成したときに元になったコレクションにM→VMの変換をかけてObservableCollectionにする(今回だと、this.ViewModelFactory.Createを使う)
			// 2. ReadOnlyReactiveCollectionは元になったコレクションのCollectionChangedイベントを購読して追加や削除時にそなえ、イベントが発生したら自身のコレクションを変化させる
			// で、1.を作るときにself.Select(converter)をやっているんだけど、selfというのは今回でいえばObservableSynchronizedCollectionでGetEnumeratorをしたときに内部でスレッドセーフにToArrayしているので、ここまでは大丈夫
			// ただ、Select以降は保証がなくて、Selectに入ってから2.の購読を開始するまでにコレクションに変化が起こると整合性が取れなくなるためロックをかける
			// コレクションを変化させるほうにもロックをする必要があってVM作成中にModelのコレクションが変化する可能性がある場合は必ずSyncRootでロックしなければならない。
			lock (mediaFileCollection.Items.SyncRoot) {
				this.Items = mediaFileCollection.Items.ToReadOnlyReactiveCollection(
					mediaFileCollection.Items.ToCollectionChanged<IMediaFileModel>(),
					this.ViewModelFactory.Create,
					disposeElement: false
				).AddTo(this.CompositeDisposable);
			}

			// リフレクションキャッシュ生成
			// メディアファイルリストの内部リスト(OC in RORC)
			var oc =
				(ObservableCollection<IMediaFileViewModel>)
					this
						.Items
						.GetType()
						.GetProperty("Source", BindingFlags.NonPublic | BindingFlags.Instance)
						.GetValue(this.Items);
			// メディアファイルリストの内部リストの更に内部リスト(List in OC)
			var innnerList =
				(List<IMediaFileViewModel>)
					oc.GetType()
						.GetProperty("Items", BindingFlags.NonPublic | BindingFlags.Instance)
						.GetValue(oc);

			// メディアファイルリストのコレクション変更通知用メソッド
			var methodInfo =
				this
					.Items
					.GetType()
					.GetMethod("OnCollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance);

			var onCollectionChanged =
				(Action<ReadOnlyReactiveCollection<IMediaFileViewModel>, NotifyCollectionChangedEventArgs>)
					Delegate.CreateDelegate(
						typeof(Action<ReadOnlyReactiveCollection<IMediaFileViewModel>, NotifyCollectionChangedEventArgs>),
						methodInfo
					);

			this.Items
				.ToCollectionChanged()
				.Where(x => x.Action == NotifyCollectionChangedAction.Reset && mediaFileCollection.Items.Count != this.Items.Count)
				.Subscribe(x => {
					innnerList.Clear();
					innnerList.AddRange(mediaFileCollection.Items.Select(this.ViewModelFactory.Create));
					onCollectionChanged(this.Items, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				});

			// モデル破棄時にこのインスタンスも破棄
			this.AddTo(mediaFileCollection.CompositeDisposable);
		}
	}
}
