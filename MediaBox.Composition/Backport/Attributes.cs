namespace System.Runtime.CompilerServices {
	internal class IsExternalInit : Attribute {
	}


}

namespace System.Diagnostics.CodeAnalysis {
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class MemberNotNullAttribute : Attribute {
		/// <summary>Initializes the attribute with a field or property member.</summary>
		/// <param name="member">
		/// The field or property member that is promised to be not-null.
		/// </param>
		public MemberNotNullAttribute(string member) {
			this.Members = new[] { member };
		}

		/// <summary>Initializes the attribute with the list of field and property members.</summary>
		/// <param name="members">
		/// The list of field and property members that are promised to be not-null.
		/// </param>
		public MemberNotNullAttribute(params string[] members) {
			this.Members = members;
		}

		/// <summary>Gets field or property member names.</summary>
		public string[] Members {
			get;
		}
	}

	/// <summary>Specifies that the method or property will ensure that the listed field and property members have not-null values when returning with the specified return value condition.</summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	public sealed class MemberNotNullWhenAttribute : Attribute {
		/// <summary>Initializes the attribute with the specified return value condition and a field or property member.</summary>
		/// <param name="returnValue">
		/// The return value condition. If the method returns this value, the associated parameter will not be null.
		/// </param>
		/// <param name="member">
		/// The field or property member that is promised to be not-null.
		/// </param>
		public MemberNotNullWhenAttribute(bool returnValue, string member) {
			this.ReturnValue = returnValue;
			this.Members = new[] { member };
		}

		/// <summary>Initializes the attribute with the specified return value condition and list of field and property members.</summary>
		/// <param name="returnValue">
		/// The return value condition. If the method returns this value, the associated parameter will not be null.
		/// </param>
		/// <param name="members">
		/// The list of field and property members that are promised to be not-null.
		/// </param>
		public MemberNotNullWhenAttribute(bool returnValue, params string[] members) {
			this.ReturnValue = returnValue;
			this.Members = members;
		}

		/// <summary>Gets the return value condition.</summary>
		public bool ReturnValue {
			get;
		}

		/// <summary>Gets field or property member names.</summary>
		public string[] Members {
			get;
		}
	}
}