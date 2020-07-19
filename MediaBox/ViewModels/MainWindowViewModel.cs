
using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// メインウィンドウViewModel
	/// </summary>
	internal class MainWindowViewModel : ViewModelBase {

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
		public MainWindowViewModel(MediaFileManager _, MainAlbumSelectorViewModel albumSelectorViewModel) {
			albumSelectorViewModel.Model.SetAlbumToCurrent(this.States.AlbumStates.AlbumHistory.Value.FirstOrDefault());
			this.AlbumSelectorViewModel = albumSelectorViewModel.AddTo(this.CompositeDisposable);

			this.InitializeCommand.Subscribe(() => {
				this.Logging.Log("起動完了");
			});
		}
	}
}
