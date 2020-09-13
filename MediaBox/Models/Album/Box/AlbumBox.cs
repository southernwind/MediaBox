using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.Box {
	/// <summary>
	/// アルバムボックスモデル
	/// </summary>
	/// <remarks>
	/// 複数のアルバムをまとめて管理するためのクラス。フォルダのような役割を持つ。
	/// </remarks>
	public class AlbumBox : ModelBase, IAlbumBox {
		private readonly ReadOnlyReactiveCollection<AlbumForBoxModel> _albumList;
		private AlbumBox? _parent;
		private readonly IMediaBoxDbContext _rdb;

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReactiveProperty<int?> AlbumBoxId {
			get;
		} = new ReactivePropertySlim<int?>();

		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReactiveProperty<string?> Title {
			get;
		} = new ReactivePropertySlim<string?>();

		/// <summary>
		/// 子アルバムボックス
		/// </summary>
		public ReactiveCollection<IAlbumBox> Children {
			get;
		} = new ReactiveCollection<IAlbumBox>();

		/// <summary>
		/// 直下アルバム
		/// 子アルバムボックスのアルバムはここには含まれない
		/// </summary>
		public IFilteredReadOnlyObservableCollection<IAlbumForBoxModel> Albums {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="albums">このアルバムボックス配下のアルバム</param>
		public AlbumBox(ReadOnlyReactiveCollection<RegisteredAlbumObject> albums, IMediaBoxDbContext rdb, IAlbumLoaderFactory albumLoaderFactory) : this(null, albums.ToReadOnlyReactiveCollection(x => x.ToAlbumModelForAlbumBox(x.AlbumId, rdb, albumLoaderFactory)), rdb) {
			lock (this._rdb) {
				var boxes = this._rdb.AlbumBoxes.Include(x => x.Albums).AsEnumerable().Select(x => (model: new AlbumBox(x.AlbumBoxId, this._albumList, this._rdb), record: x)).ToList();
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
		private AlbumBox(int? albumBoxId, ReadOnlyReactiveCollection<AlbumForBoxModel> albums, IMediaBoxDbContext rdb) {
			this._rdb = rdb;
			this._albumList = albums;
			this.AlbumBoxId.Value = albumBoxId;
			this.Albums = this._albumList
				.ToFilteredReadOnlyObservableCollection(x => x.AlbumBoxId.Value == albumBoxId).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// 子アルバムボックス追加
		/// </summary>
		/// <param name="name"></param>
		public void AddChild(string? name) {
			lock (this._rdb) {
				var record = new DataBase.Tables.AlbumBox {
					ParentAlbumBoxId = this.AlbumBoxId.Value,
					Name = name
				};
				this._rdb.AlbumBoxes.Add(record);
				this._rdb.SaveChanges();
				var model = new AlbumBox(record.AlbumBoxId, this._albumList, this._rdb);
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
			lock (this._rdb) {
				var record = this._rdb.AlbumBoxes.First(x => x.AlbumBoxId == this.AlbumBoxId.Value);
				this._rdb.AlbumBoxes.Remove(record);
				this._rdb.SaveChanges();

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
			lock (this._rdb) {
				var record = this._rdb.AlbumBoxes.First(x => x.AlbumBoxId == this.AlbumBoxId.Value);
				record.Name = name;
				this._rdb.SaveChanges();

				this.Title.Value = name;
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
