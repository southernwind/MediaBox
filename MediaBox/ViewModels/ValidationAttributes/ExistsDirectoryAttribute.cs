using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SandBeige.MediaBox.ViewModels.ValidationAttributes {
	/// <summary>
	/// ディレクトリが存在するかの検証属性
	/// </summary>
	internal class ExistsDirectoryAttribute : ValidationAttribute {
		/// <summary>
		/// 検証
		/// </summary>
		/// <param name="value">検証値</param>
		/// <param name="validationContext">未使用</param>
		/// <returns>検証結果</returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			switch (value) {
				case null:
				case string str when Directory.Exists(str):
					return ValidationResult.Success;
				case string _:
					return new ValidationResult("ディレクトリが存在しません。");
				default:
					throw new ArgumentException($"型が不正です。Type:{value.GetType().FullName}");
			}
		}
	}
}
