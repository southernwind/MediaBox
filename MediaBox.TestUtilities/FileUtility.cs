﻿using System.Collections.Generic;
using System.IO;

namespace MediaBox.TestUtilities {
	public static class FileUtility {
		public static void Copy(string sourceDirectory, string destinationDirectory, IEnumerable<string> fileNames) {
			foreach (var filename in fileNames) {
				File.Copy(Path.Combine(sourceDirectory, filename), Path.Combine(destinationDirectory, filename));
			}
		}
	}
}
