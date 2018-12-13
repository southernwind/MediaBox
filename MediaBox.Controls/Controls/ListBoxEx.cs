using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Controls.Controls {
	public class ListBoxEx :ListBox{
		#region BindableSelectedItems
		/// <summary>
		/// バインド可能選択中アイテム 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty BindableSelectedItemsProperty =
			DependencyProperty.Register(nameof(BindableSelectedItems),
				typeof(INotifyCollectionChanged),
				typeof(ListBoxEx));

		/// <summary>
		/// バインド可能選択中アイテム CLR用
		/// </summary>
		public INotifyCollectionChanged BindableSelectedItems {
			get {
				return (INotifyCollectionChanged)this.GetValue(BindableSelectedItemsProperty);
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

			foreach (var item in e.AddedItems) {
				((IList)this.BindableSelectedItems).Add((dynamic)item);
			}
			foreach (var item in e.RemovedItems) {
				((IList)this.BindableSelectedItems).Remove((dynamic)item);
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
