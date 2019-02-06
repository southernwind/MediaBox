using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Livet;

namespace SandBeige.MediaBox.Controls.Controls {
	/// <summary>
	/// VideoPlayer.xaml の相互作用ロジック
	/// </summary>
	public partial class VideoPlayer : UserControl {
		/// <summary>
		/// ソースファイルパス依存プロパティ
		/// </summary>
		public static readonly DependencyProperty FilePathProperty =
			DependencyProperty.Register(
				nameof(FilePath),
				typeof(string),
				typeof(VideoPlayer),
				new PropertyMetadata(
					(sender, e) => {
						var vp = (VideoPlayer)sender;
						if (vp.FilePath != null) {
							vp.Media.Source = new Uri(vp.FilePath);
							vp.Media.Stop();
						} else {
							vp.Media.Source = null;
						}
					}));

		/// <summary>
		/// 回転依存プロパティ
		/// </summary>
		public static readonly DependencyProperty RotationProperty =
			DependencyProperty.Register(
				nameof(Rotation),
				typeof(double?),
				typeof(VideoPlayer),
				new PropertyMetadata(
					(sender, e) => {
						var vp = (VideoPlayer)sender;
						if (vp.Rotation is double d) {
							vp.Media.LayoutTransform = new RotateTransform(d);
						} else {
							vp.Media.LayoutTransform = null;
						}
					}));

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
		/// 回転
		/// </summary>
		public double? Rotation {

			get {
				return (double?)this.GetValue(RotationProperty);
			}
			set {
				this.SetValue(RotationProperty, value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public VideoPlayer() {
			this.InitializeComponent();
			((dynamic)this.Content).DataContext = new VideoPlayerViewModel(this.Media);
		}
	}

	/// <summary>
	/// VideoPlayer用のViewModel
	/// </summary>
	internal class VideoPlayerViewModel : ViewModel {
		private readonly MediaElement _media;
		private bool _loop;
		private TimeSpan _duration;
		private TimeSpan _currentTime;
		private bool _positionMoving;

		/// <summary>
		/// ループ再生
		/// </summary>
		public bool Loop {
			get {
				return this._loop;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._loop, value);
			}
		}

		/// <summary>
		/// 全体再生時間
		/// </summary>
		public TimeSpan Duration {
			get {
				return this._duration;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._duration, value);
			}
		}

		/// <summary>
		/// 現在再生時間
		/// </summary>
		public TimeSpan CurrentTime {
			get {
				return this._currentTime;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._currentTime, value, nameof(this.CurrentTimeSeconds));
				if (this._media.NaturalDuration.HasTimeSpan) {
					if (this._positionMoving) {
						this._media.Position = this.CurrentTime;
					}
				} else {
					this.CurrentTime = TimeSpan.Zero;
				}
			}
		}

		/// <summary>
		/// シークバー用
		/// </summary>
		public double CurrentTimeSeconds {
			get {
				return this._currentTime.TotalSeconds;
			}
			set {
				this.CurrentTime = TimeSpan.FromSeconds(value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="media"></param>
		public VideoPlayerViewModel(MediaElement media) {
			this._media = media;

			this._media.LoadedBehavior = MediaState.Manual;

			// ループ再生
			this._media.MediaEnded += (sender, e) => {
				if (this.Loop) {
					this._media.Position = TimeSpan.Zero;
					this._media.Play();
				} else {
					this._media.Stop();
				}
			};

			// シークバー更新
			var timer = new System.Windows.Forms.Timer {
				Interval = 50
			};
			timer.Tick += (_, __) => {
				if (this._media.NaturalDuration.HasTimeSpan) {
					if (!this._positionMoving) {
						this.Duration = this._media.NaturalDuration.TimeSpan;
						this.CurrentTime = this._media.Position;
					}
				}
			};
			timer.Start();
		}

		/// <summary>
		/// 再生
		/// </summary>
		public void Play() {
			this._media.Play();
		}

		/// <summary>
		/// 一時停止
		/// </summary>
		public void Pause() {
			this._media.Pause();
		}

		/// <summary>
		/// 停止
		/// </summary>
		public void Stop() {
			this._media.Stop();
		}

		/// <summary>
		/// シークバー移動開始
		/// </summary>
		public void PositionMoveStart() {
			this._positionMoving = true;
			this._media.Pause();
		}

		/// <summary>
		/// シークバー移動終了
		/// </summary>
		public void PositionMoveEnd() {
			this._positionMoving = false;
			this._media.Play();
		}
	}
}
