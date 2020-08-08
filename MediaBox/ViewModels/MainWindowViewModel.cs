
using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.ViewModels.Album.Selector;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// メインウィンドウViewModel
	/// </summary>
	public class MainWindowViewModel : ViewModelBase {

		/// <summary>
		/// アルバムセレクター
		/// </summary>
		public AlbumSelectorViewModel AlbumSelectorViewModel {
			get;
		}

		/// <summary>
		/// 初期処理コマンド
		/// </summary>
		public ReactiveCommand InitializeCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="_">メディアファイル監視インスタンスだけは生成しておく(TODO:ここでインスタンス化すべきかどうかは要検討)</param>
		/// <param name="albumSelectorViewModel">アルバムセレクターViewModel</param>
		/// <param name="states">状態</param>
		/// <param name="logging">ログ</param>
		public MainWindowViewModel(IMediaFileManager _, MainAlbumSelectorViewModel albumSelectorViewModel, IStates states, ILogging logging) {
			albumSelectorViewModel.SetAlbumToCurrent.Execute(states.AlbumStates.AlbumHistory.Value.FirstOrDefault()?.AlbumObject);
			this.AlbumSelectorViewModel = albumSelectorViewModel.AddTo(this.CompositeDisposable);

			this.InitializeCommand.Subscribe(() => {
				logging.Log("起動完了");
			});
		}
	}
}
