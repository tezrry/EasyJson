using System;

namespace UniGames.EasyJson
{
	internal class JsonUInt16Controller : JsonValueTypeController
	{
		public JsonUInt16Controller()
		{
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return UInt16.Parse(data.Data);
		}
	}
}