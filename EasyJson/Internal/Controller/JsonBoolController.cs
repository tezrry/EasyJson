using System;

namespace UniGames.EasyJson
{
	internal class JsonBoolController : JsonValueTypeController
	{
		public JsonBoolController()
		{
		}

		public override string ObjectToJson(object obj)
		{
			return obj.ToString().ToLower();
		}

		public override object JsonToObject(EasyJsonData data, Type t)
		{
			string value = data.Data.ToLower();
			if (value == "true")
			{
				return true;
			}
			else if (value == "false")
			{
				return false;
			}
			else
			{
				throw new EasyJsonException("Can't parse " + data.Data + " to bool value");
			}
		}
	}
}

