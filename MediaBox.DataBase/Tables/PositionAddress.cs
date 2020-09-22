using System;

namespace SandBeige.MediaBox.DataBase.Tables {
	public class PositionAddress {
		private string? _type;
		private string? _name;
		private Position? _position;

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
			get {
				return this._type ?? throw new InvalidOperationException();
			}
			set {
				this._type = value;
			}
		}

		/// <summary>
		/// 名前
		/// </summary>
		public string Name {
			get {
				return this._name ?? throw new InvalidOperationException();
			}
			set {
				this._name = value;
			}
		}

		/// <summary>
		/// 順番
		/// </summary>
		public int SequenceNumber {
			get;
			set;
		}

		/// <summary>
		/// 位置情報
		/// </summary>
		public Position Position {
			get {
				return this._position ?? throw new InvalidOperationException();
			}
			set {
				this._position = value;
			}
		}
	}
}
