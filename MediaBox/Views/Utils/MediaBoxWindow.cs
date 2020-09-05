
using System.Windows;

using MahApps.Metro.Controls;

using Prism.Services.Dialogs;

namespace SandBeige.MediaBox.Views.Utils {
	internal partial class MediaBoxWindow : MetroWindow, IDialogWindow {
		public IDialogResult? Result {
			get;
			set;
		}


		public MediaBoxWindow() {
			this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			this.Loaded += (sender, e) => {
				if (this.DataContext is IDialogAware da) {
					this.Title = da.Title;
				}
			};
		}
	}
}
