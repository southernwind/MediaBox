using Prism.Ioc;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.God;

namespace SandBeige.MediaBox.Models.Album.Selector {
	public class AlbumSelectorFactory : FactoryBase<string, IAlbumSelector>, IAlbumSelectorProvider {
		private readonly IContainerProvider _provider;
		public AlbumSelectorFactory(IContainerProvider provider) {
			this._provider = provider;
		}

		/// <summary>
		/// アルバムセレクター作成
		/// </summary>
		/// <param name="name">セレクター名</param>
		/// <returns>アルバムセレクター</returns>
		public IAlbumSelector Create(string name) {
			return this.Create<string, IAlbumSelector>(name);
		}

		protected override IAlbumSelector CreateInstance<TKey, TValue>(TKey key) {
			var instance = this._provider.Resolve<TValue>();
			instance.SetName(key);
			return instance;
		}
	}
}
