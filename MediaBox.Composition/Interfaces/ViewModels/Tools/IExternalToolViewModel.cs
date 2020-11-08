using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.ViewModels.Tools {
	public interface IExternalToolViewModel : IViewModelBase {
		/// <summary>
		/// 表示名
		/// </summary>
		IReadOnlyReactiveProperty<string> DisplayName {
			get;
		}

		/// <summary>
		/// コマンド
		/// </summary>
		IReadOnlyReactiveProperty<string> Command {
			get;
		}

		/// <summary>
		/// 引数
		/// </summary>
		IReadOnlyReactiveProperty<string> Arguments {
			get;
		}

		/// <summary>
		/// 対象拡張子
		/// </summary>
		ReadOnlyReactiveCollection<string> TargetExtensions {
			get;
		}

		/// <summary>
		/// 外部ツール起動コマンド
		/// </summary>
		ReactiveCommand<string> StartCommand {
			get;
		}
	}
}