using Livet;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBeige.MediaBox.Models.Media
{
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
    class MediaFile:NotificationObject {
		/// <summary>
		/// ファイル名
		/// </summary>
		public ReadOnlyReactiveProperty<string> FileName {
			get;
			private set;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public ReactivePropertySlim<string> FilePath {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns><see cref="this"/></returns>
		public MediaFile Initialize(string filePath) {
			this.FilePath.Value = filePath;
			this.FileName = this.FilePath.Select(x => Path.GetFileName(x)).ToReadOnlyReactiveProperty();
			return this;
		}
	}
}
