using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// タスク状態オブジェクト
	/// </summary>
	internal class StateObject {
		/// <summary>
		/// タスク名
		/// </summary>
		public IReactiveProperty<string> Name {
			get;
		} = new ReactiveProperty<string>();
	}
}
