using SandBeige.MediaBox.Composition.Interfaces.Models.About;

namespace SandBeige.MediaBox.Models.About {
	/// <summary>
	/// ライセンス表示用オブジェクト
	/// </summary>
	public class License : ILicense {
		/// <summary>
		/// プロダクト名
		/// </summary>
		public string ProductName {
			get;
		}

		/// <summary>
		/// プロジェクトURL
		/// </summary>
		public string ProjectUrl {
			get;
		}

		/// <summary>
		/// ライセンス種別
		/// </summary>
		public string LicenseType {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="productName">プロダクト名</param>
		/// <param name="projectUrl">プロジェクトURL</param>
		/// <param name="licenseType">ライセンス種別</param>
		public License(string productName, string projectUrl, string licenseType) {
			this.ProductName = productName;
			this.ProjectUrl = projectUrl;
			this.LicenseType = licenseType;
		}

	}
}
