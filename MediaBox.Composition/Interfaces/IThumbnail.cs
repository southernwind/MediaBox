using System.Windows.Media;

using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Composition.Interfaces {
	/// <summary>
	/// サムネイル
	/// </summary>
	public interface IThumbnail {
		/// <summary>
		/// サムネイル生成済みの場所
		/// </summary>
		ThumbnailLocation Location {
			get;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		string FileName {
			get;
			set;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		string FilePath {
			get;
		}

		/// <summary>
		/// サムネイル画像イメージ
		/// </summary>
		byte[] Binary {
			set;
		}

		/// <summary>
		/// 画像方向などを適用したイメージソース
		/// </summary>
		ImageSource ImageSource {
			get;
		}

		/// <summary>
		/// このサムネイルでエラーが発生しているか
		/// </summary>
		/// <remarks>
		/// エラーフラグ
		/// </remarks>
		bool HasError {
			get;
		}

		/// <summary>
		/// このサムネイルが有効かどうか
		/// </summary>
		bool Enabled {
			get;
		}
	}
}
