using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SandBeige.MediaBox.Controls.AttachedProperties {
	/// <summary>
	/// GridViewColumnの列をViewModelから動的に変更するための添付プロパティ
	/// </summary>
	/// <remarks>
	/// StackOverFlow(https://stackoverflow.com/questions/2643545/)の回答を元に編集。
	/// 回答のままだとGridViewColumnのTemplateを変更できなかったので、変更できるようにした。
	/// <see cref="ColumnsSourceProperty"/>に列情報配列を入れ、列情報にはキープロパティを含めるようにする。
	/// <see cref="GridViewColumnTemplatesProperty"/>は<see cref="AlternateGridViewColumn"/>型の列View配列を入れる。ここで各列の見た目を定義する。
	/// <see cref="ColumnsSourceProperty"/>と<see cref="GridViewColumnTemplatesProperty"/>を関連づけるのがキープロパティで、
	/// <see cref="ColumnsSourceProperty"/>のキープロパティ名を<see cref="AlternateKeyMemberProperty"/>で指定する。
	/// <see cref="GridViewColumnTemplatesProperty"/>のキープロパティは<see cref="AlternateGridViewColumn.AlternateKeyProperty"/>なので、View側であらかじめ指定しておく。
	/// 実行時、二つのプロパティで関連付けが行われ、<see cref="GridViewColumnTemplatesProperty"/>に含まれる列だけが表示される。
	/// </remarks>
	public static class GridViewColumns {
		#region ColumnsSource
		[AttachedPropertyBrowsableForType(typeof(GridView))]
		public static object GetColumnsSource(DependencyObject obj) {
			return obj.GetValue(ColumnsSourceProperty);
		}

		public static void SetColumnsSource(DependencyObject obj, object value) {
			obj.SetValue(ColumnsSourceProperty, value);
		}

		public static readonly DependencyProperty ColumnsSourceProperty =
			DependencyProperty.RegisterAttached(
				"ColumnsSource",
				typeof(object),
				typeof(GridViewColumns),
				new UIPropertyMetadata(
					null,
					ColumnsSourceChanged));
		#endregion

		#region GridViewColumnTemplates
		[AttachedPropertyBrowsableForType(typeof(GridView))]
		public static AlternateGridViewColumnCollection GetGridViewColumnTemplates(DependencyObject obj) {
			return (AlternateGridViewColumnCollection)obj.GetValue(GridViewColumnTemplatesProperty);
		}

		public static void SetGridViewColumnTemplates(DependencyObject obj, AlternateGridViewColumnCollection value) {
			obj.SetValue(GridViewColumnTemplatesProperty, value);
		}

		public static readonly DependencyProperty GridViewColumnTemplatesProperty =
			DependencyProperty.RegisterAttached("GridViewColumnTemplates", typeof(AlternateGridViewColumnCollection), typeof(GridViewColumns), new UIPropertyMetadata(null));
		#endregion

		#region AlternateKey
		[AttachedPropertyBrowsableForType(typeof(GridView))]
		public static string GetAlternateKeyMember(DependencyObject obj) {
			return (string)obj.GetValue(AlternateKeyMemberProperty);
		}

		public static void SetAlternateKeyMember(DependencyObject obj, string value) {
			obj.SetValue(AlternateKeyMemberProperty, value);
		}

		public static readonly DependencyProperty AlternateKeyMemberProperty =
			DependencyProperty.RegisterAttached("AlternateKeyMember", typeof(string), typeof(GridViewColumns), new UIPropertyMetadata(null));
		#endregion

		/// <summary>
		/// カラム列配列変更時
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="e"></param>
		private static void ColumnsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			if (!(obj is GridView gridView)) {
				return;
			}

			gridView.Columns.Clear();

			var oldView = CollectionViewSource.GetDefaultView(e.OldValue);
			if (e.OldValue != null) {
				if (oldView != null) {
					RemoveHandlers(gridView, oldView);
				}
			}

			if (e.NewValue == null) {
				return;
			}

			var newView = CollectionViewSource.GetDefaultView(e.NewValue);
			if (newView == null) {
				return;
			}

			AddHandlers(gridView, newView);
			CreateColumns(gridView, newView);
		}

		private static readonly IDictionary<ICollectionView, List<GridView>> GridViewsByColumnsSource = new Dictionary<ICollectionView, List<GridView>>();

		private static List<GridView> GetGridViewsForColumnSource(ICollectionView columnSource) {
			if (GridViewsByColumnsSource.TryGetValue(columnSource, out var gridViews)) {
				return gridViews;
			}

			gridViews = new List<GridView>();
			GridViewsByColumnsSource.Add(columnSource, gridViews);
			return gridViews;
		}

		private static void AddHandlers(GridView gridView, ICollectionView view) {
			GetGridViewsForColumnSource(view).Add(gridView);
			view.CollectionChanged += ColumnsSource_CollectionChanged;
		}

		private static void CreateColumns(GridView gridView, ICollectionView view) {
			foreach (var item in view) {
				gridView.Columns.Add(CreateColumn(gridView, item));
			}
		}

		private static void RemoveHandlers(GridView gridView, ICollectionView view) {
			view.CollectionChanged -= ColumnsSource_CollectionChanged;
			GetGridViewsForColumnSource(view).Remove(gridView);
		}

		/// <summary>
		/// カラム列コレクション変化時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void ColumnsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			var view = sender as ICollectionView;
			var gridViews = GetGridViewsForColumnSource(view);
			if (gridViews == null || gridViews.Count == 0) {
				return;
			}

			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (var gridView in gridViews) {
						for (var i = 0; i < e.NewItems.Count; i++) {
							var column = CreateColumn(gridView, e.NewItems[i]);
							gridView.Columns.Insert(e.NewStartingIndex + i, column);
						}
					}
					break;
				case NotifyCollectionChangedAction.Move:
					foreach (var gridView in gridViews) {
						var columns = new List<GridViewColumn>();
						for (var i = 0; i < e.OldItems.Count; i++) {
							var column = gridView.Columns[e.OldStartingIndex + i];
							columns.Add(column);
						}
						for (var i = 0; i < e.NewItems.Count; i++) {
							var column = columns[i];
							gridView.Columns.Insert(e.NewStartingIndex + i, column);
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (var gridView in gridViews) {
						for (var i = 0; i < e.OldItems.Count; i++) {
							gridView.Columns.RemoveAt(e.OldStartingIndex);
						}
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					foreach (var gridView in gridViews) {
						for (var i = 0; i < e.NewItems.Count; i++) {
							var column = CreateColumn(gridView, e.NewItems[i]);
							gridView.Columns[e.NewStartingIndex + i] = column;
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					foreach (var gridView in gridViews) {
						gridView.Columns.Clear();
						CreateColumns(gridView, sender as ICollectionView);
					}
					break;
			}
		}

		private static GridViewColumn CreateColumn(GridView gridView, object columnSource) {
			var alternateKeyMember = GetAlternateKeyMember(gridView);
			var alternateKey = columnSource?.GetType().GetProperty(alternateKeyMember)?.GetValue(columnSource, null);
			if (alternateKey == null) {
				return new GridViewColumn();
			}

			var templates = GetGridViewColumnTemplates(gridView);
			var col = templates?.FirstOrDefault(x => x.AlternateKey.Equals(alternateKey));
			return col ?? new GridViewColumn();
		}
	}

	public class AlternateGridViewColumn : GridViewColumn {
		/// <summary>
		///   <see cref="AlternateKeyProperty" /> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty AlternateKeyProperty =
			DependencyProperty
				.Register(
					nameof(AlternateKey),
					typeof(object),
					typeof(AlternateGridViewColumn));

		public object AlternateKey {
			get {
				return this.GetValue(AlternateKeyProperty);
			}
			set {
				this.SetValue(AlternateKeyProperty, value);
			}
		}
	}

	public class AlternateGridViewColumnCollection : ObservableCollection<AlternateGridViewColumn> {

	}
}
