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
		public IReactiveProperty<string> FileName {
			get;
			private set;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public IReactiveProperty<string> FilePath {
			get;
			private set;
		}

		/// <summary>
		/// サムネイルファイルパス
		/// </summary>
		public IReactiveProperty<string> ThumbnailFilePath {
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
			this.FileName = this.Model.FileName.ToReactiveProperty().AddTo(this.CompositeDisposable);
			this.FilePath = this.Model.FilePath.ToReactiveProperty().AddTo(this.CompositeDisposable);
			this.ThumbnailFilePath = this.Model.ThumbnailFilePath.ToReactiveProperty().AddTo(this.CompositeDisposable);
			return this;
		}
    }
}
