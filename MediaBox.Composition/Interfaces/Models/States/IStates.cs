namespace SandBeige.MediaBox.Composition.Interfaces.Models.States {
	/// <summary>
	/// 状態
	/// </summary>
	public interface IStates {
		/// <summary>
		/// アルバムの状態
		/// </summary>
		public IAlbumStates AlbumStates {
			get;
			set;
		}

		/// <summary>
		/// 各サイズ・位置の状態
		/// </summary>
		public ISizeStates SizeStates {
			get;
			set;
		}

		/// <summary>
		/// ファイルパス設定
		/// </summary>
		/// <param name="path">パス</param>
		public void SetFilePath(string path);

		/// <summary>
		/// 保存
		/// </summary>
		public void Save();

		/// <summary>
		/// ロード
		/// </summary>
		public void Load();
	}
}
