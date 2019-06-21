using System;
using System.Linq;
using System.Linq.Expressions;

using Livet;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Expressions;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// データベース検索アルバム
	/// </summary>
	internal class LookupDatabaseAlbum : AlbumModel {
		/// <summary>
		/// 検索条件 タグ名
		/// </summary>
		public string TagName {
			get;
			set;
		}

		/// <summary>
		/// 検索条件 ワード
		/// </summary>
		public string Word {
			get;
			set;
		}

		/// <summary>
		/// 検索条件 場所
		/// </summary>
		public Address Address {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="selector">このクラスを保有しているアルバムセレクター</param>
		public LookupDatabaseAlbum(IAlbumSelector selector) : base(new ObservableSynchronizedCollection<IMediaFileModel>(), selector) {
		}

		/// <summary>
		/// データベース読み込み
		/// </summary>
		public void LoadFromDataBase() {
			this.LoadMediaFiles();
		}

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			// タグ,ワード
			Expression<Func<MediaFile, bool>> exp1 =
				mediaFile =>
					(this.TagName == null || mediaFile.MediaFileTags.Select(x => x.Tag.TagName).Contains(this.TagName)) &&
					(
						this.Word == null ||
						mediaFile.FilePath.Contains(this.Word) ||
						mediaFile.Position.DisplayName.Contains(this.Word) ||
						mediaFile.MediaFileTags.Any(x => x.Tag.TagName.Contains(this.Word))
					);
			var exp = exp1.Body;
			var visitor = new ParameterVisitor(exp1.Parameters);

			// 場所
			if (this.Address != null) {
				if (!this.Address.IsFailure && !this.Address.IsYet) {
					var current = this.Address;
					while (current is { } c && c.Type != null) {
						Expression<Func<MediaFile, bool>> exp2 = mediaFile =>
						mediaFile.Position.Addresses.Any(a => a.Type == c.Type && a.Name == c.Name);
						exp = Expression.AndAlso(exp, visitor.Visit(exp2.Body));
						current = current.Parent;
					}
				} else {
					Expression<Func<MediaFile, bool>> exp2 = mediaFile =>
						mediaFile.Latitude != null && mediaFile.Position.IsAcquired != this.Address.IsYet && mediaFile.Position.Addresses.IsEmpty();
					exp = Expression.AndAlso(exp, visitor.Visit(exp2.Body));
				}
			}

			return Expression.Lambda<Func<MediaFile, bool>>(
				exp,
				visitor.Parameters);
		}
	}
}
