using System;

using LiteDB;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.DataBase {
	public interface IDocumentDb : IDisposable {
		public ILiteCollection<MediaFile> GetMediaFilesCollection();

		public ILiteCollection<Position> GetPositionsCollection();
	}
}
