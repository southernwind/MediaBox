using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムボックスモデル
	/// 複数のアルバムをまとめて管理する
	/// </summary>
	internal class AlbumBox : ModelBase {
		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public ReactivePropertySlim<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 子アルバムボックス
		/// </summary>
		public ReactiveCollection<AlbumBox> Children {
			get;
		} = new ReactiveCollection<AlbumBox>();

		/// <summary>
		/// 直下アルバム
		/// 子アルバムボックスのアルバムはここには含まれない
		/// </summary>
		public ReactiveCollection<RegisteredAlbum> Albums {
			get;
		} = new ReactiveCollection<RegisteredAlbum>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="currentPath">パス</param>
		/// <param name="albums">アルバム</param>
		public AlbumBox(string title, string currentPath, IEnumerable<RegisteredAlbum> albums) {
			this.Title.Value = title;
			this.Albums.AddRange(albums.Where(x => x.AlbumPath.Value == currentPath));
			var regex = new Regex($"^{currentPath}/(.*?)(/|$)");
			this.Children.AddRange(
				albums
					.GroupBy(x => {
						var match = regex.Match(x.AlbumPath.Value);
						if (match.Success) {
							return match.Result("$1");
						}
						return "";
					})
					.Where(x => x.Key != "")
					.Select(x => Get.Instance<AlbumBox>(x.Key, $"{currentPath}/{x.Key}", x)));
		}
	}
}
