namespace SandBeige.MediaBox.Composition.Enum {
	/// <summary>
	/// 表示モード
	/// </summary>
	public enum DisposeState {
		/// <summary>
		/// Disposeされていない
		/// </summary>
		NotDisposed,
		/// <summary>
		/// Dispose処理中
		/// </summary>
		Disposing,
		/// <summary>
		/// Dispose完了済み
		/// </summary>
		Disposed
	}
}
