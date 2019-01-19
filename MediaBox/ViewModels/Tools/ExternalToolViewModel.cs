using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Tools;

namespace SandBeige.MediaBox.ViewModels.Tools {
	internal class ExternalToolViewModel : ViewModelBase {
		/// <summary>
		/// 表示名
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> DisplayName {
			get;
			set;
		}

		/// <summary>
		/// コマンド
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> Command {
			get;
			set;
		}

		/// <summary>
		/// 引数
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> Arguments {
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
		/// 外部ツール起動コマンド
		/// </summary>
		/// <param name="filename"></param>
		public ReactiveCommand<string> StartCommand {
			get;
		} = new ReactiveCommand<string>();

		public ExternalToolViewModel(ExternalTool model) {
			this.DisplayName = model.DisplayName.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Command = model.Command.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Arguments = model.Arguments.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.TargetExtensions = model.TargetExtensions.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this.StartCommand.Subscribe(model.Start).AddTo(this.CompositeDisposable);
		}
	}
}
