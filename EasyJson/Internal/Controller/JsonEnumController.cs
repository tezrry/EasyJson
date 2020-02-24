using System;

namespace UniGames.EasyJson
{
	internal class JsonEnumController : JsonValueTypeController
	{
		public JsonEnumController()
		{
		}

		public override string ObjectToJson(object obj)
		{
			Type ut = Enum.GetUnderlyingType(obj.GetType());
			EasyJsonController controller = GetController(ut);
			if (controller == null)
			{
				throw new EasyJsonException("Doesn't support Enum parser whose underlying type is " + ut.ToString());
			}

			return controller.ObjectToJson(Convert.ChangeType(obj, ut));
		}

		public override object JsonToObject(EasyJsonData data, Type t)
		{
            try
            {
                return Enum.Parse(t, data.Data);
            }
            catch (Exception e)
            {
                throw new EasyJsonException("Fail to parse " + data.Data + " to " + t.ToString(), e);
            }
		}
	}
}

