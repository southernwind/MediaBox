// https://archive.codeplex.com/?p=uhimaniavwp

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SandBeige.MediaBox.Controls.Controls {
	#region VirtualizingWrapPanel
	/// <summary>
	/// 子要素を仮想化する <see cref="WrapPanel"/>。
	/// </summary>
	public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo {
		#region ItemSize

		#region ItemWidth

		/// <summary>
		/// <see cref="ItemWidth"/> 依存関係プロパティの識別子。
		/// </summary>
		public static readonly DependencyProperty ItemWidthProperty =
			DependencyProperty.Register(
				"ItemWidth",
				typeof(double),
				typeof(VirtualizingWrapPanel),
				new FrameworkPropertyMetadata(
					double.NaN,
					FrameworkPropertyMetadataOptions.AffectsMeasure
				),
				IsWidthHeightValid
			);

		/// <summary>
		/// VirtualizingWrapPanel 内に含まれているすべての項目の幅を
		/// 指定する値を取得、または設定する。
		/// </summary>
		[TypeConverter(typeof(LengthConverter)), Category("共通")]
		public double ItemWidth {
			get {
				return (double)this.GetValue(ItemWidthProperty);
			}
			set {
				this.SetValue(ItemWidthProperty, value);
			}
		}

		#endregion

		#region ItemHeight

		/// <summary>
		/// <see cref="ItemHeight"/> 依存関係プロパティの識別子。
		/// </summary>
		public static readonly DependencyProperty ItemHeightProperty =
			DependencyProperty.Register(
				"ItemHeight",
				typeof(double),
				typeof(VirtualizingWrapPanel),
				new FrameworkPropertyMetadata(
					double.NaN,
					FrameworkPropertyMetadataOptions.AffectsMeasure
				),
				IsWidthHeightValid
			);

		/// <summary>
		/// VirtualizingWrapPanel 内に含まれているすべての項目の高さを
		/// 指定する値を取得、または設定する。
		/// </summary>
		[TypeConverter(typeof(LengthConverter)), Category("共通")]
		public double ItemHeight {
			get {
				return (double)this.GetValue(ItemHeightProperty);
			}
			set {
				this.SetValue(ItemHeightProperty, value);
			}
		}

		#endregion

		#region IsWidthHeightValid
		/// <summary>
		/// <see cref="ItemWidth"/>, <see cref="ItemHeight"/> に設定された値が
		/// 有効かどうかを検証するコールバック。
		/// </summary>
		/// <param name="value">プロパティに設定された値。</param>
		/// <returns>値が有効な場合は true、無効な場合は false。</returns>
		private static bool IsWidthHeightValid(object value) {
			var d = (double)value;
			return double.IsNaN(d) || (d >= 0 && !double.IsPositiveInfinity(d));
		}
		#endregion

		#endregion

		#region Orientation

		/// <summary>
		/// <see cref="Orientation"/> 依存関係プロパティの識別子。
		/// </summary>
		public static readonly DependencyProperty OrientationProperty =
			WrapPanel.OrientationProperty.AddOwner(
				typeof(VirtualizingWrapPanel),
				new FrameworkPropertyMetadata(
					Orientation.Horizontal,
					FrameworkPropertyMetadataOptions.AffectsMeasure,
					OnOrientationChanged
				)
			);

		/// <summary>
		/// 子コンテンツが配置される方向を指定する値を取得、または設定する。
		/// </summary>
		[Category("共通")]
		public Orientation Orientation {
			get {
				return (Orientation)this.GetValue(OrientationProperty);
			}
			set {
				this.SetValue(OrientationProperty, value);
			}
		}

		/// <summary>
		/// <see cref="Orientation"/> 依存関係プロパティが変更されたときに呼び出されるコールバック。
		/// </summary>
		/// <param name="d">プロパティの値が変更された <see cref="System.Windows.DependencyObject"/>。</param>
		/// <param name="e">このプロパティの有効値に対する変更を追跡するイベントによって発行されるイベントデータ。</param>
		private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (!(d is VirtualizingWrapPanel panel)) {
				return;
			}

			panel._offset = default;
			panel.InvalidateMeasure();
		}

		#endregion

		#region MeasureOverride, ArrangeOverride

		/// <summary>
		/// 指定したインデックスのアイテムの位置およびサイズを記憶するディクショナリ。
		/// </summary>
		private readonly Dictionary<int, Rect> _containerLayouts = new Dictionary<int, Rect>();

		/// <summary>
		/// 子要素に必要なレイアウトのサイズを測定し、パネルのサイズを決定する。
		/// </summary>
		/// <param name="availableSize">子要素に与えることができる使用可能なサイズ。</param>
		/// <returns>レイアウト時にこのパネルが必要とするサイズ。</returns>
		protected override Size MeasureOverride(Size availableSize) {
			this._containerLayouts.Clear();

			var isAutoWidth = double.IsNaN(this.ItemWidth);
			var isAutoHeight = double.IsNaN(this.ItemHeight);
			var childAvailable = new Size(isAutoWidth ? double.PositiveInfinity : this.ItemWidth, isAutoHeight ? double.PositiveInfinity : this.ItemHeight);
			var isHorizontal = this.Orientation == Orientation.Horizontal;

			var childrenCount = this.InternalChildren.Count;

			var itemsControl = ItemsControl.GetItemsOwner(this);
			if (itemsControl != null) {
				childrenCount = itemsControl.Items.Count;
			}

			var generator = new ChildGenerator(this);

			var x = 0.0;
			var y = 0.0;
			var lineSize = default(Size);
			var maxSize = default(Size);

			for (var i = 0; i < childrenCount; i++) {
				var childSize = this.ContainerSizeForIndex(i);

				// ビューポートとの交差判定用に仮サイズで x, y を調整
				var isWrapped = isHorizontal ?
					lineSize.Width + childSize.Width > availableSize.Width :
					lineSize.Height + childSize.Height > availableSize.Height;
				if (isWrapped) {
					x = isHorizontal ? 0 : x + lineSize.Width;
					y = isHorizontal ? y + lineSize.Height : 0;
				}

				// 子要素がビューポート内であれば子要素を生成しサイズを再計測
				var itemRect = new Rect(x, y, childSize.Width, childSize.Height);
				var viewportRect = new Rect(this._offset, availableSize);
				if (itemRect.IntersectsWith(viewportRect)) {
					var child = generator.GetOrCreateChild(i);
					child.Measure(childAvailable);
					childSize = this.ContainerSizeForIndex(i);
				}

				// 確定したサイズを記憶
				this._containerLayouts[i] = new Rect(x, y, childSize.Width, childSize.Height);

				// lineSize, maxSize を計算
				isWrapped = isHorizontal ?
					lineSize.Width + childSize.Width > availableSize.Width :
					lineSize.Height + childSize.Height > availableSize.Height;
				if (isWrapped) {
					maxSize.Width = isHorizontal ? Math.Max(lineSize.Width, maxSize.Width) : maxSize.Width + lineSize.Width;
					maxSize.Height = isHorizontal ? maxSize.Height + lineSize.Height : Math.Max(lineSize.Height, maxSize.Height);
					lineSize = childSize;

					isWrapped = isHorizontal ?
						childSize.Width > availableSize.Width :
						childSize.Height > availableSize.Height;
					if (isWrapped) {
						maxSize.Width = isHorizontal ? Math.Max(childSize.Width, maxSize.Width) : maxSize.Width + childSize.Width;
						maxSize.Height = isHorizontal ? maxSize.Height + childSize.Height : Math.Max(childSize.Height, maxSize.Height);
						lineSize = default;
					}
				} else {
					lineSize.Width = isHorizontal ? lineSize.Width + childSize.Width : Math.Max(childSize.Width, lineSize.Width);
					lineSize.Height = isHorizontal ? Math.Max(childSize.Height, lineSize.Height) : lineSize.Height + childSize.Height;
				}

				x = isHorizontal ? lineSize.Width : maxSize.Width;
				y = isHorizontal ? maxSize.Height : lineSize.Height;
			}

			maxSize.Width = isHorizontal ? Math.Max(lineSize.Width, maxSize.Width) : maxSize.Width + lineSize.Width;
			maxSize.Height = isHorizontal ? maxSize.Height + lineSize.Height : Math.Max(lineSize.Height, maxSize.Height);

			this._extent = maxSize;
			this._viewport = availableSize;

			generator.CleanupChildren();
			generator.Dispose();

			this.ScrollOwner?.InvalidateScrollInfo();

			return maxSize;
		}

		#region ChildGenerator
		/// <summary>
		/// <see cref="VirtualizingWrapPanel"/> のアイテムを管理する。
		/// </summary>
		private class ChildGenerator : IDisposable {
			#region fields

			/// <summary>
			/// アイテムを生成する対象の <see cref="VirtualizingWrapPanel"/>。
			/// </summary>
			private readonly VirtualizingWrapPanel _owner;

			/// <summary>
			/// <see cref="_owner"/> の <see cref="ItemContainerGenerator"/>。
			/// </summary>
			private readonly IItemContainerGenerator _generator;

			/// <summary>
			/// <see cref="_generator"/> の生成プロセスの有効期間を追跡するオブジェクト。
			/// </summary>
			private IDisposable _generatorTracker;

			/// <summary>
			/// 表示範囲内にある最初の要素のインデックス。
			/// </summary>
			private int _firstGeneratedIndex;

			/// <summary>
			/// 表示範囲内にある最後の要素のインデックス。
			/// </summary>
			private int _lastGeneratedIndex;

			/// <summary>
			/// 次に生成される要素の <see cref="Panel.InternalChildren"/> 内のインデックス。
			/// </summary>
			private int _currentGenerateIndex;

			#endregion

			#region _ctor

			/// <summary>
			/// <see cref="ChildGenerator"/> の新しいインスタンスを生成する。
			/// </summary>
			/// <param name="owner">アイテムを生成する対象の <see cref="VirtualizingWrapPanel"/>。</param>
			public ChildGenerator(VirtualizingWrapPanel owner) {
				this._owner = owner;

				// ItemContainerGenerator 取得前に InternalChildren にアクセスしないと null になる
				_ = owner.InternalChildren.Count;
				this._generator = owner.ItemContainerGenerator;
			}

			/// <summary>
			/// <see cref="ChildGenerator"/> のインスタンスを破棄する。
			/// </summary>
			~ChildGenerator() {
				this.Dispose();
			}

			/// <summary>
			/// アイテムの生成を終了する。
			/// </summary>
			public void Dispose() {
				this._generatorTracker?.Dispose();
			}

			#endregion

			#region GetOrCreateChild

			/// <summary>
			/// アイテムの生成を開始する。
			/// </summary>
			/// <param name="index">アイテムのインデックス。</param>
			private void BeginGenerate(int index) {
				this._firstGeneratedIndex = index;
				var startPos = this._generator.GeneratorPositionFromIndex(index);
				this._currentGenerateIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;
				this._generatorTracker = this._generator.StartAt(startPos, GeneratorDirection.Forward, true);
			}

			/// <summary>
			/// 必要に応じてアイテムを生成し、指定したインデックスのアイテムを取得する。
			/// </summary>
			/// <param name="index">取得するアイテムのインデックス。</param>
			/// <returns>指定したインデックスのアイテム。</returns>
			public UIElement GetOrCreateChild(int index) {
				if (this._generator == null) {
					return this._owner.InternalChildren[index];
				}

				if (this._generatorTracker == null) {
					this.BeginGenerate(index);
				}

				var child = (UIElement)this._generator.GenerateNext(out var newlyRealized);
				if (newlyRealized) {
					if (this._currentGenerateIndex >= this._owner.InternalChildren.Count) {
						this._owner.AddInternalChild(child);
					} else {
						this._owner.InsertInternalChild(this._currentGenerateIndex, child);
					}

					this._generator.PrepareItemContainer(child);
				}

				this._lastGeneratedIndex = index;
				this._currentGenerateIndex++;

				return child;
			}

			#endregion

			#region CleanupChildren
			/// <summary>
			/// 表示範囲外のアイテムを削除する。
			/// </summary>
			public void CleanupChildren() {
				if (this._generator == null) {
					return;
				}

				var children = this._owner.InternalChildren;

				for (var i = children.Count - 1; i >= 0; i--) {
					var childPos = new GeneratorPosition(i, 0);
					var index = this._generator.IndexFromGeneratorPosition(childPos);
					if (index < this._firstGeneratedIndex || index > this._lastGeneratedIndex) {
						this._generator.Remove(childPos, 1);
						this._owner.RemoveInternalChildRange(i, 1);
					}
				}
			}
			#endregion
		}
		#endregion

		/// <summary>
		/// 子要素を配置し、パネルのサイズを決定する。
		/// </summary>
		/// <param name="finalSize">パネル自体と子要素を配置するために使用する親の末尾の領域。</param>
		/// <returns>使用する実際のサイズ。</returns>
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in this.InternalChildren) {
				var gen = (ItemContainerGenerator)this.ItemContainerGenerator;
				var index = gen?.IndexFromContainer(child) ?? this.InternalChildren.IndexOf(child);
				if (this._containerLayouts.ContainsKey(index)) {
					var layout = this._containerLayouts[index];
					layout.Offset(this._offset.X * -1, this._offset.Y * -1);
					child.Arrange(layout);
				}
			}

			return finalSize;
		}

		#endregion

		#region ContainerSizeForIndex

		/// <summary>
		/// 直前にレイアウトした要素のサイズ。
		/// </summary>
		/// <remarks>
		/// <see cref="DataTemplate"/> 使用時、全要素のサイズが一致することを前提に、
		/// 要素のサイズの推定に使用する。
		/// </remarks>
		private Size _prevSize = new Size(16, 16);

		/// <summary>
		/// 指定したインデックスに対するアイテムのサイズを、実際にアイテムを生成せずに推定する。
		/// </summary>
		/// <param name="index">アイテムのインデックス。</param>
		/// <returns>指定したインデックスに対するアイテムの推定サイズ。</returns>
		private Size ContainerSizeForIndex(int index) {
			var getSize = new Func<int, Size>(idx => {
				UIElement item = null;
				var itemsOwner = ItemsControl.GetItemsOwner(this);
				var generator = this.ItemContainerGenerator as ItemContainerGenerator;

				if (itemsOwner == null || generator == null) {
					// VirtualizingWrapPanel 単体で使用されている場合、自身のアイテムを返す
					if (this.InternalChildren.Count > idx) {
						item = this.InternalChildren[idx];
					}
				} else {
					// generator がアイテムを未生成の場合、Items が使えればそちらを使う
					if (generator.ContainerFromIndex(idx) != null) {
						item = generator.ContainerFromIndex(idx) as UIElement;
					} else if (itemsOwner.Items.Count > idx) {
						item = itemsOwner.Items[idx] as UIElement;
					}
				}

				if (item != null) {
					// アイテムのサイズが測定済みであればそのサイズを返す
					if (item.IsMeasureValid) {
						return item.DesiredSize;
					}

					// アイテムのサイズが未測定の場合、推奨値を使う
					if (item is FrameworkElement i) {
						return new Size(i.Width, i.Height);
					}
				}

				// 前回の測定値があればそちらを使う
				if (this._containerLayouts.ContainsKey(idx)) {
					return this._containerLayouts[idx].Size;
				}

				// 有効なサイズが取得できなかった場合、直前のアイテムのサイズを返す
				return this._prevSize;
			});

			var size = getSize(index);

			// ItemWidth, ItemHeight が指定されていれば調整する
			if (!double.IsNaN(this.ItemWidth)) {
				size.Width = this.ItemWidth;
			}

			if (!double.IsNaN(this.ItemHeight)) {
				size.Height = this.ItemHeight;
			}

			return this._prevSize = size;
		}

		#endregion

		#region OnItemsChanged
		/// <summary>
		/// このパネルの <see cref="System.Windows.Controls.ItemsControl"/> に関連付けられている
		/// <see cref="System.Windows.Controls.ItemsControl.Items"/> コレクションが変更されたときに
		/// 呼び出されるコールバック。
		/// </summary>
		/// <param name="sender">イベントを発生させた <see cref="System.Object"/></param>
		/// <param name="args">イベントデータ。</param>
		/// <remarks>
		/// <see cref="System.Windows.Controls.ItemsControl.Items"/> が変更された際
		/// <see cref="System.Windows.Controls.Panel.InternalChildren"/> にも反映する。
		/// </remarks>
		protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args) {
			switch (args.Action) {
				case NotifyCollectionChangedAction.Remove:
				case NotifyCollectionChangedAction.Replace:
				case NotifyCollectionChangedAction.Move:
					this.RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
					break;
			}
		}
		#endregion

		#region IScrollInfo Members

		#region Extent

		/// <summary>
		/// エクステントのサイズ。
		/// </summary>
		private Size _extent;

		/// <summary>
		/// エクステントの縦幅を取得する。
		/// </summary>
		public double ExtentHeight {
			get {
				return this._extent.Height;
			}
		}

		/// <summary>
		/// エクステントの横幅を取得する。
		/// </summary>
		public double ExtentWidth {
			get {
				return this._extent.Width;
			}
		}

		#endregion Extent

		#region Viewport

		/// <summary>
		/// ビューポートのサイズ。
		/// </summary>
		private Size _viewport;

		/// <summary>
		/// このコンテンツに対するビューポートの縦幅を取得する。
		/// </summary>
		public double ViewportHeight {
			get {
				return this._viewport.Height;
			}
		}

		/// <summary>
		/// このコンテンツに対するビューポートの横幅を取得する。
		/// </summary>
		public double ViewportWidth {
			get {
				return this._viewport.Width;
			}
		}

		#endregion

		#region Offset

		/// <summary>
		/// スクロールしたコンテンツのオフセット。
		/// </summary>
		private Point _offset;

		/// <summary>
		/// スクロールしたコンテンツの水平オフセットを取得する。
		/// </summary>
		public double HorizontalOffset {
			get {
				return this._offset.X;
			}
		}

		/// <summary>
		/// スクロールしたコンテンツの垂直オフセットを取得する。
		/// </summary>
		public double VerticalOffset {
			get {
				return this._offset.Y;
			}
		}

		#endregion

		#region ScrollOwner
		/// <summary>
		/// スクロール動作を制御する <see cref="System.Windows.Controls.ScrollViewer"/> 要素を
		/// 取得、または設定する。
		/// </summary>
		public ScrollViewer ScrollOwner {
			get; set;
		}
		#endregion

		#region CanHorizontallyScroll
		/// <summary>
		/// 水平軸のスクロールが可能かどうかを示す値を取得、または設定する。
		/// </summary>
		public bool CanHorizontallyScroll {
			get; set;
		}
		#endregion

		#region CanVerticallyScroll
		/// <summary>
		/// 垂直軸のスクロールが可能かどうかを示す値を取得、または設定する。
		/// </summary>
		public bool CanVerticallyScroll {
			get; set;
		}
		#endregion

		#region LineUp
		/// <summary>
		/// コンテンツ内を 1 論理単位ずつ上にスクロールする。
		/// </summary>
		public void LineUp() {
			this.SetVerticalOffset(this.VerticalOffset - SystemParameters.ScrollHeight);
		}
		#endregion

		#region LineDown
		/// <summary>
		/// コンテンツ内を 1 論理単位ずつ下にスクロールする。
		/// </summary>
		public void LineDown() {
			this.SetVerticalOffset(this.VerticalOffset + SystemParameters.ScrollHeight);
		}
		#endregion

		#region LineLeft
		/// <summary>
		/// コンテンツ内を 1 論理単位ずつ左にスクロールする。
		/// </summary>
		public void LineLeft() {
			this.SetHorizontalOffset(this.HorizontalOffset - SystemParameters.ScrollWidth);
		}
		#endregion

		#region LineRight
		/// <summary>
		/// コンテンツ内を 1 論理単位ずつ右にスクロールする。
		/// </summary>
		public void LineRight() {
			this.SetHorizontalOffset(this.HorizontalOffset + SystemParameters.ScrollWidth);
		}
		#endregion

		#region PageUp
		/// <summary>
		/// コンテンツ内を 1 ページずつ上にスクロールする。
		/// </summary>
		public void PageUp() {
			this.SetVerticalOffset(this.VerticalOffset - this._viewport.Height);
		}
		#endregion

		#region PageDown
		/// <summary>
		/// コンテンツ内を 1 ページずつ下にスクロールする。
		/// </summary>
		public void PageDown() {
			this.SetVerticalOffset(this.VerticalOffset + this._viewport.Height);
		}
		#endregion

		#region PageLeft
		/// <summary>
		/// コンテンツ内を 1 ページずつ左にスクロールする。
		/// </summary>
		public void PageLeft() {
			this.SetHorizontalOffset(this.HorizontalOffset - this._viewport.Width);
		}
		#endregion

		#region PageRight
		/// <summary>
		/// コンテンツ内を 1 ページずつ右にスクロールする。
		/// </summary>
		public void PageRight() {
			this.SetHorizontalOffset(this.HorizontalOffset + this._viewport.Width);
		}
		#endregion

		#region MouseWheelUp
		/// <summary>
		/// ユーザがマウスのホイールボタンをクリックした後に、コンテンツ内を上にスクロールする。
		/// </summary>
		public void MouseWheelUp() {
			this.SetVerticalOffset(this.VerticalOffset - (SystemParameters.ScrollHeight * SystemParameters.WheelScrollLines));
		}
		#endregion

		#region MouseWheelDown
		/// <summary>
		/// ユーザがマウスのホイールボタンをクリックした後に、コンテンツ内を下にスクロールする。
		/// </summary>
		public void MouseWheelDown() {
			this.SetVerticalOffset(this.VerticalOffset + (SystemParameters.ScrollHeight * SystemParameters.WheelScrollLines));
		}
		#endregion

		#region MouseWheelLeft
		/// <summary>
		/// ユーザがマウスのホイールボタンをクリックした後に、コンテンツ内を左にスクロールする。
		/// </summary>
		public void MouseWheelLeft() {
			this.SetHorizontalOffset(this.HorizontalOffset - (SystemParameters.ScrollWidth * SystemParameters.WheelScrollLines));
		}
		#endregion

		#region MouseWheelRight
		/// <summary>
		/// ユーザがマウスのホイールボタンをクリックした後に、コンテンツ内を右にスクロールする。
		/// </summary>
		public void MouseWheelRight() {
			this.SetHorizontalOffset(this.HorizontalOffset + (SystemParameters.ScrollWidth * SystemParameters.WheelScrollLines));
		}
		#endregion

		#region MakeVisible
		/// <summary>
		/// <see cref="System.Windows.Media.Visual"/> オブジェクトの座標空間が表示されるまで、
		/// コンテンツを強制的にスクロールする。
		/// </summary>
		/// <param name="visual">表示可能になる <see cref="System.Windows.Media.Visual"/>。</param>
		/// <param name="rectangle">表示する座標空間を識別する外接する四角形。</param>
		/// <returns>表示される <see cref="System.Windows.Rect"/>。</returns>
		public Rect MakeVisible(Visual visual, Rect rectangle) {
			var idx = this.InternalChildren.IndexOf(visual as UIElement);

			if (this.ItemContainerGenerator is IItemContainerGenerator generator) {
				var pos = new GeneratorPosition(idx, 0);
				idx = generator.IndexFromGeneratorPosition(pos);
			}

			if (idx < 0) {
				return Rect.Empty;
			}

			if (!this._containerLayouts.ContainsKey(idx)) {
				return Rect.Empty;
			}

			var layout = this._containerLayouts[idx];

			if (this.HorizontalOffset + this.ViewportWidth < layout.X + layout.Width) {
				this.SetHorizontalOffset(layout.X + layout.Width - this.ViewportWidth);
			}

			if (layout.X < this.HorizontalOffset) {
				this.SetHorizontalOffset(layout.X);
			}

			if (this.VerticalOffset + this.ViewportHeight < layout.Y + layout.Height) {
				this.SetVerticalOffset(layout.Y + layout.Height - this.ViewportHeight);
			}

			if (layout.Y < this.VerticalOffset) {
				this.SetVerticalOffset(layout.Y);
			}

			layout.Width = Math.Min(this.ViewportWidth, layout.Width);
			layout.Height = Math.Min(this.ViewportHeight, layout.Height);

			return layout;
		}
		#endregion

		#region SetHorizontalOffset
		/// <summary>
		/// 水平オフセットの値を設定する。
		/// </summary>
		/// <param name="offset">包含するビューポートからのコンテンツの水平方向オフセットの程度。</param>
		public void SetHorizontalOffset(double offset) {
			if (offset < 0 || this.ViewportWidth >= this.ExtentWidth) {
				offset = 0;
			} else {
				if (offset + this.ViewportWidth >= this.ExtentWidth) {
					offset = this.ExtentWidth - this.ViewportWidth;
				}
			}

			this._offset.X = offset;

			this.ScrollOwner?.InvalidateScrollInfo();

			this.InvalidateMeasure();
		}
		#endregion

		#region SetVerticalOffset
		/// <summary>
		/// 垂直オフセットの値を設定する。
		/// </summary>
		/// <param name="offset">包含するビューポートからの垂直方向オフセットの程度。</param>
		public void SetVerticalOffset(double offset) {
			if (offset < 0 || this.ViewportHeight >= this.ExtentHeight) {
				offset = 0;
			} else {
				if (offset + this.ViewportHeight >= this.ExtentHeight) {
					offset = this.ExtentHeight - this.ViewportHeight;
				}
			}

			this._offset.Y = offset;

			this.ScrollOwner?.InvalidateScrollInfo();

			this.InvalidateMeasure();
		}
		#endregion

		#endregion
	}
	#endregion
}
