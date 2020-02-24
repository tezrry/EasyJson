using System;

namespace UniGames.EasyJson
{
	internal class JsonInt16Controller : JsonValueTypeController
	{
		public JsonInt16Controller()
		{
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return Int16.Parse(data.Data);
		}
	}
}
