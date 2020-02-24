using System;
using System.Text;

namespace UniGames.EasyJson
{
	internal class JsonStringController : EasyJsonController
	{
		public JsonStringController()
		{
		}

		public override string ObjectToJson(object obj)
		{
            if (obj == null)
                return null;

            return JsonUtil.STRING_QUOTE + JsonUtil.Escape(obj as string) + JsonUtil.STRING_QUOTE;
		}

		public override object JsonToObject(EasyJsonData data, Type t)
		{
			if (data.IsNullValue)
				return null;

			return data.Data;
		}
	}
}

