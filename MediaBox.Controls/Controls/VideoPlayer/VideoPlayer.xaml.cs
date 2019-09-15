using System.Windows;
using System.Windows.Media;

namespace SandBeige.MediaBox.Controls.Controls.VideoPlayer {
	/// <summary>
	/// VideoPlayer.xaml の相互作用ロジック
	/// </summary>
	public partial class VideoPlayer {
		/// <summary>
		/// 読み込み完了までの代替画像依存プロパティ
		/// </summary>
		public static readonly DependencyProperty AltImageProperty =
			DependencyProperty.Register(
				nameof(AltImage),
				typeof(ImageSource),
				typeof(VideoPlayer));

		/// <summary>
		/// ソースファイルパス依存プロパティ
		/// </summary>
		public static readonly DependencyProperty FilePathProperty =
			DependencyProperty.Register(
				nameof(FilePath),
				typeof(string),
				typeof(VideoPlayer),
				new PropertyMetadata((sender, e) => {
					var vp = (VideoPlayer)sender;
					vp.MediaElementDataContext.Source = vp.FilePath;
				}));

		/// <summary>
		/// MediaElementのDataContext
		/// </summary>
		private ControlPanelViewModel MediaElementDataContext {
			get {
				return (ControlPanelViewModel)((dynamic)this.Content).DataContext;
			}
		}

		/// <summary>
		/// 読み込み完了までの代替画像
		/// </summary>
		public ImageSource AltImage {
			get {
				return (ImageSource)this.GetValue(AltImageProperty);
			}
			set {
				this.SetValue(AltImageProperty, value);
			}
		}

		/// <summary>
		/// ソースファイルパス
		/// </summary>
		public string FilePath {

			get {
				return (string)this.GetValue(FilePathProperty);
			}
			set {
				this.SetValue(FilePathProperty, value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public VideoPlayer() {
			this.InitializeComponent();
			((dynamic)this.Content).DataContext = new ControlPanelViewModel(this.Media);
		}
	}
}
