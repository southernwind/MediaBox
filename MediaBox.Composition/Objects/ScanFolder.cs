using System;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Objects {
	/// <summary>
	/// スキャンディレクトリ
	/// </summary>
	public class ScanDirectory : IEquatable<ScanDirectory> {
		/// <summary>
		/// ディレクトリパス
		/// </summary>
		public IReactiveProperty<string> DirectoryPath {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// サブディレクトリを含む
		/// </summary>
		public IReactiveProperty<bool> IncludeSubdirectories {
			get;
			set;
		} = new ReactiveProperty<bool>();

		/// <summary>
		/// 監視有効/無効
		/// </summary>
		public IReactiveProperty<bool> EnableMonitoring {
			get;
			set;
		} = new ReactiveProperty<bool>();

		[Obsolete("for serialize")]
		public ScanDirectory() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="includeSubdirectories">サブディレクトリを含む</param>
		/// <param name="enableMonitoring">監視有効/無効</param>
		public ScanDirectory(string directoryPath, bool includeSubdirectories = false, bool enableMonitoring = false) {
			this.DirectoryPath.Value = directoryPath;
			this.IncludeSubdirectories.Value = includeSubdirectories;
			this.EnableMonitoring.Value = enableMonitoring;
		}

		/// <summary>
		/// 比較
		/// </summary>
		/// <param name="other">比較対象</param>
		/// <returns>結果</returns>
		public bool Equals(ScanDirectory? other) {
			return
				other != null &&
				this.DirectoryPath.Value == other.DirectoryPath.Value &&
				this.IncludeSubdirectories.Value == other.IncludeSubdirectories.Value &&
				this.EnableMonitoring.Value == other.EnableMonitoring.Value;
		}
	}
}
