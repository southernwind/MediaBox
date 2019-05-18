using System.Linq;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Album.Filter {
	public interface IFilterSetter {
		public IQueryable<MediaFile> SetFilterConditions(IQueryable<MediaFile> query);
	}
}
