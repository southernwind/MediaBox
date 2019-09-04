using System;
using System.Linq;
using System.Reflection;

using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.StyleChecker.ViewModels.Pages;

namespace SandBeige.MediaBox.StyleChecker.ViewModels {
	internal class MainWindowViewModel : ViewModel {
		public IPageViewModel[] PageViewModels {
			get;
		}

		public IReactiveProperty<IPageViewModel> CurrentPageViewModel {
			get;
			set;
		} = new ReactivePropertySlim<IPageViewModel>();

		public MainWindowViewModel() {
			this.PageViewModels =
				Assembly
					.GetExecutingAssembly()
					.GetTypes()
					.Where(x => x.GetInterface(typeof(IPageViewModel).FullName) != null)
					.Select(Activator.CreateInstance)
					.OfType<IPageViewModel>()
					.ToArray();

			this.CurrentPageViewModel.Value = this.PageViewModels.First();
		}
	}
}
