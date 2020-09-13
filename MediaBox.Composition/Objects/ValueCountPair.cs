using System;

namespace SandBeige.MediaBox.Composition.Objects {
	/// <summary>
	/// 値と件数のペア
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public readonly struct ValueCountPair<T> : IEquatable<ValueCountPair<T>> {
		public ValueCountPair(T value, int count) {
			this.Value = value;
			this.Count = count;
		}

		/// <summary>
		/// 値
		/// </summary>
		public T Value {
			get;
		}

		/// <summary>
		/// 件数
		/// </summary>
		public int Count {
			get;
		}

		public bool Equals(ValueCountPair<T> other) {
			if (other.Count == this.Count && other.Count == this.Count) {
				return true;
			}
			return false;
		}
	}
}
