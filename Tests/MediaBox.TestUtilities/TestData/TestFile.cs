using System;
using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables.Metadata;

namespace SandBeige.MediaBox.TestUtilities.TestData {
	/// <summary>
	/// テストファイル検証値保持クラス
	/// </summary>
	public struct TestFile {
		public string FileName;
		public string FilePath;
		public string Extension;
		public DateTime CreationTime;
		public DateTime ModifiedTime;
		public DateTime LastAccessTime;
		public long FileSize;
		public ComparableSize? Resolution;
		public GpsLocation Location;
		public int Rate;
		public bool IsInvalid;
		public IEnumerable<string> Tags;
		public bool Exists;
		public Jpeg Jpeg;
		public Png Png;
		public Bmp Bmp;
	}
}
