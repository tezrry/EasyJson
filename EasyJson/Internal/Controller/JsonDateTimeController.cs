using System;

namespace UniGames.EasyJson
{
    internal class JsonDateTimeController : JsonValueTypeController
    {
        public JsonDateTimeController()
        {
        }

        public override string ObjectToJson(object obj)
        {
            DateTime instance = ((DateTime)obj).ToUniversalTime();
            string value = JsonUtil.Escape(instance.ToString("yyyy-MM-dd'T'HH:mm:ss.fffZ"));//ISO8601
            return JsonUtil.STRING_QUOTE + value + JsonUtil.STRING_QUOTE;
        }

        public override object JsonToObject(EasyJsonData data, Type t)
        {
            return DateTime.Parse(data.Data).ToUniversalTime();
        }
    }
}

