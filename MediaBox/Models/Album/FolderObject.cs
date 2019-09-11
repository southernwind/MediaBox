using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

using Reactive.Bindings;

using SandBeige.MediaBox.Controls.Controls.FolderTreeViewObjects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// フォルダーモデル
	/// </summary>
	internal class FolderObject : ModelBase, IFolderTreeViewItem {
		private string _folderPath;
		private string _displayName;
		private bool _isExpanded;
		private bool _isSelected;

		/// <summary>
		/// フォルダ名 + 件数
		/// </summary>
		public string DisplayName {
			get {
				return this._displayName;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._displayName, value);
			}
		}


		/// <summary>
		/// アイコン
		/// </summary>
		public ImageSource Icon {
			get;
			set;
		}

		/// <summary>
		/// 広がっているか
		/// </summary>
		public bool IsExpanded {
			get {
				return this._isExpanded;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._isExpanded, value);
			}
		}

		/// <summary>
		/// 選択されているか
		/// </summary>
		public bool IsSelected {
			get {
				return this._isSelected;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._isSelected, value);
			}
		}

		/// <summary>
		/// フォルダパス
		/// </summary>
		public string FolderPath {
			get {
				return this._folderPath;
			}
			private set {
				this.RaisePropertyChangedIfSet(ref this._folderPath, value);
			}
		}

		/// <summary>
		/// 展開コマンド
		/// </summary>
		public ReactiveCommand ExpandCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		///  配下フォルダ
		/// </summary>
		public ReactiveCollection<IFolderTreeViewItem> Children {
			get;
		} = new ReactiveCollection<IFolderTreeViewItem>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="currentPath">このフォルダまでのパス(ルート要素は空文字)</param>
		/// <param name="directories">ツリーに含むディレクトリリスト</param>
		public FolderObject(string currentPath, IEnumerable<ValueCountPair<string>> directories) {
			this.FolderPath = currentPath;
			if (Directory.Exists(this.FolderPath)) {
				this.Icon = IconUtility.GetIcon(this.FolderPath);
			}

			this.Update(directories);
			this.ExpandCommand.Subscribe(_ => {
				this.IsExpanded = true;
				this.IsSelected = true;
			});
		}

		/// <summary>
		/// 子の更新
		/// </summary>
		/// <param name="directories">ツリーに含むディレクトリリスト</param>
		public void Update(IEnumerable<ValueCountPair<string>> directories) {
			// 子のアルバムタイトルを生成するための正規表現
			var regex = new Regex(@"^(.*?(\\|$)).*");
			// このフォルダに含まれる画像の件数を取得
			if (this.FolderPath != "") {
				var count = directories.Where(x => x.Value.StartsWith(this.FolderPath)).Sum(x => x.Count);
				this.DisplayName = $"{Regex.Replace(this.FolderPath.TrimEnd('\\'), @"^.*\\(.*)$", "$1")}({count})";
			}

			var children = directories.Where(x => x.Value.StartsWith(this.FolderPath) && x.Value != this.FolderPath).ToList();

			// 新配下アルバムをソースにアルバムボックスを作成する
			var newChildren = children.GroupBy(x => {
				var str = x.Value;
				if (this.FolderPath.Length != 0) {
					str = str.Replace(this.FolderPath, "");
				}
				var match = regex.Match(str);
				if (match.Success) {
					return Path.Combine(this.FolderPath, match.Result("$1"));
				}
				return null;
			}).ToArray();

			// 新しい子にも古い子にも含まれていれば更新のみ
			foreach (var child in this.Children.Where(x => newChildren.Select(c => c.Key).Contains(x.FolderPath))) {
				((FolderObject)child).Update(newChildren.Single(x => x.Key == child.FolderPath));
			}

			// 新しい子に含まれていなくて、古い子に含まれていれば削除する
			this.Children.RemoveRange(this.Children.Where(x => !newChildren.Select(n => n.Key).Contains(x.FolderPath)));

			// 新しい子に含まれていて、古い子に含まれていなければ追加する
			this.Children
				.AddRange(
					newChildren
						.Where(x => !this.Children.Select(c => c.FolderPath).Contains(x.Key))
						.Where(x => x.Key != null)
						.Select(x => new FolderObject($@"{Path.Combine(this.FolderPath, x.Key)}", x))
				);
		}

		/// <summary>
		/// 指定フォルダパスの選択
		/// </summary>
		/// <remarks>
		/// 直下のフォルダがフォルダパスに含まれていればフォルダを展開し、子要素の<see cref="Select(string)"/>を呼び出す。
		/// </remarks>
		/// <param name="path">フォルダパス</param>
		public void Select(string path) {
			if (path == null) {
				return;
			}
			if (path.StartsWith(this.FolderPath)) {
				this.IsExpanded = true;
				foreach (var child in this.Children) {
					if (child is Folder folder) {
						folder.Select(path);
					}
				}
			}
			if (path == this.FolderPath) {
				this.IsSelected = true;
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.FolderPath}>";
		}
	}
}
