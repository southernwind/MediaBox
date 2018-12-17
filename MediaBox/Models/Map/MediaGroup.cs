using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Map {
	internal class MediaGroup : MediaFileCollection {
		public ReactivePropertySlim<MediaFile> Core {
			get;
		} = new ReactivePropertySlim<MediaFile>();

		public Rectangle CoreRectangle {
			get;
		}

		public MediaGroup(MediaFile core, Rectangle rectangle) {
			this.Core.Value = core;
			this.Items.Add(core);
			this.CoreRectangle = rectangle;
		}
	}
}
