using System;
using System.Collections;
using System.Collections.Generic;

namespace UniGames.EasyJson
{
    internal class JsonDictionaryController : EasyJsonController
	{
        public JsonDictionaryController()
		{
            _metadataCache = new Dictionary<Type, JsonItemMetadata>();
		}

		public override string ObjectToJson(object obj)
		{
			if (obj == null) 
				return null;

			IDictionary dict = obj as IDictionary;
			EasyJsonWriter writer = new EasyJsonWriter();
			JsonItemMetadata itemMetadata = _getItemMetadata(obj.GetType());

			ObjectToJsonFunc keyFunc = _stringKeyToJsonName;
			if (itemMetadata.IndexType != typeof(string))
			{
				keyFunc = base.ObjectToJson;
			}

			writer.WriteObjectStart();
			foreach (DictionaryEntry entry in dict)
			{
				writer.WriteName(JsonUtil.Escape(keyFunc(entry.Key)));
				//writer.WriteJsonValue(base.ObjectToJson(entry.Value));
                writer.WriteValue(entry.Value);
			}
			writer.WriteObjectEnd();

			return writer.ToString();
		}

		public override object JsonToObject(EasyJsonData data, Type t)
		{
			if (data.IsNullValue)
				return null;

			if (data.Type != EasyJsonDataType.JsonObject)
			{
				throw new EasyJsonException("Invalid JSON data type " + data.Type.ToString() + ", JSON OBJECT expected");
			}
			
			IDictionary inst = Activator.CreateInstance(t) as IDictionary;
			JsonItemMetadata metadata = _getItemMetadata(t);
			
			JsonToObjectFunc keyFunc = _stringDataToKey;
			if (metadata.IndexType != typeof(string))
			{
				keyFunc = _jsonDataToKey;
			}
			
			foreach(string name in data.Names)
			{
				object key = keyFunc(name, metadata.IndexType);
				object value = base.JsonToObject(data[name], metadata.ElementType);
				
				inst.Add(key, value);
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

		string _stringKeyToJsonName(object str)
		{
			return str as string;
		}

		object _stringDataToKey(string str, Type t)
		{
			return str;
		}

		object _jsonDataToKey(string json, Type t)
		{
			EasyJsonData data = EasyJsonData.Load(json);
			return base.JsonToObject(data, t);
		}

		delegate string ObjectToJsonFunc(object obj);
		delegate object JsonToObjectFunc(string json, Type t);

        Dictionary<Type, JsonItemMetadata> _metadataCache;
	}
}

