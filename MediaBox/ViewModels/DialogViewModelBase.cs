
using System;

using Prism.Services.Dialogs;

using SandBeige.MediaBox.Composition.Bases;

namespace SandBeige.MediaBox.ViewModels {
	public abstract class DialogViewModelBase : ViewModelBase, IDialogAware {
		public abstract string? Title {
			get;
			set;
		}

		public event Action<IDialogResult>? RequestClose;

		/// <summary>
		/// クローズリクエスト
		/// </summary>
		/// <param name="dialogResult">ダイアログ結果</param>
		public void CloseRequest(IDialogResult dialogResult) {
			this.RequestClose?.Invoke(dialogResult);
		}

		/// <summary>
		/// クローズリクエスト
		/// </summary>
		/// <param name="buttonResult">ボタン押下結果</param>
		public void CloseRequest(ButtonResult buttonResult) {
			this.RequestClose?.Invoke(new DialogResult(buttonResult));
		}

		/// <summary>
		/// クローズリクエスト
		/// </summary>
		/// <param name="buttonResult">ボタン押下結果</param>
		/// <param name="parameters">ダイアログパラメータ</param>
		public void CloseRequest(ButtonResult buttonResult, DialogParameters parameters) {
			this.RequestClose?.Invoke(new DialogResult(buttonResult, parameters));
		}

		/// <summary>
		/// クローズ可能か否か
		/// </summary>
		/// <returns></returns>
		public virtual bool CanCloseDialog() {
			return true;
		}

		/// <summary>
		/// ダイアログクローズ時処理
		/// </summary>
		public virtual void OnDialogClosed() {
			this.Dispose();
		}

		/// <summary>
		/// ダイアログオープン時処理
		/// </summary>
		/// <param name="parameters">パラメータ</param>
		public virtual void OnDialogOpened(IDialogParameters parameters) {
		}
	}
}
