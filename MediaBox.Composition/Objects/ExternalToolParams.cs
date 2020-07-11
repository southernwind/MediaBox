
using System;
using System.Linq;
using System.Reactive.Linq;

using Prism.Mvvm;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.Composition.Objects {
	/// <summary>
	/// 外部ツールパラメータ
	/// </summary>
	/// <remarks>
	/// 外部ツール設定を復元するために必要な最低限のパラメータ
	/// </remarks>
	public class ExternalToolParams : BindableBase, IEquatable<ExternalToolParams> {
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

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ExternalToolParams() {
			this.DisplayName.ToUnit()
				.Merge(this.Command.ToUnit())
				.Merge(this.Arguments.ToUnit())
				.Merge(this.TargetExtensions.ToCollectionChanged().ToUnit())
				.Subscribe(_ => this.RaisePropertyChanged(nameof(this.TargetExtensions)));
		}
		public bool Equals(ExternalToolParams other) {
			return
				other != null &&
				this.DisplayName.Value == other.DisplayName.Value &&
				this.Command.Value == other.Command.Value &&
				this.Arguments.Value == other.Arguments.Value &&
				this.TargetExtensions.SequenceEqual(other.TargetExtensions);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}

	}
}
