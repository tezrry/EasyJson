using System;

namespace UniGames.EasyJson
{
    internal class JsonDataController : EasyJsonController
    {
        public JsonDataController()
        {
        }

        public override string ObjectToJson(object obj)
        {
            throw new EasyJsonException
            (
                "There is no value to transform EasyJsonData to Json", 
                new NotSupportedException("JsonDataController.ObjectToJson")
            );
        }

        public override object JsonToObject(EasyJsonData data, Type t)
        {
            return data;
        }
    }
}

