namespace SandBeige.MediaBox.Composition.Interfaces.Models.Map {
	/// <summary>
	/// 住所
	/// </summary>
	public interface IAddress {
		/// <summary>
		/// 親要素
		/// </summary>
		public IAddress Parent {
			get;
			set;
		}

		/// <summary>
		/// 場所の種類
		/// </summary>
		public string Type {
			get;
			set;
		}

		/// <summary>
		/// 場所の名前
		/// </summary>
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// 未取得の座標か否か
		/// </summary>
		public bool IsYet {
			get;
			set;
		}

		/// <summary>
		/// 取得に失敗した座標か否か
		/// </summary>
		public bool IsFailure {
			get;
			set;
		}

		/// <summary>
		/// 件数
		/// </summary>
		public int Count {
			get;
		}

		/// <summary>
		/// 子要素
		/// </summary>
		public IAddress[] Children {
			get;
		}
	}
}
