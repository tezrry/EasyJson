using System;

namespace UniGames.EasyJson
{
	internal class JsonInt32Controller : JsonValueTypeController
	{
		public JsonInt32Controller()
		{
		}

		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return Int32.Parse(data.Data);
		}
	}
}

