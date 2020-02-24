using System;

namespace UniGames.EasyJson
{
    internal class JsonNullableController : JsonValueTypeController
    {
        public JsonNullableController()
        {
        }

        public override string ObjectToJson(object obj)
        {
            if (obj == null)
                return null;

            Type ut = Nullable.GetUnderlyingType(obj.GetType());
            EasyJsonController controller = GetController(ut);
            if (controller == null)
            {
                throw new EasyJsonException("Doesn't support JSON parser for Nullable " + ut.ToString());
            }
            
            return controller.ObjectToJson(obj);
        }

        public override object JsonToObject(EasyJsonData data, Type t)
        {
            if (data.IsNullValue)
                return null;
            
            Type ut = Nullable.GetUnderlyingType(t);
            EasyJsonController controller = GetController(ut);
            if (controller == null)
            {
                throw new EasyJsonException("Doesn't support JSON parser for Nullable " + ut.ToString());
            }

            return controller.JsonToObject(data, ut);
        }
    }
}

