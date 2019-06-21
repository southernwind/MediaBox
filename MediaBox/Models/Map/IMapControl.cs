using System;
using System.Windows;
using System.Windows.Input;

using Microsoft.Maps.MapControl.WPF;

namespace SandBeige.MediaBox.Models.Map {
	/// <summary>
	/// マップコントロールインターフェイス
	/// </summary>
	/// <remarks>
	/// マップを表示するコントロールはこれらのメソッド、プロパティ、イベントをもつ必要がある
	/// というのは建前で、既存のマップコントロールの必要なメンバーをインターフェイス化しただけ。
	/// </remarks>
	public interface IMapControl : IInputElement {
		/// <summary>
		/// マップコントロールの左上からの画面上相対座標→GPS座標変換
		/// </summary>
		/// <param name="viewportPoint">マップコントロールの左上からの画面上相対座標</param>
		/// <returns>GPS座標</returns>
		Location ViewportPointToLocation(Point viewportPoint);

		/// <summary>
		/// GPS座標変換→マップコントロールの左上からの画面上相対座標
		/// </summary>
		/// <param name="location">GPS座標</param>
		/// <returns>マップコントロールの左上からの画面上相対座標</returns>
		Point LocationToViewportPoint(Location location);

		/// <summary>
		/// 表示エリア設定
		/// </summary>
		/// <param name="leftTop">左上GPS座標</param>
		/// <param name="rightBottom">右下GPS座標</param>
		/// <param name="paddingPixel">余白(px)</param>
		void SetViewArea(Location leftTop, Location rightBottom, int paddingPixel);

		/// <summary>
		/// マップコントロールのサイズ(幅)
		/// </summary>
		double ActualWidth {
			get;
		}

		/// <summary>
		/// マップコントロールのサイズ(高さ)
		/// </summary>
		double ActualHeight {
			get;
		}

		/// <summary>
		/// 範囲プロパティの値が異常値か否か
		/// </summary>
		bool HasAreaPropertyError {
			get;
		}

		/// <summary>
		/// マップ表示更新
		/// </summary>
		/// <remarks>
		/// 拡大率の変更、表示位置の変更などで発生する。
		/// </remarks>
		event EventHandler<MapEventArgs> ViewChangeOnFrame;

		/// <summary>
		/// マップダブルクリック
		/// </summary>
		event MouseButtonEventHandler MouseDoubleClick;
	}
}
