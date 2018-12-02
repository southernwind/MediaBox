using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Library.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBeige.MediaBox.ViewModels.Media {
	internal class MediaGroupViewModel :ViewModelBase{
		public MediaFileViewModel Core {
			get;
			set;
		}

		public Rectangle CoreRectangle {
			get;
			set;
		}

		public int Count {
			get {
				return this.List.Count;
			}
		}

		public IList<MediaFileViewModel> List {
			get;
		} = new List<MediaFileViewModel>();

		public MediaGroupViewModel(MediaFileViewModel core, Rectangle rectangle) {
			this.Core = core;
			this.List.Add(core);
			this.CoreRectangle = rectangle;
		}

	}
}
