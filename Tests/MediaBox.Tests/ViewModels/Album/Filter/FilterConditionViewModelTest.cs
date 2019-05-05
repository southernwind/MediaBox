using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.ViewModels.Album.Filter;

namespace SandBeige.MediaBox.Tests.ViewModels.Album.Filter {
	internal class FilterConditionViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 表示名() {
			var model = new FilteringCondition(1);
			model.DisplayName.Value = "dn";
			var vm = new FilteringConditionViewModel(model);
			vm.DisplayName.Value.Is("dn");
			model.DisplayName.Value = "mn";
			vm.DisplayName.Value.Is("mn");
		}

		[Test]
		public void フィルター条件() {
			var model = new FilteringCondition(1);
			var fic1 = new FilePathFilterItemCreator("aa");
			var fic2 = new FilePathFilterItemCreator("ab");
			var fic3 = new FilePathFilterItemCreator("ac");
			model.FilterItemCreators.AddRange(fic1, fic2);
			var vm = new FilteringConditionViewModel(model);
			vm.FilterItems.Is(fic1, fic2);
			model.FilterItemCreators.AddRange(fic3);
			vm.FilterItems.Is(fic1, fic2, fic3);
		}

		[Test]
		public void ファイルパスフィルター追加() {
			var model = new FilteringCondition(1);
			var vm = new FilteringConditionViewModel(model);
			vm.AddFilePathFilterCommand.Execute("aa");
			var fic = model.FilterItemCreators.First().IsInstanceOf<FilePathFilterItemCreator>();
			fic.Text.Is("aa");
		}

		[Test]
		public void 評価フィルター追加() {
			var model = new FilteringCondition(1);
			var vm = new FilteringConditionViewModel(model);
			vm.Rate.Value = 3;
			vm.AddRateFilterCommand.Execute();
			var fic = model.FilterItemCreators.First().IsInstanceOf<RateFilterItemCreator>();
			fic.Rate.Is(3);
		}

		[Test]
		public void 解像度フィルター追加() {
			var model = new FilteringCondition(1);
			var vm = new FilteringConditionViewModel(model);
			vm.ResolutionWidth.Value = 300;
			vm.ResolutionHeight.Value = 500;
			vm.AddResolutionFilterCommand.Execute();
			var fic = model.FilterItemCreators.First().IsInstanceOf<ResolutionFilterItemCreator>();
			fic.Resolution.Is(new ComparableSize(300, 500));
		}

		[Test]
		public void メディアタイプフィルター追加() {
			var model = new FilteringCondition(1);
			var vm = new FilteringConditionViewModel(model);
			vm.AddMediaTypeFilterCommand.Execute(true);
			var fic = model.FilterItemCreators.First().IsInstanceOf<MediaTypeFilterItemCreator>();
			fic.IsVideo.Is(true);
		}

		[Test]
		public void フィルター削除() {
			var model = new FilteringCondition(1);
			var vm = new FilteringConditionViewModel(model);
			vm.AddFilePathFilterCommand.Execute("aa");
			var fic = model.FilterItemCreators.First().IsInstanceOf<FilePathFilterItemCreator>();
			vm.FilterItems.Count.Is(1);
			vm.RemoveFilterCommand.Execute(fic);
			vm.FilterItems.Count.Is(0);
		}
	}
}
