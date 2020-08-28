using System;

using Prism.Mvvm;

using Reactive.Bindings;

using Unosquare.FFME;

namespace SandBeige.MediaBox.Controls.Controls.VideoPlayer {
	/// <summary>
	/// ビデオコントロールパネルViewModel
	/// </summary>
	public class ControlPanelViewModel : BindableBase {
		private readonly MediaElement _media;
		private bool _loop;
		private TimeSpan _duration;
		private TimeSpan _currentTime;
		private bool _positionMoving;
		private string? _source;
		private bool _isLoaded;

		/// <summary>
		/// ループ再生
		/// </summary>
		public bool Loop {
			get {
				return this._loop;
			}
			set {
				this.SetProperty(ref this._loop, value);
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
				this.SetProperty(ref this._duration, value);
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
				this.SetProperty(ref this._currentTime, value, nameof(this.CurrentTimeSeconds));
				if (this._media.NaturalDuration.HasValue) {
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
		/// 読み込み完了かどうか
		/// </summary>
		public bool IsLoaded {
			get {
				return this._isLoaded;
			}
			set {
				this.SetProperty(ref this._isLoaded, value);
			}
		}

		/// <summary>
		/// Videoソース
		/// </summary>
		public string? Source {
			get {
				return this._source;
			}
			set {
				if (!this.SetProperty(ref this._source, value)) {
					return;
				}
				this.IsLoaded = false;
				this._media.Stop();
			}
		}

		/// <summary>
		/// 再生コマンド
		/// </summary>
		public ReactiveCommand PlayCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 一時停止コマンド
		/// </summary>
		public ReactiveCommand PauseCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 停止コマンド
		/// </summary>
		public ReactiveCommand StopCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// シークバー移動開始コマンド
		/// </summary>
		public ReactiveCommand PositionMoveStartCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// シークバー移動終了コマンド
		/// </summary>
		public ReactiveCommand PositionMoveEndCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="media"></param>
		public ControlPanelViewModel(MediaElement media) {
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

			this._media.BufferingEnded += (sender, e) => {
				this.IsLoaded = true;
			};

			// シークバー更新
			var timer = new System.Windows.Forms.Timer {
				Interval = 50
			};
			timer.Tick += (_, __) => {
				if (this._media.NaturalDuration is { } ts) {
					if (!this._positionMoving) {
						this.Duration = ts;
						this.CurrentTime = this._media.Position;
					}
				}
			};
			timer.Start();

			this.PlayCommand.Subscribe(async () => {
				if (this.Source is null) {
					return;
				}
				await this._media.Open(new Uri(this.Source));
				await this._media.Play();
			});
			this.PauseCommand.Subscribe(async () => await this._media.Pause());
			this.StopCommand.Subscribe(async () => await this._media.Stop());
			this.PositionMoveStartCommand.Subscribe(async () => {
				this._positionMoving = true;
				await this._media.Pause();
			});
			this.PositionMoveEndCommand.Subscribe(async () => {
				this._positionMoving = false;
				await this._media.Play();
			});
		}
	}
}
