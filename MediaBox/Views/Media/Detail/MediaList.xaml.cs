using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SandBeige.MediaBox.Views.Media.Detail {
	/// <summary>
	/// MediaList.xaml の相互作用ロジック
	/// </summary>
	public partial class MediaList : UserControl
    {
        public MediaList()
        {
            InitializeComponent();
        }

		private void ListBoxEx_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
			if(sender is ListBox listBox) {
				if (e.Delta < 0) {
					if (listBox.SelectedIndex < listBox.Items.Count - 1) {
						listBox.SelectedIndex += 1;
					}
				} else {
					if (listBox.SelectedIndex > 0) {
						listBox.SelectedIndex -= 1;
					}
				}
			}
		}
	}
}
