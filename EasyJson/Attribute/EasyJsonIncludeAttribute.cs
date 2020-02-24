using System;

namespace UniGames.EasyJson
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class EasyJsonIncludeAttribute : Attribute
	{
		public EasyJsonIncludeAttribute()
		{
			Name = null;
		}

		public EasyJsonIncludeAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}

