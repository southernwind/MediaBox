using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SandBeige.MediaBox.Library.Video {
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
			Dictionary<string, string> func(Match match) =>
				Regex.Matches(
					match.Result("$1"),
						@"^(.*?)=(.*?)$",
						RegexOptions.Multiline
					).Cast<Match>()
					.ToDictionary(m => m.Groups[1].Value.Trim(), m => m.Groups[2].Value.Trim());

			return new Metadata {
				Formats = func(Regex.Match(output, @"^\[FORMAT](.*?)^\[/FORMAT\]", RegexOptions.Singleline | RegexOptions.Multiline)),
				Streams = Regex.Matches(output, @"^\[STREAM](.*?)^\[/STREAM\]", RegexOptions.Singleline | RegexOptions.Multiline).Cast<Match>().Select(func)
			};
		}
	}
}
