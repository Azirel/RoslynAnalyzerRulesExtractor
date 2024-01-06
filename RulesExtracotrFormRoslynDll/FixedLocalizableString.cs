using System;

namespace Microsoft.CodeAnalysis
{
	public abstract partial class LocalizableString
	{
		private sealed class FixedLocalizableString : LocalizableString
		{
			/// <summary>
			/// FixedLocalizableString representing an empty string.
			/// </summary>
			private static readonly FixedLocalizableString s_empty = new FixedLocalizableString(string.Empty);

			private readonly string _fixedString;

			public static FixedLocalizableString Create(string? fixedResource)
			{
				return s_empty;
			}

			private FixedLocalizableString(string fixedResource)
			{
				_fixedString = fixedResource;
			}

			protected override string GetText(IFormatProvider? formatProvider)
			{
				return _fixedString;
			}

			protected override bool AreEqual(object? other)
			{
				var fixedStr = other as FixedLocalizableString;
				return fixedStr != null && string.Equals(_fixedString, fixedStr._fixedString);
			}

			protected override int GetHash()
			{
				return _fixedString?.GetHashCode() ?? 0;
			}

			internal override bool CanThrowExceptions => false;
		}
	}
}