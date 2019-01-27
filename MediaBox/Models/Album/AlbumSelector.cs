﻿using System;
using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	internal class AlbumSelector : ModelBase {
		private readonly AlbumContainer _albumContainer;

		public ReadOnlyReactiveCollection<RegisteredAlbum> AlbumList {
			get;
		}

		/// <summary>
		/// カレントアルバム
		/// </summary>
		public IReactiveProperty<AlbumModel> CurrentAlbum {
			get;
		} = new ReactiveProperty<AlbumModel>();

		/// <summary>
		/// 一時アルバムフォルダパス
		/// </summary>
		public IReactiveProperty<string> TemporaryAlbumPath {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// 階層表示用アルバム格納棚
		/// </summary>
		public IReadOnlyReactiveProperty<AlbumBox> Shelf {
			get;
		}

		public AlbumSelector() {
			this._albumContainer = Get.Instance<AlbumContainer>();
			this.AlbumList = this._albumContainer.AlbumList.ToReadOnlyReactiveCollection(disposeElement: false).AddTo(this.CompositeDisposable);

			// カレントアルバム切り替え時、フォルダアルバムならDisposeしておく
			this.CurrentAlbum
				.Pairwise()
				.Subscribe(x => {
					if (x.OldItem is FolderAlbum fa) {
						fa.Dispose();
					}
				}).AddTo(this.CompositeDisposable);

			this.Shelf = this._albumContainer.Shelf.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// 引数のアルバムをカレントする
		/// </summary>
		/// <param name="album"></param>
		public void SetAlbumToCurrent(AlbumModel album) {
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// 一時アルバムをカレントにする
		/// </summary>
		public void SetTemporaryAlbumToCurrent() {
			if (this.TemporaryAlbumPath.Value == null) {
				return;
			}
			var album = Get.Instance<FolderAlbum>(this.TemporaryAlbumPath.Value);
			this.CurrentAlbum.Value = album;
		}

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		public void DeleteAlbum(RegisteredAlbum album) {
			lock (this.DataBase) {
				this.DataBase.Remove(this.DataBase.Albums.Single(x => x.AlbumId == album.AlbumId.Value));
				this.DataBase.SaveChanges();
			}
			this._albumContainer.RemoveAlbum(album);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.CurrentAlbum.Value.Title.Value}>";
		}
	}
}
