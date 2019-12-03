using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムボックスモデル
	/// </summary>
	/// <remarks>
	/// 複数のアルバムをまとめて管理するためのクラス。フォルダのような役割を持つ。
	/// </remarks>
	internal class AlbumBox : ModelBase {
		private readonly ReadOnlyReactiveCollection<RegisteredAlbum> _albumList;
		private AlbumBox _parent;

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReactiveProperty<int?> AlbumBoxId {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 子アルバムボックス
		/// </summary>
		public ReactiveCollection<AlbumBox> Children {
			get;
		} = new ReactiveCollection<AlbumBox>();

		/// <summary>
		/// 直下アルバム
		/// 子アルバムボックスのアルバムはここには含まれない
		/// </summary>
		public IFilteredReadOnlyObservableCollection<RegisteredAlbum> Albums {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="albums">このアルバムボックス配下のアルバム</param>
		public AlbumBox(ReadOnlyReactiveCollection<RegisteredAlbum> albums) : this(null, albums) {
			lock (this.Rdb) {
				var boxes = this.Rdb.AlbumBoxes.Include(x => x.Albums).AsEnumerable().Select(x => (model: new AlbumBox(x.AlbumBoxId, albums), record: x)).ToList();
				foreach (var (model, record) in boxes) {
					model.Title.Value = record.Name;
					model.Children.AddRange(boxes.Where(b => b.record.ParentAlbumBoxId == record.AlbumBoxId).Select(x => x.model).Do(x => x._parent = model));
				}
				this.Children.AddRange(boxes.Where(x => x.record.Parent == null).Select(x => x.model));
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="albumBoxId">アルバムボックスID</param>
		/// <param name="albums">このアルバムボックス配下のアルバム</param>
		private AlbumBox(int? albumBoxId, ReadOnlyReactiveCollection<RegisteredAlbum> albums) {
			this._albumList = albums;
			this.AlbumBoxId.Value = albumBoxId;
			this.Albums = this._albumList.ToFilteredReadOnlyObservableCollection(x => x.AlbumBoxId.Value == albumBoxId).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// 子アルバムボックス追加
		/// </summary>
		/// <param name="name"></param>
		public void AddChild(string name) {
			lock (this.Rdb) {
				var record = new DataBase.Tables.AlbumBox {
					ParentAlbumBoxId = this.AlbumBoxId.Value,
					Name = name
				};
				this.Rdb.AlbumBoxes.Add(record);
				this.Rdb.SaveChanges();
				var model = new AlbumBox(record.AlbumBoxId, this._albumList);
				model.Title.Value = name;
				model._parent = this;
				this.Children.Add(model);
			}
		}

		/// <summary>
		/// アルバムボックス削除
		/// </summary>
		public void Remove() {
			if (!this.AlbumBoxId.Value.HasValue) {
				throw new InvalidOperationException();
			}
			lock (this.Rdb) {
				var record = this.Rdb.AlbumBoxes.First(x => x.AlbumBoxId == this.AlbumBoxId.Value);
				this.Rdb.AlbumBoxes.Remove(record);
				this.Rdb.SaveChanges();

				this._parent?.Children.Remove(this);
			}
		}

		/// <summary>
		/// アルバムボックスタイトル変更
		/// </summary>
		/// <param name="name">変更後タイトル</param>
		public void Rename(string name) {
			if (!this.AlbumBoxId.Value.HasValue) {
				throw new InvalidOperationException();
			}
			lock (this.Rdb) {
				var record = this.Rdb.AlbumBoxes.First(x => x.AlbumBoxId == this.AlbumBoxId.Value);
				record.Name = name;
				this.Rdb.SaveChanges();

				this.Title.Value = name;
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
