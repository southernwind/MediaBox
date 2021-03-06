using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.ViewModels.Settings {
	/// <summary>
	/// 表示する列を編集するためのViewModel
	/// </summary>
	public class ColumnSettingsWindowViewModel : DialogViewModelBase {
		/// <summary>
		/// 候補リスト
		/// </summary>
		public IReadOnlyReactiveProperty<AvailableColumns[]> ColumnCandidates {
			get;
		}

		/// <summary>
		/// 表示する列リスト
		/// </summary>
		public ReactiveCollection<AvailableColumns> Columns {
			get;
		}

		/// <summary>
		/// 候補インデックス
		/// </summary>
		public IReactiveProperty<int> SelectedCandidateIndex {
			get;
		} = new ReactiveProperty<int>();

		/// <summary>
		/// 表示する列インデックス
		/// </summary>
		public IReactiveProperty<int> SelectedIndex {
			get;
		} = new ReactiveProperty<int>();

		/// <summary>
		/// 上へ移動コマンド
		/// </summary>
		public ReactiveCommand UpCommand {
			get;
		}

		/// <summary>
		/// 下へ移動コマンド
		/// </summary>
		public ReactiveCommand DownCommand {
			get;
		}

		/// <summary>
		/// 追加コマンド
		/// </summary>
		public ReactiveCommand AddCommand {
			get;
		}

		/// <summary>
		/// 削除コマンド
		/// </summary>
		public ReactiveCommand RemoveCommand {
			get;
		}

		/// <summary>
		/// ウィンドウタイトル
		/// </summary>
		public override string? Title {
			get {
				return "表示列設定";
			}
			set {
			}
		}

		public ColumnSettingsWindowViewModel(ISettings settings) {
			this.Columns =
				settings
					.GeneralSettings
					.EnabledColumns;

			var candidate = Enum.GetValues(typeof(AvailableColumns)).OfType<AvailableColumns>();

			this.ColumnCandidates =
				this.Columns
					.CollectionChangedAsObservable()
					.ToUnit()
					.Merge(Observable.Return(Unit.Default))
					.Select(_ => candidate.Where(x => !this.Columns.Contains(x)).ToArray())
					.ToReadOnlyReactivePropertySlim<AvailableColumns[]>();

			this.UpCommand =
				this.SelectedIndex
					.Select(x => x > 0)
					.ToReactiveCommand()
					.AddTo(this.CompositeDisposable);

			this.UpCommand.Subscribe(_ => {
				var index = this.SelectedIndex.Value;
				settings
					.GeneralSettings
					.EnabledColumns
					.Move(index, index - 1);

				this.SelectedIndex.Value = index - 1;
			}).AddTo(this.CompositeDisposable);

			this.DownCommand =
				this.SelectedIndex
					.Select(x => x != -1 && x < this.Columns.Count - 1)
					.ToReactiveCommand()
					.AddTo(this.CompositeDisposable);

			this.DownCommand.Subscribe(_ => {
				var index = this.SelectedIndex.Value;
				settings
					.GeneralSettings
					.EnabledColumns
					.Move(index, index + 1);
				this.SelectedIndex.Value = index + 1;
			}).AddTo(this.CompositeDisposable);

			this.AddCommand =
				this.SelectedCandidateIndex
					.Select(x => x != -1)
					.ToReactiveCommand()
					.AddTo(this.CompositeDisposable);

			this.AddCommand.Subscribe(_ => {
				settings
					.GeneralSettings
					.EnabledColumns
					.Add(this.ColumnCandidates.Value[this.SelectedCandidateIndex.Value]);
			});

			this.RemoveCommand =
				this.SelectedIndex
					.Select(x => x != -1)
					.ToReactiveCommand()
					.AddTo(this.CompositeDisposable);

			this.RemoveCommand.Subscribe(_ => {
				settings
					.GeneralSettings
					.EnabledColumns
					.RemoveAt(this.SelectedIndex.Value);
			});
		}
	}
}
