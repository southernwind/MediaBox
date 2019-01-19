
using System;
using System.Reactive.Linq;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.Composition.Objects {
	public class ExternalToolParams : NotificationObject {
		/// <summary>
		/// 表示名
		/// </summary>
		public IReactiveProperty<string> DisplayName {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// コマンド
		/// </summary>
		public IReactiveProperty<string> Command {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// 引数
		/// </summary>
		public IReactiveProperty<string> Arguments {
			get;
			set;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// 対象拡張子
		/// </summary>
		public ReactiveCollection<string> TargetExtensions {
			get;
			set;
		} = new ReactiveCollection<string>();

		public ExternalToolParams() {
			this.DisplayName.ToUnit()
				.Merge(this.Command.ToUnit())
				.Merge(this.Arguments.ToUnit())
				.Merge(this.TargetExtensions.ToCollectionChanged().ToUnit())
				.Subscribe(_ => this.RaisePropertyChanged(nameof(this.TargetExtensions)));
		}
	}
}
