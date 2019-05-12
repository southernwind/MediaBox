using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

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
		public void CreateThumbnail(string filePath, string thumbnailFilePath, int width, int height) {
			var thumbSize = $"{width}x{height}";

			var process = Process.Start(new ProcessStartInfo {
				FileName = this._ffmpegPath,
				Arguments = $"-ss 0 -i \"{filePath}\" -vframes 1 -f image2 -s {thumbSize} \"{thumbnailFilePath}\" -y",
				CreateNoWindow = true,
				UseShellExecute = false
			});
			process.WaitForExit();
		}

		/// <summary>
		/// メタデータ抽出
		/// </summary>
		/// <param name="filepath">動画ファイルパス</param>
		/// <returns>メタデータ</returns>
		public Metadata ExtractMetadata(string filepath) {
			var process = new Process {
				StartInfo = new ProcessStartInfo {
					FileName = this._ffprobePath,
					Arguments = $"\"{ filepath }\" -hide_banner -show_entries stream -show_entries format",
					CreateNoWindow = true,
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					UseShellExecute = false
				}
			};
			var output = new StringBuilder();
			var error = new StringBuilder();
			using (var outputWaitHandle = new AutoResetEvent(false))
			using (var errorWaitHandle = new AutoResetEvent(false)) {
				process.OutputDataReceived += (sender, e) => {
					if (e.Data == null) {
						outputWaitHandle.Set();
						return;
					}
					output.AppendLine(e.Data);
				};

				process.ErrorDataReceived += (sender, e) => {
					if (e.Data == null) {
						errorWaitHandle.Set();
						return;
					}
					error.AppendLine(e.Data);
				};

				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				process.WaitForExit();
				outputWaitHandle.WaitOne();
				errorWaitHandle.WaitOne();
				if (process.ExitCode != 0) {
					throw new Exception(error.ToString());
				}
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
				Formats = func(Regex.Match(output.ToString(), @"^\[FORMAT](.*?)^\[/FORMAT\]", RegexOptions.Singleline | RegexOptions.Multiline)),
				Streams = Regex.Matches(output.ToString(), @"^\[STREAM](.*?)^\[/STREAM\]", RegexOptions.Singleline | RegexOptions.Multiline).Cast<Match>().Select(func)
			};
		}
	}
}
