namespace SandBeige.MediaBox.Composition.Enum {
	/// <summary>
	/// タスク実行優先度
	/// </summary>
	/// <remarks>
	/// 上から順に実行される。
	/// </remarks>
	public enum Priority {
		// フルサイズイメージロード
		LoadFullImage,
		// アルバムのメディアファイル読み込み
		LoadMediaFiles,
		// 位置情報の取得
		GetPositionDetails,
		// アルバムにファイル追加
		AddMediaFilesToAlbum,
		// メディアファイルの登録(キューへの追加のみ)
		RegisterMediaFiles,
		// サムネイル作成
		CreateThumbnail,
		// 逆ジオコーディング
		ReverseGeoCoding
	}
}
