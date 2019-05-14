using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.About {
	internal class AboutModel : ModelBase {

		/// <summary>
		/// カレントライセンス
		/// </summary>
		public IReactiveProperty<License> CurrentLicense {
			get;
		} = new ReactiveProperty<License>();

		/// <summary>
		/// ライセンステキスト
		/// </summary>
		public IReadOnlyReactiveProperty<string> LicenseText {
			get;
		}

		/// <summary>
		/// ライセンスリスト
		/// </summary>
		public ReactiveCollection<License> Licenses {
			get;
		} = new ReactiveCollection<License>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AboutModel() {
			this.Licenses.AddRange(
				new License("ChainingAssertion-NUnit", "https://github.com/neuecc/ChainingAssertion", "MIT"),
				new License("MetadataExtractor", "https://github.com/drewnoakes/metadata-extractor-dotnet", "Apache2.0"),
				new License("Livet", "https://github.com/ugaya40/Livet", "zlib/libpng"),
				new License("log4net", "https://github.com/apache/logging-log4net", "Apache2.0"),
				new License("MahApps.Metro", "https://github.com/MahApps/MahApps.Metro/", "MIT"),
				new License("EntityFrameworkCore", "https://github.com/aspnet/EntityFrameworkCore", "Apache2.0"),
				new License("NUnit", "https://github.com/nunit/nunit", "MIT"),
				new License("ReactiveProperty", "https://github.com/runceel/ReactiveProperty", "MIT"),
				new License("Unity", "https://github.com/unitycontainer/unity", "Apache2.0"),
				new License("FFME", "https://github.com/unosquare/ffmediaelement", "Ms-PL"),
				new License("VirtualizingWrapPanel", "https://gitlab.com/sbaeumlisberger/virtualizing-wrap-panel", "MIT")
			);

			this.CurrentLicense.Value = this.Licenses.First();

			this.LicenseText =
				this.CurrentLicense
					.Synchronize()
					.Select(x => {
						var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\Licenses", $"{x.ProductName}.txt");
						return File.ReadAllText(path, Encoding.UTF8);
					})
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);
		}
	}
}
