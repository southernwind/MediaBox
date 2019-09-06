using System.Collections.Generic;

namespace SandBeige.MediaBox.StyleChecker.Models {
	internal class Nestable {

		public string Name {
			get;
		}

		public IEnumerable<Nestable> Children {
			get;
		}

		public Nestable(string name, params Nestable[] children) {
			this.Name = name;
			this.Children = children;
		}
	}
}
