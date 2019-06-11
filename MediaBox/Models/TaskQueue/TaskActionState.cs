using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// 進捗率、タスク名更新のためにバックグラウンドメソッドに渡すオブジェクト
	/// </summary>
	internal class TaskActionState {
		/// <summary>
		/// タスク名
		/// </summary>
		public IReactiveProperty<string> TaskName {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 進捗最大値
		/// </summary>
		public IReactiveProperty<double?> ProgressMax {
			get;
		} = new ReactivePropertySlim<double?>();

		/// <summary>
		/// 進捗現在値
		/// </summary>
		public IReactiveProperty<double> ProgressValue {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="taskName">タスク名</param>
		/// <param name="progressMax">進捗最大値</param>
		/// <param name="progressValue">進捗現在地</param>
		public TaskActionState(IReactiveProperty<string> taskName, IReactiveProperty<double?> progressMax, IReactiveProperty<double> progressValue) {
			this.TaskName = taskName;
			this.ProgressMax = progressMax;
			this.ProgressValue = progressValue;
		}
	}
}
