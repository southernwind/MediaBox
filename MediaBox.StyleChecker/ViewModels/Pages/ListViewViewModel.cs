using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.StyleChecker.ViewModels.Pages {
	internal class ListViewViewModel : IPageViewModel {
		public string Title {
			get {
				return "ListView";
			}
		}


		public IEnumerable<string> ItemsSource {
			get {
				return new[] {
					"寛政",
					"享和",
					"文化",
					"文政",
					"天保",
					"弘化",
					"嘉永",
					"安政",
					"万延",
					"文久",
					"元治",
					"慶応",
					"明治",
					"大正",
					"昭和",
					"平成",
					"令和"
				};
			}
		}

		public string SelectedItem {
			get {
				return this.ItemsSource.First();
			}
		}
	}
}
