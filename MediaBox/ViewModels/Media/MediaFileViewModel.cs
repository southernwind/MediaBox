using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
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
		}
	}
}
