using System.IO;
using System.Linq;
using System.Windows;

using SandBeige.MediaBox.Controls.Controls.FolderTreeViewObjects;

namespace SandBeige.MediaBox.Controls.Controls {
	/// <summary>
	/// FolderTreeView.xaml の相互作用ロジック
	/// </summary>
	public partial class FolderTreeView {
		private bool _selectedItemChanging;

		/// <summary>
		/// 選択中フォルダパス依存プロパティ
		/// </summary>
		public static readonly DependencyProperty SelectedFolderPathProperty =
			DependencyProperty.Register(
				nameof(SelectedFolderPath),
				typeof(string),
				typeof(FolderTreeView),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
					(sender, e) => ((FolderTreeView)sender).OnSelectedFolderPathChanged()));

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FolderTreeView() {
			this.Root = new[]{
				new Folder {
					DisplayName = "PC",
					Children = DriveInfo.GetDrives().Select(x => new Folder(x)).ToList(),
					IsExpanded = true
				}
			};

			this.InitializeComponent();
		}

		/// <summary>
		/// ルート
		/// </summary>
		public Folder[] Root {
			get;
			set;
		}

		/// <summary>
		/// 選択中フォルダパス
		/// </summary>
		public string SelectedFolderPath {
			get {
				return (string)this.GetValue(SelectedFolderPathProperty);
			}
			set {
				this.SetValue(SelectedFolderPathProperty, value);
			}
		}

		/// <summary>
		/// 選択中アイテム変更時
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e) {
			if (e.NewValue == null) {
				// 選択解除の場合
				this.SetValue(SelectedFolderPathProperty, null);
				return;
			}

			if (!(e.NewValue is Folder folder)) {
				return;
			}
			this._selectedItemChanging = true;
			this.SetValue(SelectedFolderPathProperty, folder.FolderPath);
			this._selectedItemChanging = false;
		}

		/// <summary>
		/// <see cref="SelectedFolderPath"/>が変更時
		/// </summary>
		public void OnSelectedFolderPathChanged() {
			// 選択アイテムをUI操作で変更した場合は何もしない
			if (this._selectedItemChanging) {
				return;
			}
			this.Root.FirstOrDefault()?.Select(this.SelectedFolderPath);
		}
	}

}
