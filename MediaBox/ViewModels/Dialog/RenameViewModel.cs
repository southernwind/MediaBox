using System;

using Livet.Messaging.Windows;

using Reactive.Bindings;

namespace SandBeige.MediaBox.ViewModels.Dialog {
	internal class RenameViewModel : ViewModelBase {
		/// <summary>
		/// メッセージ
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

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
		/// 編集が完了したか否か
		/// </summary>
		public bool Completed {
			get;
			private set;
		}

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
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="message">メッセージ</param>
		/// <param name="initialText">初期値</param>
		public RenameViewModel(string title, string message, string initialText) {
			this.Title.Value = title;
			this.Message.Value = message;
			this.Text.Value = initialText;

			this.CompleteCommand.Subscribe(x => {
				this.Completed = true;
				this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
			});

			this.CancelCommand.Subscribe(x => {
				this.Completed = false;
				this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
			});
		}
	}
}
