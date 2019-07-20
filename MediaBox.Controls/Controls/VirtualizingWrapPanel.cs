using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SandBeige.MediaBox.Controls.Controls {
	public class VirtualizingTilePanel : VirtualizingPanel, IScrollInfo {
		#region レイアウト固有コード
		// タイルレイアウト以外の場合はここを変更する。
		public VirtualizingTilePanel() {
			// For use in the IScrollInfo implementation
			this.RenderTransform = this._trans;
		}

		/// <summary>
		/// 子要素のサイズを制御する依存関係プロパティ
		/// </summary>
		public static readonly DependencyProperty ChildSizeProperty
			= DependencyProperty.RegisterAttached("ChildSize", typeof(double), typeof(VirtualizingTilePanel),
				new FrameworkPropertyMetadata(200.0d, FrameworkPropertyMetadataOptions.AffectsMeasure |
													  FrameworkPropertyMetadataOptions.AffectsArrange));

		/// <summary>
		/// 子要素のサイズ
		/// </summary>
		public double ChildSize {
			get {
				return (double)this.GetValue(ChildSizeProperty);
			}
			set {
				this.SetValue(ChildSizeProperty, value);
			}
		}

		/// <summary>
		/// 子要素の計測
		/// </summary>
		/// <param name="availableSize">使用可能な領域のサイズ</param>
		/// <returns>必要なサイズ</returns>
		protected override Size MeasureOverride(Size availableSize) {
			this.UpdateScrollInfo(availableSize);

			// レイアウトアルゴリズムに基づいて表示される範囲を特定する
			this.GetVisibleRange(out var firstVisibleItemIndex, out var lastVisibleItemIndex);

			// バグの回避のため、ジェネレータにアクセスする前にInternalChildrenプロパティのsetterを呼び出しておく必要があるらしい。
			var children = this.InternalChildren;
			var generator = this.ItemContainerGenerator;

			// 見える範囲の項目の、最初のジェネレーター位置を取得する。
			var startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

			// 子を挿入する位置のインデックスを取得する。
			var childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

			using (generator.StartAt(startPos, GeneratorDirection.Forward, true)) {
				for (var itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex) {
					if (!(generator.GenerateNext(out var newlyRealized) is UIElement child)) {
						continue;
					}
					if (newlyRealized) {
						// 子要素を最後に入れるか、間に入れるかを判定する。
						if (childIndex >= children.Count) {
							this.AddInternalChild(child);
						} else {
							this.InsertInternalChild(childIndex, child);
						}

						generator.PrepareItemContainer(child);
					}

					// 子の測定はレイアウトアルゴリズム依存
					child.Measure(this.GetChildSize());
				}
			}

			// ※ これはパフォーマンスを更に改善する場合、ここで行う必要はなく、遅延実行しても良い。
			this.CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);

			return availableSize;
		}

		/// <summary>
		/// 子の配置
		/// </summary>
		/// <param name="finalSize">使用可能領域サイズ</param>
		/// <returns>使用サイズ</returns>
		protected override Size ArrangeOverride(Size finalSize) {
			var generator = this.ItemContainerGenerator;

			this.UpdateScrollInfo(finalSize);

			for (var i = 0; i < this.Children.Count; i++) {
				var child = this.Children[i];

				// 子オフセットを項目オフセットにマッピングする。
				var itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

				this.ArrangeChild(itemIndex, child, finalSize);
			}

			return finalSize;
		}

		/// <summary>
		/// 表示されなくなったアイテムの再仮想化
		/// </summary>
		/// <param name="minDesiredGenerated">表示する必要のアイテムの最小のインデックス</param>
		/// <param name="maxDesiredGenerated">表示する必要のあるアイテムの最大のインデックス</param>
		private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated) {
			var children = this.InternalChildren;
			var generator = this.ItemContainerGenerator;

			for (var i = children.Count - 1; i >= 0; i--) {
				var childGeneratorPos = new GeneratorPosition(i, 0);
				var itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
				if (itemIndex >= minDesiredGenerated && itemIndex <= maxDesiredGenerated) {
					continue;
				}

				generator.Remove(childGeneratorPos, 1);
				this.RemoveInternalChildRange(i, 1);
			}
		}

		/// <summary>
		/// 項目変更時
		/// </summary>
		/// <remarks>
		/// 項目が削除された場合に必要に応じてUI上の子要素も削除する。
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args) {
			switch (args.Action) {
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Move:
					this.RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
					break;
			}
		}


		/// <summary>
		/// 使用可能領域サイズに基づいてビューの範囲を計算する。
		/// </summary>
		/// <param name="availableSize">使用領域サイズ</param>
		/// <param name="itemCount">アイテム個数</param>
		/// <returns></returns>
		private Size CalculateExtent(Size availableSize, int itemCount) {
			var childrenPerRow = this.CalculateChildrenPerRow(availableSize);

			return new Size(childrenPerRow * this.ChildSize, this.ChildSize * Math.Ceiling((double)itemCount / childrenPerRow));
		}

		/// <summary>
		/// 表示されている子の範囲を取得する
		/// </summary>
		/// <param name="firstVisibleItemIndex">表示する必要のアイテムの最小のインデックス</param>
		/// <param name="lastVisibleItemIndex">表示する必要のあるアイテムの最大のインデックス</param>
		private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex) {
			var childrenPerRow = this.CalculateChildrenPerRow(this._extent);

			firstVisibleItemIndex = (int)Math.Floor(this._offset.Y / this.ChildSize) * childrenPerRow;
			lastVisibleItemIndex = (int)Math.Ceiling((this._offset.Y + this._viewport.Height) / this.ChildSize) * childrenPerRow - 1;

			var itemsControl = ItemsControl.GetItemsOwner(this);
			var itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
			if (lastVisibleItemIndex < itemCount) {
				return;
			}

			lastVisibleItemIndex = itemCount - 1;

		}

		/// <summary>
		/// 子のサイズの取得
		/// </summary>
		/// <returns>サイズ</returns>
		private Size GetChildSize() {
			// タイルレイアウトなので、固定
			return new Size(this.ChildSize, this.ChildSize);
		}

		/// <summary>
		/// 子要素の配置
		/// </summary>
		/// <param name="itemIndex">子要素のインデックス</param>
		/// <param name="child">配置先の要素</param>
		/// <param name="finalSize">パネルサイズ</param>
		private void ArrangeChild(int itemIndex, UIElement child, Size finalSize) {
			var childrenPerRow = this.CalculateChildrenPerRow(finalSize);

			var row = itemIndex / childrenPerRow;
			var column = itemIndex % childrenPerRow;

			child.Arrange(new Rect(column * this.ChildSize, row * this.ChildSize, this.ChildSize, this.ChildSize));
		}

		/// <summary>
		/// タイルレイアウト用のヘルパー関数
		/// </summary>
		/// <param name="availableSize">使用可能領域サイズ</param>
		/// <returns>行</returns>
		private int CalculateChildrenPerRow(Size availableSize) {
			// 各行に収まる子要素の数を計算する。
			int childrenPerRow;
			if (double.IsPositiveInfinity(availableSize.Width)) {
				childrenPerRow = this.Children.Count;
			} else {
				childrenPerRow = Math.Max(1, (int)Math.Floor(availableSize.Width / this.ChildSize));
			}

			return childrenPerRow;
		}

		#endregion

		#region IScrollInfo実装
		// 参考: http://blogs.msdn.com/bencon/

		private void UpdateScrollInfo(Size availableSize) {
			// 項目数の確認
			var itemsControl = ItemsControl.GetItemsOwner(this);
			var itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;

			var extent = this.CalculateExtent(availableSize, itemCount);
			// 更新範囲
			if (extent != this._extent) {
				this._extent = extent;
				this.ScrollOwner?.InvalidateScrollInfo();
			}

			// viewportの更新
			if (availableSize != this._viewport) {
				this._viewport = availableSize;
				this.ScrollOwner?.InvalidateScrollInfo();
			}
		}

		public ScrollViewer ScrollOwner {
			get;
			set;
		}

		public bool CanHorizontallyScroll {
			get;
			set;
		}

		public bool CanVerticallyScroll {
			get;
			set;
		}

		public double HorizontalOffset {
			get {
				return this._offset.X;
			}
		}

		public double VerticalOffset {
			get {
				return this._offset.Y;
			}
		}

		public double ExtentHeight {
			get {
				return this._extent.Height;
			}
		}

		public double ExtentWidth {
			get {
				return this._extent.Width;
			}
		}

		public double ViewportHeight {
			get {
				return this._viewport.Height;
			}
		}

		public double ViewportWidth {
			get {
				return this._viewport.Width;
			}
		}

		public void LineUp() {
			this.SetVerticalOffset(this.VerticalOffset - 10);
		}

		public void LineDown() {
			this.SetVerticalOffset(this.VerticalOffset + 10);
		}

		public void PageUp() {
			this.SetVerticalOffset(this.VerticalOffset - this._viewport.Height);
		}

		public void PageDown() {
			this.SetVerticalOffset(this.VerticalOffset + this._viewport.Height);
		}

		public void MouseWheelUp() {
			this.SetVerticalOffset(this.VerticalOffset - 10);
		}

		public void MouseWheelDown() {
			this.SetVerticalOffset(this.VerticalOffset + 10);
		}

		public void LineLeft() {
			throw new InvalidOperationException();
		}

		public void LineRight() {
			throw new InvalidOperationException();
		}

		public Rect MakeVisible(Visual visual, Rect rectangle) {
			return new Rect();
		}

		public void MouseWheelLeft() {
			throw new InvalidOperationException();
		}

		public void MouseWheelRight() {
			throw new InvalidOperationException();
		}

		public void PageLeft() {
			throw new InvalidOperationException();
		}

		public void PageRight() {
			throw new InvalidOperationException();
		}

		public void SetHorizontalOffset(double offset) {
			throw new InvalidOperationException();
		}

		public void SetVerticalOffset(double offset) {
			if (offset < 0 || this._viewport.Height >= this._extent.Height) {
				offset = 0;
			} else {
				if (offset + this._viewport.Height >= this._extent.Height) {
					offset = this._extent.Height - this._viewport.Height;
				}
			}

			this._offset.Y = offset;

			this.ScrollOwner?.InvalidateScrollInfo();

			this._trans.Y = -offset;

			// 子要素の強制具現化
			this.InvalidateMeasure();
		}

		private readonly TranslateTransform _trans = new TranslateTransform();
		private Size _extent = new Size(0, 0);
		private Size _viewport = new Size(0, 0);
		private Point _offset;

		#endregion

	}
}
