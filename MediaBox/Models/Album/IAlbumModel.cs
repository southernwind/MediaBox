using System;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.Album {
	public interface IAlbumModel : IDisposable {
		/// <summary>
		/// アルバムタイトル
		/// </summary>
		IReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// アルバム情報読み込み
		/// </summary>
		void LoadMediaFiles();
	}
}
