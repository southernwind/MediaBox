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
		/// 選択中フォルダパス依存プロパティ
		/// </summary>
		public static readonly DependencyProperty RootProperty =
			DependencyProperty.Register(
				nameof(Root),
				typeof(IFolderTreeViewItem),
				typeof(FolderTreeView));

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FolderTreeView() {
			this.InitializeComponent();
		}

		/// <summary>
		/// ルート
		/// </summary>
		public IFolderTreeViewItem Root {
			get {
				return (IFolderTreeViewItem)this.GetValue(RootProperty);
			}
			set {
				this.SetValue(RootProperty, value);
			}
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

			this._selectedItemChanging = true;
			this.SetValue(SelectedFolderPathProperty, ((IFolderTreeViewItem)e.NewValue).FolderPath);
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
			this.Root.Select(this.SelectedFolderPath);
		}
	}

}
