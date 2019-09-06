using System.Collections.Generic;

namespace SandBeige.MediaBox.StyleChecker.Models {


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
