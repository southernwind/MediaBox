using System;
using System.Collections.Concurrent;

namespace SandBeige.MediaBox.God {
	/// <summary>
	/// 同一キーから生成されるインスタンスを唯一つのものにするためのファクトリークラス
	/// </summary>
	/// <typeparam name="TKeyBase">キー基底クラス</typeparam>
	/// <typeparam name="TValueBase">値基底クラス</typeparam>
	public abstract class FactoryBase<TKeyBase, TValueBase> where TValueBase : class, IDisposable {
		/// <summary>
		/// プール
		/// </summary>
		protected readonly ConcurrentDictionary<TKeyBase, WeakReference<TValueBase>> Pool;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected FactoryBase() {
			this.Pool = new ConcurrentDictionary<TKeyBase, WeakReference<TValueBase>>(6, 10000);
		}

		/// <summary>
		/// 値作成
		/// </summary>
		/// <typeparam name="TKey">キー</typeparam>
		/// <typeparam name="TValue">値</typeparam>
		/// <param name="key">キー</param>
		/// <returns>値</returns>
		protected TValue Create<TKey, TValue>(TKey key, Func<TKey, TValueBase> createFunc = null)
			where TKey : TKeyBase
			where TValue : TValueBase {
			if (createFunc == null) {
				createFunc = this.CreateInstance<TKey, TValue>;
			}
			// Poolにキーがなかった場合、CreateInstanceを使って値を生成する
			// その後、TryGetTargetを呼ぶまでにGCされると参照が消えるため、
			// 別の変数でGC対象にならないように参照しておく
			if (key == null) {
				return default;
			}
			TValueBase value;
			var wr = this.Pool.GetOrAdd(
				key,
				k => {
					value = createFunc((TKey)k);
					return new WeakReference<TValueBase>(value);
				});
			// 弱参照が取得出来たら値を取り出してみる
			if (wr.TryGetTarget(out var mf)) {
				// 取り出せた場合(GC未実施)
				return (TValue)mf;
			}

			// 取り出せなかった場合(GC済み)
			// 再度値を生成して登録しておく
			var instance = createFunc(key);
			wr.SetTarget(instance);
			return (TValue)instance;
		}

		/// <summary>
		/// キーからインスタンスを作成するための関数
		/// </summary>
		/// <typeparam name="TKey">キー</typeparam>
		/// <typeparam name="TValue">値</typeparam>
		/// <param name="key">キー</param>
		/// <returns>値</returns>
		protected abstract TValueBase CreateInstance<TKey, TValue>(TKey key)
			where TKey : TKeyBase
			where TValue : TValueBase;
	}
}
