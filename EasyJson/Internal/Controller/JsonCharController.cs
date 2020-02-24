using System;

namespace UniGames.EasyJson
{
	internal class JsonCharController : JsonValueTypeController
	{
		public JsonCharController()
		{
		}

		public override string ObjectToJson(object obj)
		{
			string str = JsonUtil.Escape(obj.ToString());
			return JsonUtil.STRING_QUOTE + str + JsonUtil.STRING_QUOTE;
		}
		
		public override object JsonToObject(EasyJsonData data, Type t)
		{
			return Char.Parse(data.Data);
		}
	}
}

