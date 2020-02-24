using System;

namespace UniGames.EasyJson
{
	internal class JsonUInt64Controller : JsonValueTypeController
	{
		public JsonUInt64Controller()
		{
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return UInt64.Parse(data.Data);
		}
	}
}

