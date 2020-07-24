using System;
using System.Collections.Generic;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Services {
	/// <summary>
	/// 揮発性な状態の共有サービス
	/// </summary>
	public class VolatilityStateShareService {
		public IReactiveProperty<IEnumerable<IMediaFileModel>> MediaFileModels {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());
	}
}
