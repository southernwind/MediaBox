using System;
using System.Collections.Generic;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Collection;

namespace SandBeige.MediaBox.Models.Media {
	internal class VideoFileModel : MediaFileModel {
		public override IEnumerable<TitleValuePair> Properties {
			get {
				return Array.Empty<TitleValuePair>();
			}
		}

		public VideoFileModel(string filePath) : base(filePath) {
		}

		public override MediaFile RegisterToDataBase() {
			throw new NotImplementedException();
		}

	}
}
