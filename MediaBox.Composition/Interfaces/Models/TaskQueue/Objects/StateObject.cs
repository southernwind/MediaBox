using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue.Objects {
	/// <summary>
	/// タスク状態オブジェクト
	/// </summary>
	public class StateObject {
		/// <summary>
		/// タスク名
		/// </summary>
		public IReactiveProperty<string> Name {
			get;
		} = new ReactiveProperty<string>();
	}
}
