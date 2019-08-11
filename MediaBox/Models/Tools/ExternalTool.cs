using System.Diagnostics;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Models.Tools {
	/// <summary>
	/// 外部ツール
	/// </summary>
	internal class ExternalTool : ModelBase {
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
			this.Logging.Log($"外部ツール起動 コマンド[{this.Command.Value}] ファイル名[{filename}] パラメータ[{this.Arguments.Value}]");
			var process = Process.Start(this.Command.Value, $"\"{filename}\" {this.Arguments.Value}");
			this.Logging.Log($"起動 [{process.Id}]");
		}

		/// <summary>
		/// 設定値から生成するモデル
		/// </summary>
		/// <param name="param"></param>
		public ExternalTool(ExternalToolParams param) {
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
