﻿using System.Collections;
using System.Windows.Controls;
using System.Windows.Media;

namespace SandBeige.MediaBox.Controls.Controls {
	public class ListBoxEx : ListBox {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListBoxEx() {
			this.SelectionChanged += (sender, e) => {
				if (this.SelectedItems.Count == 1) {
					this.ScrollIntoView(this.SelectedItem);
				}
			};

			this.IsVisibleChanged += (sender, e) => {
				this.ScrollIntoView(this.SelectedItem);
			};
		}

		/// <summary>
		/// ItemsSourceの変化時
		/// </summary>
		/// <param name="oldValue">変更前値</param>
		/// <param name="newValue">変更後値</param>
		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
			// スクロール位置をリセット
			if (this.VisualChildrenCount != 0) {
				if (VisualTreeHelper.GetChild(this, 0) is Border border) {
					if (border.Child is ScrollViewer scrollViewer) {
						scrollViewer.ScrollToTop();
					}
				}
			}

			base.OnItemsSourceChanged(oldValue, newValue);
		}
	}
}
