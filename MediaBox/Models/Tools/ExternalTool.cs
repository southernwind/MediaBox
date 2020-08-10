using System;
using System.Diagnostics;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Notification;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Notification;
namespace SandBeige.MediaBox.Models.Tools {
	/// <summary>
	/// 外部ツール
	/// </summary>
	public class ExternalTool : ModelBase {
		private readonly ILogging _logging;
		private readonly INotificationManager _notificationManager;
		/// <summary>
		/// 表示名
		/// </summary>
		public IReadOnlyReactiveProperty<string> DisplayName {
			get;
			set;
		}

		/// <summary>
		/// コマンド
		/// </summary>
		public IReadOnlyReactiveProperty<string> Command {
			get;
			set;
		}

		/// <summary>
		/// 引数
		/// </summary>
		public IReadOnlyReactiveProperty<string> Arguments {
			get;
			set;
		}

		/// <summary>
		/// 対象拡張子
		/// </summary>
		public ReadOnlyReactiveCollection<string> TargetExtensions {
			get;
			set;
		}

		/// <summary>
		/// 外部ツール起動
		/// </summary>
		/// <param name="filename"></param>
		public void Start(string filename) {
			this._logging.Log($"外部ツール起動 コマンド[{this.Command.Value}] ファイル名[{filename}] パラメータ[{this.Arguments.Value}]");
			try {
				var process = Process.Start(this.Command.Value, $"\"{filename}\" {this.Arguments.Value}");
				this._logging.Log($"起動 [{process.Id}]");
			} catch (Exception ex) {
				this._logging.Log(ex);
				this._notificationManager.Notify(new Error(null, $"外部ツールの起動に失敗しました。[{this.DisplayName.Value}]"));
			}

		}

		/// <summary>
		/// 設定値から生成するモデル
		/// </summary>
		/// <param name="param"></param>
		public ExternalTool(ExternalToolParams param, ILogging logging, INotificationManager notificationManager) {
			this._logging = logging;
			this._notificationManager = notificationManager;
			this.DisplayName = param.DisplayName.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Command = param.Command.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Arguments = param.Arguments.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.TargetExtensions = param.TargetExtensions.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName.Value}>";
		}
	}
}
