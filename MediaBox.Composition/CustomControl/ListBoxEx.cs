﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Composition.CustomControl {
	public class ListBoxEx :ListBox{
		public ListBoxEx() {
			this.SelectionChanged += (sender, e) => {
				this.ScrollIntoView(this.SelectedItem);
			};
		}
	}
}
