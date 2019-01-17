using System;
using System.ComponentModel;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Settings {
	public interface IForTestSettings : INotifyPropertyChanged, IDisposable {
		IReactiveProperty<bool> RunOnBackground {
			get;
		}

		/// <summary>
		/// 設定ロード
		/// </summary>
		/// <param name="forTestSettings">読み込み元設定</param>
		void Load(IForTestSettings forTestSettings);

		/// <summary>
		/// 設定ロード
		/// </summary>
		void Load();
	}
}
