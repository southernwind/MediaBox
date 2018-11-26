using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Livet;

namespace SandBeige.MediaBox.Controls.Controls {
	/// <summary>
	/// FolderTreeView.xaml の相互作用ロジック
	/// </summary>
	public partial class FolderTreeView : TreeView {
		/// <summary>
		/// 選択中フォルダパス依存プロパティ
		/// </summary>
		public static readonly DependencyProperty SelectedFolderPathProperty = 
			DependencyProperty.Register(
				nameof(SelectedFolderPath),
				typeof(string),
				typeof(FolderTreeView),
				new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public FolderTreeView() {
			this.Root = new[]{
				new Folder() {
					Name = "PC",
					Children = DriveInfo.GetDrives().Select(x => new Folder(x)),
					IsExpanded = true
				}
			};

			InitializeComponent();
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

			this.SetValue(SelectedFolderPathProperty, folder.FolderPath);
		}
	}

	public class Folder :NotificationObject {
		private static readonly Dictionary<DriveType, string> _driveTypes = new Dictionary<DriveType, string> {
			{DriveType.Unknown,"不明" },
			{DriveType.NoRootDirectory,"" },
			{DriveType.Removable,"リムーバブルディスク" },
			{DriveType.Fixed,"ローカルディスク" },
			{DriveType.Network,"ネットワークドライブ" },
			{DriveType.CDRom,"CDドライブ" },
			{DriveType.Ram,"RAMディスク" }
		};

		private static readonly IEnumerable<Folder> _dummyChildren = new List<Folder>() { null };
		private static readonly IEnumerable<Folder> _emptyChildren = new List<Folder>();
		private bool _isExpanded;
		private IEnumerable<Folder> _children;

		internal Folder() {
		}

		internal Folder(string folderPath) {
			this.Name = folderPath.Split('\\').Last();
			this.FolderPath = $@"{folderPath}\";
		}

		internal Folder(DriveInfo drive) {
			var v = _driveTypes[drive.DriveType];
			if (drive.IsReady) {
				v = string.IsNullOrEmpty(drive.VolumeLabel) ? v : drive.VolumeLabel;
			} else {
				this.Children = _emptyChildren;
			}
			this.FolderPath = drive.Name;
			this.Name = $"{v}({this.FolderPath.Replace(@"\", "")})";
		}

		/// <summary>
		/// フォルダパス
		/// </summary>
		public string FolderPath {
			get;
		}

		/// <summary>
		/// フォルダ名
		/// </summary>
		public string Name {
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
				if (this._isExpanded == value) {
					return;
				}
				this._isExpanded = value;
				if (this._isExpanded && this._children == null) {
					try {
						this.Children = Directory.EnumerateDirectories(this.FolderPath).Select(x => new Folder(x));
					} catch (UnauthorizedAccessException) {
						// TODO : 対処
					}
				}
				this.RaisePropertyChanged();
			}
		}

		/// <summary>
		/// 配下フォルダ
		/// </summary>
		public IEnumerable<Folder> Children {
			get {
				return this._children ?? _dummyChildren;
			}
			internal set {
				this._children = value;
				this.RaisePropertyChanged();
			}
		}
	}
}
