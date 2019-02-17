using System;
using System.Windows;

using Livet;

using Unosquare.FFME;

namespace SandBeige.MediaBox.Controls.Controls {
	/// <summary>
	/// VideoPlayer.xaml の相互作用ロジック
	/// </summary>
	public partial class VideoPlayer {
		/// <summary>
		/// ソースファイルパス依存プロパティ
		/// </summary>
		public static readonly DependencyProperty FilePathProperty =
			DependencyProperty.Register(
				nameof(FilePath),
				typeof(string),
				typeof(VideoPlayer),
				new PropertyMetadata(
					async (sender, e) => {
						var vp = (VideoPlayer)sender;
						if (vp.FilePath != null) {
							// TODO : これだと動画が切り替わらないことがある
							// たぶん、この問題 https://github.com/unosquare/ffmediaelement/issues/287
							vp.Media.Source = new Uri(vp.FilePath);
						} else {
							await vp.Media.Close();
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

			// ループ再生
			this._media.MediaEnded += async (sender, e) => {
				if (this.Loop) {
					await this._media.Stop();
					await this._media.Play();
				} else {
					await this._media.Stop();
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
		public async void Play() {
			await this._media.Open(this._media.Source);
			await this._media.Play();
		}

		/// <summary>
		/// 一時停止
		/// </summary>
		public async void Pause() {
			await this._media.Pause();
		}

		/// <summary>
		/// 停止
		/// </summary>
		public async void Stop() {
			await this._media.Stop();
		}

		/// <summary>
		/// シークバー移動開始
		/// </summary>
		public async void PositionMoveStart() {
			this._positionMoving = true;
			await this._media.Pause();
		}

		/// <summary>
		/// シークバー移動終了
		/// </summary>
		public async void PositionMoveEnd() {
			this._positionMoving = false;
			await this._media.Play();
		}
	}
}
