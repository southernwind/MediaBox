using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SandBeige.MediaBox.ViewModels.ValidationAttributes {
	/// <summary>
	/// ディレクトリが存在するかの検証属性
	/// </summary>
	internal class DirectoryExistsAttribute : ValidationAttribute {

		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			if(value == null) {
				return ValidationResult.Success;
			}

			if (value is string str) {
				if (Directory.Exists(str)) {
					return ValidationResult.Success;
				}
				return new ValidationResult("ディレクトリが存在しません。");
			}
			throw new ArgumentException($"型が不正です。Type:{value.GetType().FullName}");
		}
	}
}
