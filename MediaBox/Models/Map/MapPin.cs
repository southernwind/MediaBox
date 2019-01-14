﻿using Reactive.Bindings;

using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Map {
	/// <summary>
	/// マップピン
	/// このグループを一つのピンとして表示する
	/// </summary>
	internal class MapPin : MediaFileCollection {
		/// <summary>
		/// 代表メディア
		/// </summary>
		public ReactivePropertySlim<MediaFile> Core {
			get;
		} = new ReactivePropertySlim<MediaFile>();

		/// <summary>
		/// 表示領域
		/// この領域がかぶるアイテムを吸収していく
		/// </summary>
		public Rectangle CoreRectangle {
			get;
		}

		/// <summary>
		/// ピン状態
		/// </summary>
		public ReactivePropertySlim<PinState> PinState {
			get;
		} = new ReactivePropertySlim<PinState>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="core">代表メディア</param>
		/// <param name="rectangle">表示領域</param>
		public MapPin(MediaFile core, Rectangle rectangle) {
			this.Core.Value = core;
			this.Items.Add(core);
			this.CoreRectangle = rectangle;
		}
	}

	// TODO : もっと妥当な場所、名前がある。後で移動
	/// <summary>
	/// ピン状態
	/// </summary>
	public enum PinState {
		Selected,
		Indeterminate,
		Unselected
	}
}