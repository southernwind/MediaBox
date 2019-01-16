using System.Diagnostics;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Objects {
	public class ExternalTool {
		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// コマンド
		/// </summary>
		public IReactiveProperty<string> Command {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// 引数
		/// </summary>
		public IReactiveProperty<string> Arguments {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// 対象拡張子
		/// </summary>
		public IReactiveProperty<string[]> TargetExtensions {
			get;
			set;
		} = new ReactiveProperty<string[]>();

		/// <summary>
		/// 外部ツール起動
		/// </summary>
		/// <param name="filename"></param>
		public void Start(string filename) {
			Process.Start(this.Command.Value, $"{filename} {this.Arguments.Value}");
		}
	}
}
