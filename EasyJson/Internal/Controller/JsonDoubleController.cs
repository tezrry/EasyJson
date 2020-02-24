using System;

namespace UniGames.EasyJson
{
	internal class JsonDoubleController : JsonValueTypeController
	{
		public JsonDoubleController()
		{
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return Double.Parse(data.Data);
		}
	}
}

