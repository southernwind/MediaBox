using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Video {
	/// <summary>
	/// FFmpeg操作クラス
	/// </summary>
	public class FFmpeg {
		private readonly string _ffprobePath;
		private readonly string _ffmpegPath;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ffmpegFolderPath">ffmpeg.exe,ffprobe.exeが入っているフォルダパス</param>
		public FFmpeg(string ffmpegFolderPath) {
			this._ffprobePath = Path.Combine(ffmpegFolderPath, "ffprobe.exe");
			this._ffmpegPath = Path.Combine(ffmpegFolderPath, "ffmpeg.exe");
			if (!File.Exists(this._ffprobePath)) {
				throw new FileNotFoundException("ffprobe.exeが見つからない!", this._ffprobePath);
			}
			if (!File.Exists(this._ffmpegPath)) {
				throw new FileNotFoundException("ffmpeg.exeが見つからない!", this._ffmpegPath);
			}
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		/// <param name="filePath">動画ファイルパス</param>
		/// <param name="thumbnailDirectoryPath">サムネイル作成ディレクトリ</param>
		/// <param name="width">サムネイル幅</param>
		/// <param name="height">サムネイル高さ</param>
		/// <returns>作成されたサムネイルファイル名</returns>
		public string CreateThumbnail(string filePath, string thumbnailDirectoryPath, int width, int height) {
			var thumbnailFileName = "";
			using (var crypto = new SHA256CryptoServiceProvider()) {
				thumbnailFileName = $"{string.Join("", crypto.ComputeHash(Encoding.UTF8.GetBytes(filePath)).Select(b => $"{b:X2}").ToArray())}.jpg";
			}
			var thumbnailFilePath = Path.Combine(thumbnailDirectoryPath, thumbnailFileName);
			var thumbSize = $"{width}x{height}";

			var process = Process.Start(new ProcessStartInfo {
				FileName = this._ffmpegPath,
				Arguments = $"-ss 0 -i \"{filePath}\" -vframes 1 -f image2 -s {thumbSize} \"{thumbnailFilePath}\" -y",
				CreateNoWindow = true,
				UseShellExecute = false
			});
			process.WaitForExit();
			return thumbnailFileName;
		}

		/// <summary>
		/// メタデータ抽出
		/// </summary>
		/// <param name="filepath">動画ファイルパス</param>
		/// <returns>メタデータ</returns>
		public Metadata ExtractMetadata(string filepath) {
			var process = Process.Start(new ProcessStartInfo {
				FileName = this._ffprobePath,
				Arguments = $"{filepath} -hide_banner -show_entries stream -show_entries format",
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false
			});
			var output = process.StandardOutput.ReadToEnd();
			if (process.ExitCode != 0) {
				throw new Exception(process.StandardError.ReadToEnd());
			}

			// 整形のためのローカル関数
			Attributes<string> func(Match match) =>
				Regex.Matches(
					match.Result("$1"),
						@"^(.*?)=(.*?)$",
						RegexOptions.Multiline
					).Cast<Match>()
					.ToAttributes(m => m.Groups[1].Value.Trim(), m => m.Groups[2].Value.Trim());

			return new Metadata {
				Formats = func(Regex.Match(output, @"^\[FORMAT](.*?)^\[/FORMAT\]", RegexOptions.Singleline | RegexOptions.Multiline)),
				Streams = Regex.Matches(output, @"^\[STREAM](.*?)^\[/STREAM\]", RegexOptions.Singleline | RegexOptions.Multiline).Cast<Match>().Select(func)
			};
		}
	}
}
