using System;
using System.Collections;
using System.Collections.Generic;

namespace UniGames.EasyJson
{
    internal class JsonListController : EasyJsonController
	{
        public JsonListController()
		{
            _metadataCache = new Dictionary<Type, JsonItemMetadata>();
		}

		public override string ObjectToJson(object obj)
		{
			if (obj == null)
				return null;

			IList list = obj as IList;
			EasyJsonWriter writer = new EasyJsonWriter();

			writer.WriteArrayStart();
			foreach (object element in list)
			{
				//writer.WriteJsonValue(base.ObjectToJson(element));
                writer.WriteValue(element);
			}
			writer.WriteArrayEnd();

			return writer.ToString();
		}

		public override object JsonToObject(EasyJsonData data, Type t)
		{
			if (data.IsNullValue)
				return null;

			if (data.Type != EasyJsonDataType.JsonArray)
			{
				throw new EasyJsonException("Invalid JSON data type " + data.Type.ToString() + ", JSON ARRAY expected");
			}

			JsonItemMetadata metadata = _getItemMetadata(t);
			IList inst = Activator.CreateInstance(t) as IList;

			for (int i = 0; i < data.Count; ++i)
			{
				object element = base.JsonToObject(data[i], metadata.ElementType);
                inst.Add(element);
			}

			return inst;
		}

        JsonItemMetadata _getItemMetadata(Type t)
        {
            JsonItemMetadata metadata = null;
            _metadataCache.TryGetValue(t, out metadata);
            if (metadata == null)
            {
                metadata = new JsonItemMetadata(t);
                _metadataCache[t] = metadata;
            }

            return metadata;
        }

        Dictionary<Type, JsonItemMetadata> _metadataCache;
	}
}

