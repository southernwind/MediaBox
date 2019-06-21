using System;

namespace SandBeige.MediaBox.Models.Map {
	public class Place {
		/// <summary>
		/// 場所の種類
		/// </summary>
		public string Type {
			get;
		}

		/// <summary>
		/// 場所の名前
		/// </summary>
		public string Name {
			get;
		}

		[Obsolete("for serialize")]
		public Place() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="type">場所の種類</param>
		/// <param name="name">場所の名前</param>
		public Place(string type, string name) {
			this.Type = type;
			this.Name = name;
		}
	}
}
