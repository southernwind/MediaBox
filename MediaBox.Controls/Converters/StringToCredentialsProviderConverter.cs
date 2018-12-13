using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;

namespace SandBeige.MediaBox.Controls.Converters {
	public class StringToCredentialsProviderConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(value is string str)) {
				return new CredentialsProviderImp("");
			}

			var credentials = new CredentialsProviderImp(str);
			return credentials;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

	public class CredentialsProviderImp : CredentialsProvider {
		private readonly string _id;
		public CredentialsProviderImp(string id) {
			this._id = id;
		}

		public override string SessionId {
			get;
		}

		public override void GetCredentials(Action<Credentials> callback) {
			var c = new Credentials {
				ApplicationId = this._id
			};
			callback(c);
		}
	}
}
