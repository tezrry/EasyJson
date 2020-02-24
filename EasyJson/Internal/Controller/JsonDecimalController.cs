using System;

namespace UniGames.EasyJson
{
	internal class JsonDecimalController : JsonValueTypeController
	{
		public JsonDecimalController()
		{
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return Decimal.Parse(data.Data);
		}
	}
}

