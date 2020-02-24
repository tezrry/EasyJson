using System;

namespace UniGames.EasyJson
{
	internal class JsonFloatController : JsonValueTypeController
	{
		public JsonFloatController()
		{
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return Single.Parse(data.Data);
		}
	}
}

