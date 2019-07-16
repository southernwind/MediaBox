using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Tools;

namespace SandBeige.MediaBox.Tests.Models.Tools {
	internal class ExternalToolsFactoryTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();

			var etp1 = new ExternalToolParams();
			etp1.DisplayName.Value = "et1";
			etp1.TargetExtensions.AddRange(".jpg", ".png", ".mp4");
			var etp2 = new ExternalToolParams();
			etp2.DisplayName.Value = "et2";
			etp2.TargetExtensions.AddRange(".jpg", ".gif", ".mov");
			var etp3 = new ExternalToolParams();
			etp3.DisplayName.Value = "et3";
			etp3.TargetExtensions.AddRange(".jpg", ".png", ".mp4", ".mov");
			var etp4 = new ExternalToolParams();
			etp4.DisplayName.Value = "et4";
			this.Settings.GeneralSettings.ExternalTools.AddRange(etp1, etp2, etp3, etp4);
		}

		[TestCase(".jpg", "et1", "et2", "et3")]
		[TestCase(".png", "et1", "et3")]
		[TestCase(".mp4", "et1", "et3")]
		[TestCase(".gif", "et2")]
		[TestCase(".mov", "et2", "et3")]
		public void パターン(string extension, params string[] names) {
			var etf = new ExternalToolsFactory();
			var ets = etf.Create(extension);
			ets.Select(x => x.DisplayName.Value).Is(names);
		}

	}
}
