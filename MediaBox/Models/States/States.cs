using System;
using System.IO;
using System.Xaml;
using System.Xml;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Utilities;

using Unity.Attributes;

namespace SandBeige.MediaBox.Models.States {
	public class States {
		private readonly string _statesFilePath;

		/// <summary>
		/// ロガー
		/// </summary>
		[Dependency]
		protected ILogging Logging {
			get;
			set;
		}

		/// <summary>
		/// アルバムの状態
		/// </summary>
		public AlbumStates AlbumStates {
			get;
			set;
		} = Get.Instance<AlbumStates>();

		[Obsolete("for serialize")]
		public States() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path">状態ファイルパス</param>
		public States(string path) {
			this._statesFilePath = path;
		}

		/// <summary>
		/// 保存
		/// </summary>
		public void Save() {
			using (var ms = new MemoryStream()) {
				XamlServices.Save(ms, this);
				using (var fs = File.Create(this._statesFilePath)) {
					ms.WriteTo(fs);
				}
			}
		}

		/// <summary>
		/// 読み込み
		/// </summary>
		public void Load() {
			if (!File.Exists(this._statesFilePath)) {
				this.Logging.Log("状態ファイルなし");
				this.LoadDefault();
				return;
			}

			try {

				if (!(XamlServices.Load(this._statesFilePath) is States states)) {
					this.Logging.Log("状態ファイル読み込み失敗", LogLevel.Warning);
					this.LoadDefault();
					return;
				}
				this.AlbumStates.Load(states.AlbumStates);

			} catch (XmlException ex) {
				this.Logging.Log("状態ファイル読み込み失敗", LogLevel.Warning, ex);
				this.LoadDefault();
			}
		}

		private void LoadDefault() {
			this.AlbumStates.LoadDefault();
		}
	}
}
