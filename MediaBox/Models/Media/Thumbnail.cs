using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// サムネイル
	/// </summary>
	internal static class Thumbnail {
		/// <summary>
		/// サムネイルファイル名取得
		/// </summary>
		/// <param name="filePath">生成元ファイルパス</param>
		/// <returns>サムネイルファイル名</returns>
		public static string GetThumbnailFileName(string filePath) {
			using (var crypto = new SHA256CryptoServiceProvider()) {
				return $"{string.Join("", crypto.ComputeHash(Encoding.UTF8.GetBytes(filePath)).Select(b => $"{b:X2}"))}".Insert(2, @"\");
			}
		}
	}
}
