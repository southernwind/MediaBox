namespace SandBeige.MediaBox.DataBase.Tables {
	public class PositionAddress {
		/// <summary>
		/// 緯度
		/// </summary>
		public double Latitude {
			get;
			set;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public double Longitude {
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
		/// 名前
		/// </summary>
		public string Name {
			get;
			set;
		}

		/// <summary>
		/// 位置情報
		/// </summary>
		public Position Position {
			get;
			set;
		}
	}
}
