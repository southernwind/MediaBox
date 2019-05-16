using System;
using System.ComponentModel;

using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// ソート条件
	/// </summary>
	internal class SortItem<TKey> : ModelBase, ISortItem {
		/// <summary>
		/// 保存時のキー値
		/// </summary>
		public string Key {
			get {
				return this.GetValue<string>();
			}
			set {
				this.SetValue(value);
			}
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return this.GetValue<string>();
			}
			set {
				this.SetValue(value);
			}
		}

		public Func<IMediaFileModel, TKey> KeySelector {
			get {
				return this.GetValue<Func<IMediaFileModel, TKey>>();
			}
			set {
				this.SetValue(value);
			}
		}

		/// <summary>
		/// 有効 / 無効
		/// </summary>
		public bool Enabled {
			get {
				return this.GetValue<bool>();
			}
			set {
				this.SetValue(value);
			}
		}

		/// <summary>
		/// ソートの方向
		/// </summary>
		public ListSortDirection Direction {
			get {
				return this.GetValue<ListSortDirection>();
			}
			set {
				this.SetValue(value);
			}
		}

		/// <summary>
		/// 最終変更日時
		/// </summary>
		public DateTime UpdateTime {
			get {
				return this.GetValue<DateTime>();
			}
			set {
				this.SetValue(value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="key">保存時のキー</param>
		/// <param name="keySelector">キー選択式</param>
		/// <param name="displayName">表示名</param>
		public SortItem(string key, Func<IMediaFileModel, TKey> keySelector, string displayName) {
			this.Key = key;
			this.KeySelector = keySelector;
			this.DisplayName = displayName;

			// 有効無効が切り替わるたびに最終変更日時を更新する
			this.ObserveProperty(x => x.Enabled)
				.Subscribe(_ => {
					this.UpdateTime = DateTime.Now;
				});
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}
