namespace SandBeige.MediaBox.DataBase.Tables {
	public class PositionAddress {
		/// <summary>
		/// 場所の種類
		/// </summary>
		public string Type {
			get;
			set;
		} = string.Empty;

		/// <summary>
		/// 名前
		/// </summary>
		public string Name {
			get;
			set;
		} = string.Empty;

		/// <summary>
		/// 順番
		/// </summary>
		public int SequenceNumber {
			get;
			set;
		}
	}
}
