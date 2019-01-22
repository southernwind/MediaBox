using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Media {
	internal class VideoFileModel : MediaFileModel {
		public VideoFileModel(string filePath) : base(filePath) {
		}

		public override MediaFile RegisterToDataBase() {
			throw new System.NotImplementedException();
		}
	}
}
