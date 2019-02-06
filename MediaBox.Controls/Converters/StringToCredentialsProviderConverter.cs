using System;
using System.Globalization;
using System.Windows.Data;

using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;

namespace SandBeige.MediaBox.Controls.Converters {
	/// <summary>
	/// <see cref="string"/>→<see cref="CredentialProvider"/>コンバーター
	/// </summary>
	public class StringToCredentialsProviderConverter : IValueConverter {
		/// <summary>
		/// コンバート
		/// </summary>
		/// <param name="value">変換前値(<see cref="string"/></param>
		/// <param name="targetType">未使用</param>
		/// <param name="parameter">未使用</param>
		/// <param name="culture">未使用</param>
		/// <returns>変換後値(<see cref="CredentialsProvider"/>)</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is string str)) {
				return new CredentialsProviderImp("");
			}

			var credentials = new CredentialsProviderImp(str);
			return credentials;
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// CredentialsProviderを実装するクラス
	/// </summary>
	/// <remarks>
	/// コンストラクタの引数で受け取った文字列をApplicationIdとするCredentialsを発行するプロバイダー
	/// </remarks>
	public class CredentialsProviderImp : CredentialsProvider {
		private readonly string _id;
		/// <summary>
		/// コンバーター
		/// </summary>
		/// <param name="id">アプリケーションID</param>
		public CredentialsProviderImp(string id) {
			this._id = id;
		}

		/// <summary>
		/// 未使用
		/// </summary>
		public override string SessionId {
			get;
		}

		/// <summary>
		/// <see cref="Credentials">取得
		/// </summary>
		/// <param name="callback">生成した<see cref="Credentials"/></param>
		public override void GetCredentials(Action<Credentials> callback) {
			var c = new Credentials {
				ApplicationId = this._id
			};
			callback(c);
		}
	}
}
