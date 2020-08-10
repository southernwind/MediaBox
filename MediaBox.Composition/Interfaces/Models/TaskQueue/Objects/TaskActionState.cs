using System.Threading;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue.Objects {
	/// <summary>
	/// 進捗率、タスク名更新のためにバックグラウンドメソッドに渡すオブジェクト
	/// </summary>
	public class TaskActionState {
		/// <summary>
		/// タスク名
		/// </summary>
		public IReactiveProperty<string> TaskName {
			get;
		}

		/// <summary>
		/// 進捗最大値
		/// </summary>
		public IReactiveProperty<double?> ProgressMax {
			get;
		}

		/// <summary>
		/// 進捗現在値
		/// </summary>
		public IReactiveProperty<double> ProgressValue {
			get;
		}

		public CancellationToken CancellationToken {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="taskName">タスク名</param>
		/// <param name="progressMax">進捗最大値</param>
		/// <param name="progressValue">進捗現在地</param>
		/// <param name="cancellationToken">キャンセレーショントークン</param>
		public TaskActionState(IReactiveProperty<string> taskName, IReactiveProperty<double?> progressMax, IReactiveProperty<double> progressValue, CancellationToken cancellationToken) {
			this.TaskName = taskName;
			this.ProgressMax = progressMax;
			this.ProgressValue = progressValue;
			this.CancellationToken = cancellationToken;
		}
	}
}
