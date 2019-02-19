using System.Windows.Media;

namespace SandBeige.MediaBox.Composition.Interfaces {
	/// <summary>
	/// サムネイル
	/// </summary>
	public interface IThumbnail {
		/// <summary>
		/// ファイル名
		/// </summary>
		string FileName {
			get;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		string FilePath {
			get;
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
	}
}
