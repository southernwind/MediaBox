namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// タスク実行優先度
	/// </summary>
	/// <remarks>
	/// 上から順に実行される。
	/// </remarks>
	internal enum Priority {
		LoadFullImage,
		LoadFolderAlbumFileInfo,
		LoadRegisteredAlbumOnLoad,
		LoadRegisteredAlbumOnRegister
	}
}
