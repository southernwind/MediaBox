using System;
using System.Windows.Input;

using Reactive.Bindings;

namespace SandBeige.MediaBox.God {
	/// <summary>
	/// 拡張メソッドクラス
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// マウスホイールイベント→ズームレベルへの変換関数
		/// </summary>
		/// <param name="source">マウスホイールイベントを流すソース</param>
		/// <param name="sourceZoomLevel">基となるズームレベル</param>
		/// <returns>ズームレベル</returns>
		public static IReadOnlyReactiveProperty<int> ToZoomLevel(this IObservable<MouseWheelEventArgs> source, IReactiveProperty<int> sourceZoomLevel = null) {
			var level = sourceZoomLevel ?? new ReactiveProperty<int>();
			source.Subscribe(x => {
				if (x.Delta < 0) {
					if (level.Value <= Controls.Converters.ZoomLevel.MinLevel) {
						x.Handled = true;
						return;
					}

					level.Value -= 1;
				} else {
					if (level.Value >= Controls.Converters.ZoomLevel.MaxLevel) {
						x.Handled = true;
						return;
					}

					level.Value += 1;
				}

				x.Handled = true;
			});
			return level.ToReadOnlyReactiveProperty();
		}
	}
}
