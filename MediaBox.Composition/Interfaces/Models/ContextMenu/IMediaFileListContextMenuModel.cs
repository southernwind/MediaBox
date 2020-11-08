using System.Collections.Generic;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.Tool;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.ContextMenu {
	public interface IMediaFileListContextMenuModel : IModelBase {
		/// <summary>
		/// 対象メディアファイルViewModelリスト
		/// </summary>
		IReactiveProperty<IEnumerable<IMediaFileModel>> TargetFiles {
			get;
		}

		IReactiveProperty<IAlbumObject> TargetAlbum {
			get;
		}

		IReadOnlyReactiveProperty<IAlbumBox> Shelf {
			get;
		}

		IReadOnlyReactiveProperty<bool> IsRegisteredAlbum {
			get;
		}

		/// <summary>
		/// 対象外部ツール
		/// </summary>
		ReadOnlyReactiveCollection<IExternalTool> ExternalTools {
			get;
		}

		/// <summary>
		/// 対象ファイルすべての評価設定
		/// </summary>
		/// <param name="rate"></param>
		void SetRate(int rate);

		/// <summary>
		/// サムネイルの作成
		/// </summary>
		void CreateThumbnail();

		/// <summary>
		/// ディレクトリオープン
		/// </summary>
		void OpenDirectory();

		/// <summary>
		/// 登録から削除
		/// </summary>
		void DeleteFileFromRegistry();

		/// <summary>
		/// アルバムからファイル削除
		/// </summary>
		void RemoveMediaFileFromAlbum();

		/// <summary>
		/// アルバム
		/// </summary>
		void AddMediaFileToOtherAlbum(int albumId);
	}
}