using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Controls.Controls {
	public class TwoWayBindableSelectedItemsListBox<T> : ListBoxEx where T : class {

		/// <summary>
		/// バインド可能選択中アイテム 依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty BindableSelectedItemsProperty =
			DependencyProperty.Register(nameof(BindableSelectedItems),
				typeof(IEnumerable<T>),
				typeof(TwoWayBindableSelectedItemsListBox<T>),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) => {
					if (sender is TwoWayBindableSelectedItemsListBox<T> lb) {
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
		/// ListBoxExのコレクション変更時
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSelectionChanged(SelectionChangedEventArgs e) {
			if (this._bindableSelectedItemsChanging) {
				return;
			}
			this._selectionChanging = true;
			Console.WriteLine("OnSelectionChanged");

			this.BindableSelectedItems = this.SelectedItems.Cast<T>().ToArray();

			this._selectionChanging = false;

			base.OnSelectionChanged(e);
		}

		private void OnBindableSelectedItemsPropertyChanged() {
			if (this._selectionChanging) {
				return;
			}
			var array = this.BindableSelectedItems;

			Console.WriteLine("OnBindableSelectedItemsPropertyChanged");
			this._bindableSelectedItemsChanging = true;
			this.SelectedItems.Clear();
			if (array != null) {
				((ObservableCollection<object>)this.SelectedItems).AddRange(array);
			}
			this._bindableSelectedItemsChanging = false;
		}
	}
}