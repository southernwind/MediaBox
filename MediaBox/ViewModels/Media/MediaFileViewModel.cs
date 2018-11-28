using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

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
			private set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> FileName {
			get;
			private set;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> FilePath {
			get;
			private set;
		}

		/// <summary>
		/// サムネイル
		/// </summary>
		public ReadOnlyReactivePropertySlim<Thumbnail> Thumbnail {
			get;
			private set;
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double?> Latitude {
			get;
			private set;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double?> Longitude {
			get;
			private set;
		}

		/// <summary>
		/// Exif情報のタイトル・値ペアリスト
		/// </summary>
		public ReadOnlyReactivePropertySlim<IEnumerable<TitleValuePair>> Exif {
			get;
			private set;
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
			private set;
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
		/// 初期処理
		/// </summary>
		/// <param name="mediaFile">メディアファイルModel</param>
		/// <returns><see cref="this"/></returns>
		public MediaFileViewModel Initialize(MediaFile mediaFile) {
			this.Model = mediaFile;
			this.FileName = this.Model.FileName.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.FilePath = this.Model.FilePath.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Thumbnail = this.Model.Thumbnail.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Latitude = this.Model.Latitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Longitude = this.Model.Longitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Exif = this.Model.Exif.Select(x => x?.ToTitleValuePair()).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Tags = this.Model.Tags.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			// Exif読み込みコマンド
			this.ExifLoadCommand.Subscribe(this.Model.LoadExifIfNotLoaded).AddTo(this.CompositeDisposable);

			//タグ追加コマンド
			this.AddTagCommand.Subscribe(this.Model.AddTag).AddTo(this.CompositeDisposable);

			//タグ削除コマンド
			this.RemoveTagCommand.Subscribe(this.Model.RemoveTag).AddTo(this.CompositeDisposable);

			return this;
		}
	}
}
