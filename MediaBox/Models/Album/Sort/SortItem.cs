using System;
using System.ComponentModel;
using System.Reactive.Linq;

using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// ソート条件
	/// </summary>
	internal class SortItem : ModelBase {
		/// <summary>
		/// プロパティ名
		/// </summary>
		public string PropertyName {
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
		/// <param name="propertyName">プロパティ名</param>
		/// <param name="displayName">表示名</param>
		public SortItem(string propertyName, string displayName) {
			this.PropertyName = propertyName;
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
