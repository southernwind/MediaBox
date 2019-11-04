using System;
using Microsoft.Toolkit.Win32.UI.XamlHost;

namespace SandBeige.MediaBox.XamlIslands
{
    /// <summary>
    /// 既定の Application クラスを補完するアプリケーション固有の動作を提供します。
    /// </summary>
    sealed partial class App : XamlApplication {
	    public App() {
		    this.Initialize();
	    }
	}
}
