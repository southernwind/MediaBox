using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.About;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Models.About {
	public class AboutModel : ModelBase, IAboutModel {

		/// <summary>
		/// カレントライセンス
		/// </summary>
		public IReactiveProperty<ILicense> CurrentLicense {
			get;
		} = new ReactiveProperty<ILicense>();

		/// <summary>
		/// ライセンステキスト
		/// </summary>
		public IReadOnlyReactiveProperty<string> LicenseText {
			get;
		}

		/// <summary>
		/// ライセンスリスト
		/// </summary>
		public ReactiveCollection<ILicense> Licenses {
			get;
		} = new ReactiveCollection<ILicense>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AboutModel() {
			this.Licenses.AddRange(
				new License("MetadataExtractor", "https://github.com/drewnoakes/metadata-extractor-dotnet", "Apache2.0"),
				new License("LiteDB", "https://github.com/mbdavid/LiteDB", "MIT"),
				new License("Livet", "https://github.com/ugaya40/Livet", "zlib/libpng"),
				new License("log4net", "https://github.com/apache/logging-log4net", "Apache2.0"),
				new License("MahApps.Metro", "https://github.com/MahApps/MahApps.Metro/", "MIT"),
				new License("EntityFrameworkCore", "https://github.com/aspnet/EntityFrameworkCore", "Apache2.0"),
				new License("Magick.NET", "https://github.com/dlemstra/Magick.NET", "Apache2.0"),
				new License("NUnit", "https://github.com/nunit/nunit", "MIT"),
				new License("Prism", "https://github.com/PrismLibrary/Prism", "MIT"),
				new License("ReactiveProperty", "https://github.com/runceel/ReactiveProperty", "MIT"),
				new License("Unity", "https://github.com/unitycontainer/unity", "Apache2.0"),
				new License("FFME", "https://github.com/unosquare/ffmediaelement", "Ms-PL"),
				new License("FluentAssertions", "https://github.com/fluentassertions/fluentassertions", "Apache2.0")
			);

			this.CurrentLicense.Value = this.Licenses.First();

			this.LicenseText =
				this.CurrentLicense
					.Synchronize()
					.Select(x => {
						var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, @"Assets\Licenses", $"{x.ProductName}.txt");
						return File.ReadAllText(path, Encoding.UTF8);
					})
					.ToReadOnlyReactivePropertySlim(null!)
					.AddTo(this.CompositeDisposable);
		}
	}
}
