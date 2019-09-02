using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.ViewModels.Album.Filter;
using SandBeige.MediaBox.ViewModels.Album.Filter.Creators;

namespace SandBeige.MediaBox.Tests.ViewModels.Album.Filter {
	internal class FilterConditionViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 表示名() {
			var rfo = new RestorableFilterObject();
			using var model = new FilteringCondition(rfo);
			model.DisplayName.Value = "dn";
			using var vm = new FilteringConditionViewModel(model);
			vm.DisplayName.Value.Is("dn");
			model.DisplayName.Value = "mn";
			vm.DisplayName.Value.Is("mn");
		}

		[Test]
		public void フィルター条件() {
			var rfo = new RestorableFilterObject();
			using var model = new FilteringCondition(rfo);
			model.AddFilePathFilter("aa", SearchTypeInclude.Include);
			model.AddFilePathFilter("ab", SearchTypeInclude.Include);
			using var vm = new FilteringConditionViewModel(model);
			vm.FilterItems.Count.Is(2);
			vm.FilterItems.Is(model.FilterItemCreators);
			model.AddFilePathFilter("ac", SearchTypeInclude.Include);
			vm.FilterItems.Count.Is(3);
			vm.FilterItems.Is(model.FilterItemCreators);
		}

		[Test]
		public void ファイルパスフィルター追加() {
			var rfo = new RestorableFilterObject();
			using var model = new FilteringCondition(rfo);
			using var vm = new FilteringConditionViewModel(model);
			var cvm = vm.FilterCreatorViewModels.OfType<FilePathFilterCreatorViewModel>().First();
			cvm.FilePath.Value = "aa";
			cvm.SearchType.Value = cvm.SearchTypeList.First(x => x.Value == SearchTypeInclude.Include);
			cvm.AddFilePathFilterCommand.Execute();
			var fic = model.FilterItemCreators.First().IsInstanceOf<FilePathFilterItemCreator>();
			fic.Text.Is("aa");
		}

		[Test]
		public void 評価フィルター追加() {
			var rfo = new RestorableFilterObject();
			using var model = new FilteringCondition(rfo);
			using var vm = new FilteringConditionViewModel(model);
			var cvm = vm.FilterCreatorViewModels.OfType<RateFilterCreatorViewModel>().First();
			cvm.RateText.Value = "3";
			cvm.SearchType.Value = cvm.SearchTypeList.First(x => x.Value == SearchTypeComparison.GreaterThanOrEqual);
			cvm.AddRateFilterCommand.Execute();
			var fic = model.FilterItemCreators.First().IsInstanceOf<RateFilterItemCreator>();
			fic.Rate.Is(3);
		}

		[Test]
		public void 解像度フィルター追加() {
			var rfo = new RestorableFilterObject();
			using var model = new FilteringCondition(rfo);
			using var vm = new FilteringConditionViewModel(model);
			var cvm = vm.FilterCreatorViewModels.OfType<ResolutionFilterCreatorViewModel>().First();
			cvm.ResolutionWidth.Value = 300;
			cvm.ResolutionHeight.Value = 500;
			cvm.AddResolutionFilterCommand.Execute();
			var fic = model.FilterItemCreators.First().IsInstanceOf<ResolutionFilterItemCreator>();
			fic.Resolution.Is(new ComparableSize(300, 500));
		}

		[Test]
		public void メディアタイプフィルター追加() {
			var rfo = new RestorableFilterObject();
			using var model = new FilteringCondition(rfo);
			using var vm = new FilteringConditionViewModel(model);
			var cvm = vm.FilterCreatorViewModels.OfType<MediaTypeFilterCreatorViewModel>().First();
			cvm.MediaType.Value = cvm.MediaTypeList.First(x => x.Value);
			cvm.AddMediaTypeFilterCommand.Execute();
			var fic = model.FilterItemCreators.First().IsInstanceOf<MediaTypeFilterItemCreator>();
			fic.IsVideo.Is(true);
		}

		[Test]
		public void フィルター削除() {
			var rfo = new RestorableFilterObject();
			using var model = new FilteringCondition(rfo);
			using var vm = new FilteringConditionViewModel(model);
			var cvm = vm.FilterCreatorViewModels.OfType<FilePathFilterCreatorViewModel>().First();
			cvm.FilePath.Value = "aa";
			cvm.SearchType.Value = cvm.SearchTypeList.First(x => x.Value == SearchTypeInclude.Include);
			cvm.AddFilePathFilterCommand.Execute();
			var fic = model.FilterItemCreators.First().IsInstanceOf<FilePathFilterItemCreator>();
			vm.FilterItems.Count.Is(1);
			vm.RemoveFilterCommand.Execute(fic);
			vm.FilterItems.Count.Is(0);
		}
	}
}
