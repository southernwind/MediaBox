using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xaml;
using System.Xml;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;
using SandBeige.MediaBox.Utilities;

using Unity.Attributes;

namespace SandBeige.MediaBox.Models.States {
	/// <summary>
	/// 状態
	/// </summary>
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

		/// <summary>
		/// 各サイズ・位置の状態
		/// </summary>
		public SizeStates SizeStates {
			get;
			set;
		} = Get.Instance<SizeStates>();

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
				try {
					var d = new ISettingsBase[] {
						this.AlbumStates,
						this.SizeStates
					}.ToDictionary(x => x.GetType(), x => x.Export());
					XamlServices.Save(ms, d);
					using (var fs = File.Create(this._statesFilePath)) {
						ms.WriteTo(fs);
					}
				} catch (IOException ex) {
					this.Logging.Log("状態保存失敗", LogLevel.Warning, ex);
				}
			}
		}

		/// <summary>
		/// ロード
		/// </summary>
		public void Load() {
			this.LoadDefault();
			if (!File.Exists(this._statesFilePath)) {
				this.Logging.Log("状態ファイルなし");
				return;
			}

			try {

				if (!(XamlServices.Load(this._statesFilePath) is Dictionary<Type, Dictionary<string, dynamic>> states)) {
					this.Logging.Log("状態ファイル読み込み失敗", LogLevel.Warning);
					return;
				}
				foreach (var s in new ISettingsBase[] { this.AlbumStates, this.SizeStates }) {
					s.Import(states[s.GetType()]);
				}
			} catch (XmlException ex) {
				this.Logging.Log("状態ファイル読み込み失敗", LogLevel.Warning, ex);
			}
		}

		/// <summary>
		/// デフォルトロード
		/// </summary>
		private void LoadDefault() {
			this.AlbumStates.LoadDefault();
			this.SizeStates.LoadDefault();
		}
	}
}
