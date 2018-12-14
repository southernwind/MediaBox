using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Library.Exif;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.ViewModels.Media {
	/// <summary>
	/// メディアファイルViewModel
	/// </summary>
	internal class MediaFileViewModel : ViewModelBase {
		/// <summary>
		/// メディアファイルModel
		/// </summary>
		public MediaFile Model {
			get;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> FileName {
			get;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> FilePath {
			get;
		}

		/// <summary>
		/// フルサイズイメージ
		/// </summary>
		public ReactivePropertySlim<ImageSource> Image {
			get;
		} = new ReactivePropertySlim<ImageSource>();
		
		/// <summary>
		/// サムネイル
		/// </summary>
		public ReadOnlyReactivePropertySlim<Thumbnail> Thumbnail {
			get;
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double?> Latitude {
			get;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double?> Longitude {
			get;
		}

		/// <summary>
		/// 画像の回転
		/// </summary>
		public ReadOnlyReactivePropertySlim<int?> Orientation {
			get;
		}

		/// <summary>
		/// Exif情報のタイトル・値ペアリスト
		/// </summary>
		public ReadOnlyReactivePropertySlim<IEnumerable<TitleValuePair>> Exif {
			get;
		}

		// Exif読み込みコマンド
		public ReactiveCommand ExifLoadCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// タグリスト
		/// </summary>
		public ReadOnlyReactiveCollection<string> Tags {
			get;
		}

		/// <summary>
		/// タグ追加コマンド
		/// </summary>
		public ReactiveCommand<string> AddTagCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// タグ削除コマンド
		/// </summary>
		public ReactiveCommand<string> RemoveTagCommand {
			get;
		} = new ReactiveCommand<string>();

		/// <summary>
		/// イメージロードコマンド
		/// </summary>
		public ReactiveCommand LoadImageCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="mediaFile">メディアファイルModel</param>
		public MediaFileViewModel(MediaFile mediaFile) {
			this.Model = mediaFile;
			this.FileName = this.Model.FileName.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.FilePath = this.Model.FilePath.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Thumbnail = this.Model.Thumbnail.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Latitude = this.Model.Latitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Longitude = this.Model.Longitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Exif = this.Model.Exif.Select(x => x?.ToTitleValuePair()).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Tags = this.Model.Tags.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			this.Orientation = this.Model.Orientation.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			// Exif読み込みコマンド
			this.ExifLoadCommand.Subscribe(async () => await this.Model.LoadExifIfNotLoadedAsync()).AddTo(this.CompositeDisposable);

			//タグ追加コマンド
			this.AddTagCommand.Subscribe(this.Model.AddTag).AddTo(this.CompositeDisposable);

			//タグ削除コマンド
			this.RemoveTagCommand.Subscribe(this.Model.RemoveTag).AddTo(this.CompositeDisposable);

			this.LoadImageCommand
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(x => {
				var image = new BitmapImage();
				image.BeginInit();
				image.UriSource = new Uri(this.FilePath.Value);
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.CreateOptions = BitmapCreateOptions.None;

				switch (this.Orientation.Value) {
					case null:
					case 1:
					case 2:
						image.Rotation = Rotation.Rotate0;
						break;
					case 3:
					case 4:
						image.Rotation = Rotation.Rotate180;
						break;
					case 5:
					case 8:
						image.Rotation = Rotation.Rotate270;
						break;
					case 6:
					case 7:
						image.Rotation = Rotation.Rotate90;
						break;
				}

				image.EndInit();
				image.Freeze();
				
				if (new int?[] { 2, 4, 5, 7 }.Contains(this.Orientation.Value)) {
					this.Image.Value = new TransformedBitmap(image, new ScaleTransform(-1, 1, 0, 0));
					return;
				}

				this.Image.Value = image;
			});
		}
	}
}
