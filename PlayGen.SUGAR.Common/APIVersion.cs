﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.SUGAR.Common
{
	/// <summary>
	/// This class is used to see which version of the API exists in consuming 
	/// projects to facillitate checking compatibility.
	/// 
	/// Major versions should increment when API Breaking changes are added.
	/// 
	/// Minor version should increment for Fixes and Additions that won't cause existing clients with the same Major
	/// version to break.
	/// 
	/// Build version should increment for every build.
	/// </summary>
    public static class APIVersion
	{
		public const int Major = 1;

		public const int Minor = 1;

		public const int Build = 1;

		public static string Version => $"{Major}.{Minor}.{Build}";

		public static bool IsCompatible(int major)
		{
			return Major == major;
		}

		public static bool IsCompatible(string version)
		{
			return Version.Split('.').First() == version.Split('.').First();
		}
	}
}
