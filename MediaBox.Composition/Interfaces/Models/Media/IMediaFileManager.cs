using System;
using System.Collections.Generic;
namespace SandBeige.MediaBox.Composition.Interfaces.Models.Media {
	/// <summary>
	/// メディアファイル監視
	/// </summary>
	public interface IMediaFileManager {
		/// <summary>
		/// メディアファイル登録通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnRegisteredMediaFiles {
			get;
		}

		/// <summary>
		/// メディアファイル登録通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnDeletedMediaFiles {
			get;
		}

		/// <summary>
		/// データベースからファイルを削除
		/// </summary>
		/// <param name="mediaFiles">削除するファイル</param>
		public void DeleteItems(IEnumerable<IMediaFileModel> mediaFiles);


		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="directoryPath">登録するファイルを含んでいるフォルダパス</param>
		public void RegisterItems(string directoryPath);

		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="mediaFilePaths">登録ファイル</param>
		public void RegisterItems(IEnumerable<string> mediaFilePaths);
	}
}
