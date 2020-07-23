using System;
using System.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.ViewModels.Dialog {
	public class CommonDialogWindowViewModel : DialogViewModelBase {
		/// <summary>
		/// パラメータ名 タイトル
		/// </summary>
		public static string ParameterNameTitle = nameof(ParameterNameTitle);

		/// <summary>
		/// パラメータ名
		/// </summary>
		public static string ParameterNameMessage = nameof(ParameterNameMessage);

		/// <summary>
		/// パラメータ名
		/// </summary>
		public static string ParameterNameButton = nameof(ParameterNameButton);

		/// <summary>
		/// パラメータ名
		/// </summary>
		public static string ParameterNameDefaultButton = nameof(ParameterNameDefaultButton);

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
		public ReactiveProperty<ButtonResult> Result {
			get;
		} = new ReactiveProperty<ButtonResult>();

		/// <summary>
		/// 選択コマンド
		/// </summary>
		public ReactiveCommand<ButtonResult> SelectCommand {
			get;
		} = new ReactiveCommand<ButtonResult>();

		/// <summary>
		/// ウィンドウタイトル
		/// </summary>
		public override string Title {
			get;
			set;
		}

		public override void OnDialogOpened(IDialogParameters parameters) {
			base.OnDialogOpened(parameters);

			this.Title = parameters.GetValue<string>(ParameterNameTitle);
			this.Message.Value = parameters.GetValue<string>(ParameterNameMessage);
			var button = parameters.GetValue<MessageBoxButton>(ParameterNameButton);
			var defaultButton = parameters.GetValue<ButtonResult>(ParameterNameDefaultButton);
			if (new[] { MessageBoxButton.OK, MessageBoxButton.OKCancel }.Contains(button)) {
				this.ButtonList.Add(
					new ButtonParam("OK", ButtonResult.OK, defaultButton == ButtonResult.OK)
				);
			}
			if (new[] { MessageBoxButton.YesNo, MessageBoxButton.YesNoCancel }.Contains(button)) {
				this.ButtonList.AddRange(
					new ButtonParam("Yes", ButtonResult.Yes, defaultButton == ButtonResult.Yes),
					new ButtonParam("No", ButtonResult.No, defaultButton == ButtonResult.No)
				);
			}
			if (new[] { MessageBoxButton.OKCancel, MessageBoxButton.YesNoCancel }.Contains(button)) {
				this.ButtonList.Add(
					new ButtonParam("Cancel", ButtonResult.Cancel, defaultButton == ButtonResult.Cancel)
				);
			}
			this.SelectCommand.Subscribe(this.CloseRequest).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// ボタンパラメーター
		/// </summary>
		public class ButtonParam {
			/// <summary>
			/// 押されたときの結果
			/// </summary>
			public ButtonResult DialogResult {
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
					return this.DialogResult == ButtonResult.Cancel;
				}
			}

			/// <summary>
			/// 初期選択済みか
			/// </summary>
			public bool IsDefault {
				get;
			}

			public ButtonParam(string displayName, ButtonResult dialogResult, bool isDefault = false) {
				this.DisplayName = displayName;
				this.DialogResult = dialogResult;
				this.IsDefault = isDefault;
			}

		}
	}
}
