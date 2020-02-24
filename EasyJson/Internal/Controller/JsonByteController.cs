using System;

namespace UniGames.EasyJson
{
	internal class JsonByteController : JsonValueTypeController
	{
		public JsonByteController()
		{
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return Byte.Parse(data.Data);
		}
	}
}

