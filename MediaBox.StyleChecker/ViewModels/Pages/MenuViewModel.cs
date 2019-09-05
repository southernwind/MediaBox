using System.Collections.Generic;

namespace SandBeige.MediaBox.StyleChecker.ViewModels.Pages {
	internal class MenuViewModel : IPageViewModel {
		public string Title {
			get {
				return "Menu";
			}
		}

		public IEnumerable<Century> CenturyList {
			get {
				return new[] {
					new Century(
						19,
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
						"明治"),
					new Century(
						20,
						"大正",
						"昭和",
						"平成"),
					new Century(
						21,
						"令和")
				};
			}
		}
	}

	internal class Century {
		public int Number {
			get;
		}

		public IEnumerable<string> EraList {
			get;
		}

		public Century(int number, params string[] eraList) {
			this.Number = number;
			this.EraList = eraList;
		}
	}
}
