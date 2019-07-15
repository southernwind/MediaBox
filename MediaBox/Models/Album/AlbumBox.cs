using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// アルバムボックスモデル
	/// </summary>
	/// <remarks>
	/// 複数のアルバムをまとめて管理するためのクラス。フォルダのような役割を持つ。
	/// </remarks>
	internal class AlbumBox : ModelBase {
		private readonly int? _albumBoxId;

		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		} = new ReactivePropertySlim<string>();

		public AlbumBox Parent {
			get;
			set;
		}

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
		public ReactiveCollection<RegisteredAlbum> Albums {
			get;
		} = new ReactiveCollection<RegisteredAlbum>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="albums">このアルバムボックス配下のアルバム</param>
		public AlbumBox(IEnumerable<RegisteredAlbum> albums) {
			lock (this.DataBase) {
				var boxes = this.DataBase.AlbumBoxes.Include(x => x.Albums).AsEnumerable().Select(x => (model: new AlbumBox(x.AlbumBoxId), record: x)).ToList();
				foreach (var (model, record) in boxes) {
					model.Title.Value = record.Name;
					model.Albums.AddRange(albums.Where(a => a.AlbumBoxId.Value == model._albumBoxId));
					model.Children.AddRange(boxes.Where(b => b.record.ParentAlbumBoxId == record.AlbumBoxId).Select(x => x.model).Do(x => x.Parent = model));
				}
				this.Children.AddRange(boxes.Where(x => x.record.Parent == null).Select(x => x.model));
				this.Albums.AddRange(albums.Where(x => x.AlbumBoxId.Value == null));
			}

			this.Title.Subscribe(x => {
				lock (this.DataBase) {
					var record = this.DataBase.AlbumBoxes.FirstOrDefault(x => x.AlbumBoxId == this._albumBoxId);
					if (record == null) {
						return;
					}
					record.Name = x;
					this.DataBase.SaveChanges();
				}
			});
		}

		private AlbumBox(int albumBoxId) {
			this._albumBoxId = albumBoxId;
		}

		/// <summary>
		/// 子アルバムボックス追加
		/// </summary>
		/// <param name="name"></param>
		public void AddChild(string name) {
			lock (this.DataBase) {
				var record = new DataBase.Tables.AlbumBox() {
					ParentAlbumBoxId = this._albumBoxId,
					Name = name
				};
				this.DataBase.AlbumBoxes.Add(record);
				this.DataBase.SaveChanges();
				var model = new AlbumBox(record.AlbumBoxId);
				model.Title.Value = name;
				this.Children.Add(model);
			}
		}

		/// <summary>
		/// アルバムボックス削除
		/// </summary>
		public void Remove() {
			lock (this.DataBase) {
				var record = this.DataBase.AlbumBoxes.First(x => x.AlbumBoxId == this._albumBoxId);
				this.DataBase.AlbumBoxes.Remove(record);
				this.DataBase.SaveChanges();

				this.Parent?.Children.Remove(this);
			}
		}

		public void Rename(string name) {
			lock (this.DataBase) {
				var record = this.DataBase.AlbumBoxes.First(x => x.AlbumBoxId == this._albumBoxId);
				record.Name = name;
				this.DataBase.SaveChanges();

				this.Title.Value = name;
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}
	}
}
