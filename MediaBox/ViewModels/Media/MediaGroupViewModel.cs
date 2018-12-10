using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Library.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Media {
	internal class MediaGroupViewModel :MediaFileCollectionViewModel {
		public MediaFileViewModel Core {
			get;
			set;
		}

		public Rectangle CoreRectangle {
			get;
			set;
		}

		public MediaGroupViewModel(MediaFileViewModel core, Rectangle rectangle) {
			this.Core = core;
			this.Add(core);
			this.CoreRectangle = rectangle;
		}
	}
}
