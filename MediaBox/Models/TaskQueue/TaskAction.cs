﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// タスク定義
	/// </summary>
	/// <remarks>
	/// タスクが完了すると<see cref="OnTaskCompleted"/>が流れる。
	/// </remarks>
	internal class TaskAction : IComparable {
		/// <summary>
		/// 実行するタスク
		/// </summary>
		private readonly Action _action;

		/// <summary>
		/// タスク完了通知用サブジェクト
		/// </summary>
		private readonly Subject<Unit> _onTaskCompletedSubject = new Subject<Unit>();

		/// <summary>
		/// タスク完了通知
		/// </summary>
		public IObservable<Unit> OnTaskCompleted {
			get {
				return this._onTaskCompletedSubject.AsObservable();
			}
		}

		/// <summary>
		/// タスク優先度
		/// </summary>
		public Priority Priority {
			get;
		}

		/// <summary>
		/// タスクキャンセル用トークン
		/// </summary>
		public CancellationToken Token {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="action">タスク</param>
		/// <param name="priority">タスク優先度</param>
		/// <param name="token">キャンセルトークン</param>
		public TaskAction(Action action, Priority priority, CancellationToken token) {
			this._action = action;
			this.Priority = priority;
			this.Token = token;
		}

		/// <summary>
		/// タスク実行
		/// </summary>
		public void Do() {
			this._action();
			this._onTaskCompletedSubject.OnNext(Unit.Default);
		}

		/// <summary>
		/// 比較
		/// </summary>
		/// <param name="obj">比較対象</param>
		/// <returns>比較結果</returns>
		public int CompareTo(object obj) {
			if (obj is TaskAction ta) {
				return this.Priority.CompareTo(ta.Priority);
			}
			return -1;
		}
	}
}
