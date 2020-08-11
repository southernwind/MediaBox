using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Helpers;

using SandBeige.MediaBox.Composition.Interfaces.Models.Notification;
using SandBeige.MediaBox.Composition.Interfaces.Models.Tool;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;

namespace SandBeige.MediaBox.Models.Tools {
	/// <summary>
	/// 拡張子をキーとした外部ツールリストのファクトリー
	/// </summary>
	/// <remarks>
	/// DIコンテナによってSingletonとして扱われる。
	/// <see cref="Create(string)"/>にキー情報として拡張子(".jpg"など)を渡すと
	/// <see cref="Settings.GeneralSettings.ExternalTools"/>からキーとして渡された拡張子を<see cref="ExternalToolParams.TargetExtensions"/>に含む
	/// <see cref="ExternalToolParams"/>を<see cref="IExternalTool"/>に変換したリストを返却する。
	/// </remarks>
	public class ExternalToolsFactory : FactoryBase<string, ReadOnlyReactiveCollection<IExternalTool>>, IExternalToolsFactory {
		private readonly ISettings _settings;
		private readonly ILogging _logging;
		private readonly INotificationManager _notificationManager;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ExternalToolsFactory(ISettings settings, ILogging logging, INotificationManager notificationManager) {
			this._settings = settings;
			this._logging = logging;
			this._notificationManager = notificationManager;

		}

		/// <summary>
		/// 外部ツールリスト作成
		/// </summary>
		/// <param name="key">拡張子</param>
		/// <returns>外部ツールリスト</returns>
		public ReadOnlyReactiveCollection<IExternalTool> Create(string key) {
			return this.Create<string, ReadOnlyReactiveCollection<IExternalTool>>(key);
		}

		/// <summary>
		/// キャッシュされていない場合のインスタンス生成関数
		/// </summary>
		/// <typeparam name="TKey">キーの型(string)</typeparam>
		/// <typeparam name="TValue">値の型(ReadOnlyReactiveCollection`ExternalToolViewModel)</typeparam>
		/// <param name="key">キーになる拡張子</param>
		/// <returns>外部ツールリスト</returns>
		protected override ReadOnlyReactiveCollection<IExternalTool> CreateInstance<TKey, TValue>(TKey key) {
			// 設定値の変更を監視して外部ツールリストを生成し直すReadOnlyReactiveCollectionを返す
			return
				this._settings
					.GeneralSettings
					.ExternalTools
					.ToFilteredReadOnlyObservableCollection(x => x.TargetExtensions.Select(e => e.ToLower()).Contains(key.ToLower()))
					.ToReadOnlyReactiveCollection<ExternalToolParams, IExternalTool>(x => new ExternalTool(x, this._logging, this._notificationManager));
		}
	}
}
