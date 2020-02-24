using System;

namespace UniGames.EasyJson
{
	internal class JsonUInt32Controller : JsonValueTypeController
	{
		public JsonUInt32Controller()
		{
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return UInt32.Parse(data.Data);
		}
	}
}

