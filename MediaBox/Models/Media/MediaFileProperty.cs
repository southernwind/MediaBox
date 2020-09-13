using System.Collections.Generic;
using System.Linq;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルプロパティ
	/// </summary>
	public class MediaFileProperty : IMediaFileProperty {
		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
		}

		/// <summary>
		/// 代表値と件数
		/// </summary>
		public ValueCountPair<string?> RepresentativeValue {
			get {
				return this.Values.First();
			}
		}

		/// <summary>
		/// 値と件数リスト
		/// </summary>
		public IEnumerable<ValueCountPair<string?>> Values {
			get;
		}

		/// <summary>
		/// 複数の値が含まれているか
		/// </summary>
		public bool HasMultipleValues {
			get {
				return this.Values.Count() >= 2;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="values">値と件数リスト</param>
		public MediaFileProperty(string title, IEnumerable<ValueCountPair<string?>> values) {
			this.Title = title;
			this.Values = values;
		}
	}
}
