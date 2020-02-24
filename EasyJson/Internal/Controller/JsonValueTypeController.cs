using System;

namespace UniGames.EasyJson
{
	internal class JsonValueTypeController : EasyJsonController
	{
		public JsonValueTypeController()
		{
		}

		public override string ObjectToJson(object obj)
		{
            return obj.ToString();
		}
	}
}

