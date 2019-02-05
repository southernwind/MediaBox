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
	/// </summary>
	/// <remarks>
	/// 複数のアルバムをまとめて管理するためのクラス。フォルダのような役割を持つ。
	/// <see cref="RegisteredAlbum.AlbumPath"/>をソースに生成される。
	/// 例えば、<see cref="RegisteredAlbum.AlbumPath"/>が"/a/b/c"だった場合a配下にb、b配下にcという名前の<see cref="AlbumBox"/>が生成される。
	/// cには生成元となった<see cref="AlbumBox"/>が格納される。
	/// 入れ子構造になっていて、<see cref="Children"/>に子アルバムボックスを持つ。
	/// 直下のアルバムは<see cref="Albums"/>に含まれる。
	/// <see cref="Update(IEnumerable{RegisteredAlbum})"/>を呼び出すことで配下のアルバム、アルバムボックスを更新することができる。
	/// </remarks>
	internal class AlbumBox : ModelBase {
		private readonly string _currentPath;
		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
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
		/// <param name="title">アルバムボックスのタイトル</param>
		/// <param name="currentPath">このアルバムまでのパス(ルート要素は空文字)</param>
		/// <param name="albums">このアルバムボックス配下のアルバム</param>
		public AlbumBox(string title, string currentPath, IEnumerable<RegisteredAlbum> albums) {
			this.Title.Value = title;
			this._currentPath = currentPath;

			this.Update(albums);
		}

		/// <summary>
		/// 子の更新
		/// </summary>
		/// <remarks>
		/// 必要な要素にだけ更新をかける。
		/// </remarks>
		/// <param name="albums">新配下アルバム</param>
		public void Update(IEnumerable<RegisteredAlbum> albums) {
			// 子のアルバムタイトルを生成するための正規表現
			var regex = new Regex($"^{this._currentPath}/(.*?)(/|$)");

			// (新)このアルバムボックス直下の子を取得
			var newAlbums = albums.Where(x => x.AlbumPath.Value == this._currentPath);

			// 直下のアルバムの更新　不要なものを削除し、足りていないものを追加する
			this.Albums.RemoveRange(this.Albums.Except(newAlbums));
			this.Albums.AddRange(newAlbums.Except(this.Albums));

			// 新配下アルバムをソースにアルバムボックスを作成する
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

		public override string ToString() {
			return $"<[{base.ToString()}] {this._currentPath}/{this.Title.Value}>";
		}
	}
}
