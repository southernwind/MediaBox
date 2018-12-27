using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Controls.Controls {
	public class ListBoxEx : ListBox {
		#region BindableSelectedItems
		/// <summary>
		/// バインド可能選択中アイテム 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty BindableSelectedItemsProperty =
			DependencyProperty.Register(nameof(BindableSelectedItems),
				typeof(IList),
				typeof(ListBoxEx));

		/// <summary>
		/// バインド可能選択中アイテム CLR用
		/// </summary>
		public IList BindableSelectedItems {
			get {
				return (IList)this.GetValue(BindableSelectedItemsProperty);
			}
			set {
				this.SetValue(BindableSelectedItemsProperty, value);
			}
		}

		/// <summary>
		/// ListBoxExのコレクション変更時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (this.BindableSelectedItems == null) {
				return;
			}

			if (this.SelectionMode == SelectionMode.Single) {
				if (this.SelectedItems.Count == 0) {
					this.BindableSelectedItems.Clear();
				} else if (this.BindableSelectedItems.Count == 0) {
					this.BindableSelectedItems.Add(this.SelectedItems[0]);
				} else {
					if (this.BindableSelectedItems.Count != 1) {
						this.BindableSelectedItems.RemoveAt(1);
					}
					this.BindableSelectedItems[0] = this.SelectedItems[0];
				}
			} else {
				foreach (var item in e.RemovedItems) {
					this.BindableSelectedItems.Remove(item);
				}
				foreach (var item in e.AddedItems) {
					this.BindableSelectedItems.Add(item);
				}
			}
		}

		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ListBoxEx() {
			this.SelectionChanged += this.OnSelectionChanged;
			this.SelectionChanged += (sender, e) => {
				this.ScrollIntoView(this.SelectedItem);
			};

			this.IsVisibleChanged += (sender, e) => {
				this.ScrollIntoView(this.SelectedItem);
			};
		}

	}
}
