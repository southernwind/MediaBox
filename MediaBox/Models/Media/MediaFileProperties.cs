using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルプロパティ一覧
	/// 複数のメディアファイルのプロパティをまとめて一つのプロパティとして閲覧できるようにする
	/// </summary>
	internal class MediaFileProperties : MediaFileCollection {
		/// <summary>
		/// タグリスト
		/// </summary>
		public ReactivePropertySlim<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		} = new ReactivePropertySlim<IEnumerable<ValueCountPair<string>>>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileProperties() {
			void func(object s, NotifyCollectionChangedEventArgs e) {
				this.UpdateTags();
			}
			this.Items
				.ToCollectionChanged()
				.Subscribe(x => {
					this.UpdateTags();
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							x.Value.Tags.CollectionChanged += func;
							break;
						case NotifyCollectionChangedAction.Remove:
							x.Value.Tags.CollectionChanged -= func;
							break;
					}
				});
		}

		/// <summary>
		/// タグ追加
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public void AddTag(string tagName) {
			foreach (var item in this.Items) {
				item.AddTag(tagName);
			}
		}

		/// <summary>
		/// タグ削除
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public void RemoveTag(string tagName) {
			foreach (var item in this.Items) {
				item.RemoveTag(tagName);
			}
		}

		/// <summary>
		/// タグ更新
		/// </summary>
		private void UpdateTags() {
			this.Tags.Value =
				this.Items
					.SelectMany(x => x.Tags)
					.GroupBy(x => x)
					.Select(x => new ValueCountPair<string>(x.Key, x.Count()));
		}
	}

	/// <summary>
	/// 値と件数のペア
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal class ValueCountPair<T> {
		public ValueCountPair(T value, int count) {
			this.Value = value;
			this.Count = count;
		}

		/// <summary>
		/// 値
		/// </summary>
		public T Value {
			get;
		}

		/// <summary>
		/// 件数
		/// </summary>
		public int Count {
			get;
		}
	}
}
