using System;

namespace UniGames.EasyJson
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class EasyJsonExcludeAttribute : Attribute
	{
	}
}

