using System;
using System.Collections.Generic;
using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;

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
			this.Items.ToCollectionChanged().Subscribe(_ => this.UpdateTags());
			this.Items
				.ToReadOnlyReactiveCollection(x => {
					return x.Tags.ToCollectionChanged().Subscribe(_ => {
						this.UpdateTags();
					}).AddTo(this.CompositeDisposable);
				}).AddTo(this.CompositeDisposable)
				.DisposeWhenRemove()
				.AddTo(this.CompositeDisposable);
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
