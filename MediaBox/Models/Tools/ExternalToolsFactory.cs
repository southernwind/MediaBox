using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Helpers;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Tools {
	/// <summary>
	/// 拡張子をキーとした外部ツールリストのファクトリー
	/// </summary>
	internal class ExternalToolsFactory : FactoryBase<string, ReadOnlyReactiveCollection<ExternalTool>> {
		private readonly ISettings _settings;
		public ExternalToolsFactory() {
			this._settings = Get.Instance<ISettings>();
		}

		public ReadOnlyReactiveCollection<ExternalTool> Create(string key) {
			return this.Create<string, ReadOnlyReactiveCollection<ExternalTool>>(key);
		}

		protected override ReadOnlyReactiveCollection<ExternalTool> CreateInstance<TKey, TValue>(TKey key) {
			// 設定値の変更を監視して外部ツールリストを生成し直すReadOnlyReactiveCollectionを返す
			return
				this._settings
					.GeneralSettings
					.ExternalTools
					.ToFilteredReadOnlyObservableCollection(x => x.TargetExtensions.Select(e => e.ToLower()).Contains(key.ToLower()))
					.ToReadOnlyReactiveCollection(x => Get.Instance<ExternalTool>(x));
		}
	}
}
