using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reactive.Bindings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	internal class MediaFileProperties : MediaFileCollection {
		/// <summary>
		/// タグリスト
		/// </summary>
		public ReactivePropertySlim<IEnumerable<ValueCountPair<string>>> Tags {
			get;
		} = new ReactivePropertySlim<IEnumerable<ValueCountPair<string>>>();

		public MediaFileProperties() {
			void func(object s,NotifyCollectionChangedEventArgs e) {
				this.UpdateTags();
			}
			this.Items
				.ToCollectionChanged()
				.Subscribe(x => {
					this.UpdateTags();
					if (x.Action == NotifyCollectionChangedAction.Add) {
						x.Value.Tags.CollectionChanged += func;
					} else if(x.Action == NotifyCollectionChangedAction.Remove){
						x.Value.Tags.CollectionChanged -= func;
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

		private void UpdateTags() {
			this.Tags.Value =
				this.Items
					.SelectMany(x => x.Tags)
					.GroupBy(x => x)
					.Select(x => new ValueCountPair<string>(x.Key, x.Count()));
		}
	}

	internal class ValueCountPair<T> {
		public ValueCountPair(T value, int count) {
			this.Value = value;
			this.Count = count;
		}

		public T Value {
			get;
		}

		public int Count {
			get;
		}
	}
}
