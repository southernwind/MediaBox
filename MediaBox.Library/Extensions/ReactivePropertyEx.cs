using System;
using System.Reactive.Disposables;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.Library.Extensions {
	public static class ReactivePropertyEx {
		/// <summary>
		/// ReactiveProperty同士の双方向同期
		/// </summary>
		/// <typeparam name="TSource">同期元型</typeparam>
		/// <typeparam name="TDest">同期先型</typeparam>
		/// <param name="source">同期元</param>
		/// <param name="dest">同期先</param>
		/// <param name="selector1">同期元→同期先変換</param>
		/// <param name="selector2">同期先→同期元変換</param>
		/// <returns></returns>
		public static IDisposable TwoWaySynchronize<TSource, TDest>(
			this IReactiveProperty<TSource> source,
			IReactiveProperty<TDest> dest,
			Func<TSource, TDest> convert,
			Func<TDest, TSource> convertBack) {
			var disposable = new CompositeDisposable();
			var sourceUpdating = false;
			var destUpdating = false;
			source.Subscribe(x => {
				if (sourceUpdating) {
					return;
				}

				destUpdating = true;
				dest.Value = convert(x);
				destUpdating = false;
			}).AddTo(disposable);

			dest.Subscribe(x => {
				if (destUpdating) {
					return;
				}

				sourceUpdating = true;
				source.Value = convertBack(x);
				sourceUpdating = false;
			}).AddTo(disposable);

			return disposable;
		}
	}
}
