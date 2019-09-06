using System.Collections.Generic;

using SandBeige.MediaBox.StyleChecker.Models;

namespace SandBeige.MediaBox.StyleChecker.ViewModels.Pages {
	internal class TreeViewViewModel : IPageViewModel {
		public string Title {
			get {
				return "TreeView";
			}
		}
		public IEnumerable<Nestable> NestableList {
			get {
				return new[] {
					new Nestable("将軍",
						new Nestable("大老"),
						new Nestable("老中",
							new Nestable("側衆"),
							new Nestable("高家"),
							new Nestable("大番頭"),
							new Nestable("大目付"),
							new Nestable("江戸町奉行"),
							new Nestable("勘定奉行"),
							new Nestable("勘定吟味役"),
							new Nestable("関東郡代"),
							new Nestable("作事奉行"),
							new Nestable("道中奉行"),
							new Nestable("宗門改"),
							new Nestable("城代"),
							new Nestable("町奉行"),
							new Nestable("奉行"),
							new Nestable("甲府勤番")),
						new Nestable("側用人"),
						new Nestable("奏者番"),
						new Nestable("寺社奉行"),
						new Nestable("京都所司代"),
						new Nestable("大阪城代"),
						new Nestable("若年寄",
							new Nestable("書院番頭",
								new Nestable("書院番組頭")),
							new Nestable("小姓組番頭",
								new Nestable("小姓組頭")),
							new Nestable("目付")))
				};
			}
		}
	}
}
