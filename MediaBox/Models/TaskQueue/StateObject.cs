﻿using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// タスク状態オブジェクト
	/// </summary>
	internal class StateObject : ModelBase {
		/// <summary>
		/// タスク名
		/// </summary>
		public IReactiveProperty<string> Name {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// タスク進捗度
		/// </summary>
		public IReactiveProperty<double?> Progress {
			get;
		} = new ReactiveProperty<double?>();
	}
}
