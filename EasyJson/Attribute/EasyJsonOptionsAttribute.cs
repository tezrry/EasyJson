using System;

namespace UniGames.EasyJson
{
	[FlagsAttribute]
	public enum EasyJsonOption
	{
		None = 0,
		DynamicType = 1,
		StaticField = 2,
		StaticProperty = 4,
		NoInherited = 8
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class EasyJsonOptionsAttribute : Attribute
	{
		public EasyJsonOptionsAttribute(EasyJsonOption options)
		{
			Options = options;
		}

		public EasyJsonOption Options { get; private set; }
	}
}

