using System;
using System.Linq;
using System.Windows;

using Livet.Messaging.Windows;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.ViewModels.Dialog {
	internal class DialogViewModel : ViewModelBase {
		/// <summary>
		/// メッセージ
		/// </summary>
		public ReactiveProperty<string> Title {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// メッセージ
		/// </summary>
		public ReactiveProperty<string> Message {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// ボタンリスト
		/// </summary>
		public ReactiveCollection<ButtonParam> ButtonList {
			get;
		} = new ReactiveCollection<ButtonParam>();

		/// <summary>
		/// 選択結果
		/// </summary>
		public ReactiveProperty<MessageBoxResult> Result {
			get;
		} = new ReactiveProperty<MessageBoxResult>();

		/// <summary>
		/// 選択コマンド
		/// </summary>
		public ReactiveCommand<MessageBoxResult> SelectCommand {
			get;
		} = new ReactiveCommand<MessageBoxResult>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="message">メッセージ</param>
		/// <param name="button">ボタン種別</param>
		/// <param name="defaultButton">初期選択中ボタン</param>
		public DialogViewModel(string title, string message, MessageBoxButton button, MessageBoxResult defaultButton = MessageBoxResult.None) {
			this.Title.Value = title;
			this.Message.Value = message;
			if (new[] { MessageBoxButton.OK, MessageBoxButton.OKCancel }.Contains(button)) {
				this.ButtonList.Add(
					new ButtonParam("OK", MessageBoxResult.OK, defaultButton == MessageBoxResult.OK)
				);
			}
			if (new[] { MessageBoxButton.YesNo, MessageBoxButton.YesNoCancel }.Contains(button)) {
				this.ButtonList.AddRange(
					new ButtonParam("Yes", MessageBoxResult.Yes, defaultButton == MessageBoxResult.Yes),
					new ButtonParam("No", MessageBoxResult.No, defaultButton == MessageBoxResult.No)
				);
			}
			if (new[] { MessageBoxButton.OKCancel, MessageBoxButton.YesNoCancel }.Contains(button)) {
				this.ButtonList.Add(
					new ButtonParam("Cancel", MessageBoxResult.Cancel, defaultButton == MessageBoxResult.Cancel)
				);
			}
			this.SelectCommand.Subscribe(x => {
				this.Result.Value = x;
				this.Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
			}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// ボタンパラメーター
		/// </summary>
		public class ButtonParam {
			/// <summary>
			/// 押されたときの結果
			/// </summary>
			public MessageBoxResult DialogResult {
				get;
			}

			/// <summary>
			/// 表示名
			/// </summary>
			public string DisplayName {
				get;
			}

			/// <summary>
			/// ESC押下で選択されるか
			/// </summary>
			public bool IsCancel {
				get {
					return this.DialogResult == MessageBoxResult.Cancel;
				}
			}

			/// <summary>
			/// 初期選択済みか
			/// </summary>
			public bool IsDefault {
				get;
			}

			public ButtonParam(string displayName, MessageBoxResult dialogResult, bool isDefault = false) {
				this.DisplayName = displayName;
				this.DialogResult = dialogResult;
				this.IsDefault = isDefault;
			}

		}
	}
}
