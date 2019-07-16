using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SandBeige.MediaBox.Controls.DataTemplateSelectors {
	/// <summary>
	/// 型によってテンプレートを切り替えるテンプレートセレクター
	/// </summary>
	[ContentProperty("Templates")]
	public class ContentTypeTemplateSelector : DataTemplateSelector {
		/// <summary>
		/// 変換先テンプレート
		/// </summary>
		public Collection<DataTemplate> Templates {
			get;
		} = new Collection<DataTemplate>();


		/// <summary>
		/// アイテムに対応する<see cref="DataTemplate"/>を<see cref="Templates"/>から探して返す。継承非対応。
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <param name="container">コンテナ</param>
		/// <returns>対応する<see cref="DataTemplate"/></returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			return this.Templates.FirstOrDefault(x => x.DataType as Type == item.GetType());
		}
	}
}
