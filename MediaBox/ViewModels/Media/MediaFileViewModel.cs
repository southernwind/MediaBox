using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using SandBeige.MediaBox.Models.Media;
using Reactive.Bindings;
using SandBeige.MediaBox.Repository;
using Unity;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using System.IO;
using SandBeige.MediaBox.ViewModels.ValidationAttributes;
using System.Windows.Media.Imaging;
using SandBeige.MediaBox.Base;

namespace SandBeige.MediaBox.ViewModels.Media
{
	/// <summary>
	/// メディアファイルViewModel
	/// </summary>
    internal class MediaFileViewModel : ViewModelBase
    {
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
		/// サムネイルファイルパス
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> ThumbnailFilePath {
			get;
			private set;
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double> Latitude {
			get;
			private set;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double> Longitude {
			get;
			private set;
		}


		/// <summary>
		/// 初期処理
		/// </summary>
		/// <param name="mediaFile">メディアファイルModel</param>
		/// <returns><see cref="this"/></returns>
		public MediaFileViewModel Initialize(MediaFile mediaFile)
		{
			this.Model = mediaFile;
			this.FileName = this.Model.FileName.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.FilePath = this.Model.FilePath.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.ThumbnailFilePath = this.Model.ThumbnailFilePath.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Latitude = this.Model.Latitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.Longitude = this.Model.Longitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			return this;
		}
    }
}
