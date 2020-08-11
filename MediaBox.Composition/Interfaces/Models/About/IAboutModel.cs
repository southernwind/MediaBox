using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.About {
	public interface IAboutModel {
		IReactiveProperty<ILicense> CurrentLicense {
			get;
		}
		ReactiveCollection<ILicense> Licenses {
			get;
		}
		IReadOnlyReactiveProperty<string> LicenseText {
			get;
		}
	}
}