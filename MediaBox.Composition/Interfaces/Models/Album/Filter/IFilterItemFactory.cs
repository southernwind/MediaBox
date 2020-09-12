namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter {
	public interface IFilterItemFactory {
		IFilterItem Create<T>(T filterItemObject) where T : IFilterItemObject;
	}
}