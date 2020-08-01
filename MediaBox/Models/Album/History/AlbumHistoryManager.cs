using System.Linq;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.History {
	/// <summary>
	/// アルバム履歴管理
	/// </summary>
	public class AlbumHistoryManager : ModelBase {
		private readonly States.States _states;

		public AlbumHistoryManager(States.States states) {
			this._states = states;
		}
		/// <summary>
		/// アルバム履歴追加
		/// </summary>
		/// <param name="album">追加対象アルバム</param>
		public void Add(string title, IAlbumObject album) {
			// TODO : 同一アルバム判定 雑な判定なので、いいように作り変える
			if (this._states.AlbumStates.AlbumHistory.FirstOrDefault()?.Title == title) {
				return;
			}
			this._states.AlbumStates.AlbumHistory.Insert(0, new HistoryObject { Title = title, AlbumObject = album });
			// 10件目以降は削除
			foreach (var h in this._states.AlbumStates.AlbumHistory.Skip(10).ToArray()) {
				this._states.AlbumStates.AlbumHistory.Remove(h);
			}
		}
	}
}
