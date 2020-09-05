using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Tool;

namespace SandBeige.MediaBox.ViewModels.Tools {
	/// <summary>
	/// 外部ツールViewModel
	/// </summary>
	public class ExternalToolViewModel : ViewModelBase {
		/// <summary>
		/// 表示名
		/// </summary>
		public IReadOnlyReactiveProperty<string> DisplayName {
			get;
		}

		/// <summary>
		/// コマンド
		/// </summary>
		public IReadOnlyReactiveProperty<string> Command {
			get;
		}

		/// <summary>
		/// 引数
		/// </summary>
		public IReadOnlyReactiveProperty<string> Arguments {
			get;
		}

		/// <summary>
		/// 対象拡張子
		/// </summary>
		public ReadOnlyReactiveCollection<string> TargetExtensions {
			get;
		}

		/// <summary>
		/// 外部ツール起動コマンド
		/// </summary>
		public ReactiveCommand<string> StartCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public ExternalToolViewModel(IExternalTool model) {
			this.ModelForToString = model;
			this.DisplayName = model.DisplayName.ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
			this.Command = model.Command.ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
			this.Arguments = model.Arguments.ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
			this.TargetExtensions = model.TargetExtensions.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this.StartCommand.Subscribe(model.Start).AddTo(this.CompositeDisposable);
		}
	}
}
