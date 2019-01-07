using System;
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
		private readonly string _currentPath;
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
			this._currentPath = currentPath;

			this.Update(albums);
		}

		/// <summary>
		/// 子の更新
		/// </summary>
		/// <param name="albums">新配下アルバム</param>
		public void Update(IEnumerable<RegisteredAlbum> albums) {
			var regex = new Regex($"^{this._currentPath}/(.*?)(/|$)");
			var newAlbums = albums.Where(x => x.AlbumPath.Value == this._currentPath);

			this.Albums.RemoveRange(this.Albums.Except(newAlbums));
			this.Albums.AddRange(newAlbums.Except(this.Albums));

			var newChildren = albums.GroupBy(x => {

				var match = regex.Match(x.AlbumPath.Value);
				if (match.Success) {
					return match.Result("$1");
				}
				return "";
			}).ToArray();

			// 新しい子にも古い子にも含まれていれば更新のみ
			foreach (var child in this.Children.Where(x => newChildren.Select(c => c.Key).Contains(x.Title.Value))) {
				child.Update(newChildren.Single(x => x.Key == child.Title.Value));
			}

			// 新しい子に含まれていなくて、古い子に含まれていれば削除する
			this.Children.RemoveRange(this.Children.Where(x => !newChildren.Select(n => n.Key).Contains(x.Title.Value)));

			// 新しい子に含まれていて、古い子に含まれていなければ追加する
			this.Children
				.AddRange(
					newChildren
						.Where(x => !this.Children.Select(c => c.Title.Value).Contains(x.Key))
						.Where(x => x.Key != "")
						.Select(x => Get.Instance<AlbumBox>(x.Key, $"{this._currentPath}/{x.Key}", x))
				);
		}
	}
}
