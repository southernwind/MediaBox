using System;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.ViewModels.Dialog {
	public class RenameWindowViewModel : DialogViewModelBase {
		public static string ParameterNameTitle = nameof(ParameterNameTitle);
		public static string ParameterNameMessage = nameof(ParameterNameMessage);
		public static string ParameterNameInitialText = nameof(ParameterNameInitialText);
		public static string ResultParameterNameText = nameof(ResultParameterNameText);
		/// <summary>
		/// メッセージ
		/// </summary>
		public IReactiveProperty<string> Message {
			get;
		} = new ReactivePropertySlim<string>();

		public IReactiveProperty<string> Text {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// キャンセルコマンド
		/// </summary>
		public ReactiveCommand CancelCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンプリートコマンド
		/// </summary>
		public ReactiveCommand CompleteCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// ウィンドウタイトル
		/// </summary>
		public override string Title {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RenameWindowViewModel() {
			this.CompleteCommand.Subscribe(x => {
				var param = new DialogParameters() {
					{ResultParameterNameText, this.Text.Value }
				};
				this.CloseRequest(ButtonResult.OK, param);
			}).AddTo(this.CompositeDisposable);

			this.CancelCommand.Subscribe(x => {
				this.CloseRequest(ButtonResult.Cancel);
			}).AddTo(this.CompositeDisposable);
		}

		public override void OnDialogOpened(IDialogParameters parameters) {
			base.OnDialogOpened(parameters);
			// タイトル取得
			this.Title = parameters.GetValue<string>(ParameterNameTitle);
			// メッセージ取得
			this.Message.Value = parameters.GetValue<string>(ParameterNameMessage);
			// 初期値取得
			this.Text.Value = parameters.GetValue<string>(ParameterNameInitialText);
		}
	}
}
