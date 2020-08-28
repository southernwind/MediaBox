using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Controls.Controls {
	/// <summary>
	/// 双方向バインド可能な<see cref="BindableSelectedItems"/>を持つListView
	/// </summary>
	/// <typeparam name="T">バインドするアイテムの型</typeparam>
	public class TwoWayBindableSelectedItemsListView<T> : ListViewEx where T : class {

		/// <summary>
		/// バインド可能選択中アイテム 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty BindableSelectedItemsProperty =
			DependencyProperty.Register(nameof(BindableSelectedItems),
				typeof(IEnumerable<T>),
				typeof(TwoWayBindableSelectedItemsListView<T>),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) => {
					if (sender is TwoWayBindableSelectedItemsListView<T> lb) {
						lb.OnBindableSelectedItemsPropertyChanged();
					}
				}));


		private bool _selectionChanging;
		private bool _bindableSelectedItemsChanging;

		/// <summary>
		/// バインド可能選択中アイテム CLR用
		/// </summary>
		public IEnumerable<T> BindableSelectedItems {
			get {
				return (IEnumerable<T>)this.GetValue(BindableSelectedItemsProperty);
			}
			set {
				this.SetValue(BindableSelectedItemsProperty, value);
			}
		}

		/// <summary>
		/// <see cref="ListViewEx.SelectedItems"/>変更時
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			if (this._bindableSelectedItemsChanging) {
				return;
			}
			if (this.SelectedItems.Count == 0) {
				return;
			}
			this._selectionChanging = true;

			this.BindableSelectedItems = this.SelectedItems.Cast<T>().ToArray();

			this._selectionChanging = false;

			base.OnSelectionChanged(e);
		}

		/// <summary>
		/// <see cref="BindableSelectedItems"/>変更時
		/// </summary>
		private void OnBindableSelectedItemsPropertyChanged() {
			if (this._selectionChanging) {
				return;
			}
			var array = this.BindableSelectedItems;

			this._bindableSelectedItemsChanging = true;
			if (array != null) {
				this.SetSelectedItems(array);
			}
			this._bindableSelectedItemsChanging = false;
		}

		/// <summary>
		/// リフレクションを利用してSelectedItemsの内容を書き換える
		/// </summary>
		/// <param name="selectedItems">書き換え後の選択中アイテムリスト</param>
		private void SetSelectedItems(IEnumerable<T> selectedItems) {
			var items = (IList<object>)typeof(ObservableCollection<object>).GetProperty("Items", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(this.SelectedItems)!;
			var methodInfo = typeof(ObservableCollection<object>).GetMethod("OnCollectionReset", BindingFlags.NonPublic | BindingFlags.Instance)!;
			var onCollectionReset =
				(Action<ObservableCollection<object>>)
					Delegate.CreateDelegate(
						typeof(Action<ObservableCollection<object>>),
						methodInfo
					);
			items.Clear();
			items.AddRange(selectedItems);
			onCollectionReset.Invoke((ObservableCollection<object>)this.SelectedItems);
		}
	}
}