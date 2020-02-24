using System;
using System.Reflection;

namespace UniGames.EasyJson
{
	internal class JsonSystemTypeController : EasyJsonController
	{
		public JsonSystemTypeController()
		{
		}

		public override string ObjectToJson(object obj)
		{
			if (obj == null)
			{
				return null;
			}

            string str = JsonUtil.Escape(obj.ToString());
			return JsonUtil.STRING_QUOTE + str + JsonUtil.STRING_QUOTE;
		}

		public override object JsonToObject(EasyJsonData data, Type t)
		{
			if (data.IsNullValue)
				return null;

            Type inst = null;
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if ((inst = ass.GetType(data.Data)) != null)
                    return inst;
            }

            throw new EasyJsonException("Can't find type definition " + data.Data);
		}

	}
}

