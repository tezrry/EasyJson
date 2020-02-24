using System;

namespace UniGames.EasyJson
{
	internal class JsonInt64Controller : JsonValueTypeController
	{
		public JsonInt64Controller()
		{
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return Int64.Parse(data.Data);
		}
	}
}

