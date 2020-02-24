using System;
using System.Text;
using System.Collections.Generic;

namespace UniGames.EasyJson
{
	public enum EasyJsonDataType
	{
		Unknown,
		JsonString,
		JsonObject,
		JsonArray,
	}

	public class EasyJsonData
	{
		public static EasyJsonData Load(string json)
		{
			if (json == null)
				return null;

			int start = 0;
			EasyJsonData jsonData = __load(json, ref start);

			JsonUtil.SkipIgnoredChar(json, ref start, false);
			if (start < json.Length)
			{
				throw new EasyJsonException("Bad JSON format");
			}

			return jsonData;
		}

		static EasyJsonData __load(string json, ref int start)
		{
			JsonUtil.SkipIgnoredChar(json, ref start, false);
			if (start == json.Length)
			{
				return null;
			}
			
			EasyJsonDataType type = EasyJsonDataType.Unknown;
			switch (json[start])
			{
				case JsonUtil.OBJECT_START:
					type = EasyJsonDataType.JsonObject;
					break;

				case JsonUtil.ARRAY_START:
					type = EasyJsonDataType.JsonArray;
					break;

				case JsonUtil.STRING_QUOTE:
					type = EasyJsonDataType.JsonString;
					break;
			}
			
			EasyJsonData jsonData = new EasyJsonData(type);
			jsonData._load(json, ref start);

			return jsonData;
		}


		protected EasyJsonData(EasyJsonDataType type)
		{
			Type = type;
			switch (type)
			{
				case EasyJsonDataType.JsonObject:
					_object = new Dictionary<string, EasyJsonData>();
					break;

				case EasyJsonDataType.JsonArray:
					_array = new List<EasyJsonData>();
					break;

				default:
					_data = null;
					break;
			}
		}


		public EasyJsonDataType Type { get; private set; }

		public EasyJsonData this[string name]
		{
			get
			{
				if (_object == null)
				{
					throw new EasyJsonException("Can't get JSON data using STRING name");
				}

				EasyJsonData data = null;
				_object.TryGetValue(name, out data);

				return data;
			}
		}

		public string[] Names
		{
			get
			{
				if (_object == null)
				{
					throw new EasyJsonException("Can't get names for non JSON OBJECT");
				}

				string[] temp = new string[Count];
				_object.Keys.CopyTo(temp, 0);
				return temp;
			}
		}

		public EasyJsonData this[int index]
		{
			get
			{
				if (_array == null)
				{
					throw new EasyJsonException("Can't get JSON data using INT index");
				}

				return _array[index];
			}
		}

		public string Data
		{
			get
			{
				return _data;
			}
		}

		public int Count
		{
			get
			{
				switch (Type)
				{
					case EasyJsonDataType.JsonObject:
						return _object.Count;
						
					case EasyJsonDataType.JsonArray:
						return _array.Count;
						
					default:
						if (_data == null)
						{
							return 0;
						}
						else
						{
							return 1;
						}
				}
			}
		}

		public bool IsNullValue
		{
			get
			{
				return (Type == EasyJsonDataType.Unknown && _data == null);
			}
		}

        public int StartIdx
        {
            get
            {
                return _startIdx;
            }
        }

        public int EndIdx
        {
            get
            {
                return _endIdx;
            }
        }

        public override string ToString()
        {
            if (Type == EasyJsonDataType.JsonObject)
            {
                EasyJsonWriter writer = new EasyJsonWriter();
                writer.WriteObjectStart();
                foreach (string name in Names)
                {
                    writer.WriteName(name);
                    writer.WriteJsonValue(this[name].ToString());
                }
                writer.WriteObjectEnd();

                return writer.ToString();
            }
            else if (Type == EasyJsonDataType.JsonArray)
            {
                EasyJsonWriter writer = new EasyJsonWriter();
                writer.WriteArrayStart();
                for (int i = 0; i < Count; ++i)
                {
                    writer.WriteJsonValue(this[i].ToString());
                }
                writer.WriteArrayEnd();

                return writer.ToString();
            }
            else if (Type == EasyJsonDataType.JsonString)
            {
                return JsonUtil.STRING_QUOTE + Data + JsonUtil.STRING_QUOTE;
            }
            else
            {
                return Data;
            }
        }


		void _load(string json, ref int start)
		{
            _startIdx = start;
            switch (Type)
			{
				case EasyJsonDataType.JsonObject:
					_loadObject(json, ref start);
					break;

				case EasyJsonDataType.JsonArray:
					_loadArray(json, ref start);
					break;

				case EasyJsonDataType.JsonString:
					_loadString(json, ref start);
					break;

				default:
					_loadData(json, ref start);
					break;
			}
            _endIdx = start;
		}

		void _loadObject(string json, ref int start)
		{
			do
			{
				++start;
				string name = JsonUtil.GetString(json, ref start);
				if (name == null)
				{
					if (json[start] != JsonUtil.OBJECT_END)
					{
						throw new EasyJsonException("Bad JSON format");
					}

					break;
				}
				else
				{
					JsonUtil.SkipIgnoredChar(json, ref start, false);
					if (start == json.Length)
					{
						throw new EasyJsonException("Bad JSON format");
					}
					if (json[start] != JsonUtil.KV_SEPARATOR)
					{
						throw new EasyJsonException("Bad JSON format");
					}
					
					++start;
					EasyJsonData data = EasyJsonData.__load(json, ref start);
					
					_object.Add(name, data);
					
					JsonUtil.SkipIgnoredChar(json, ref start, false);
					if (start < json.Length)
					{
						if (json[start] == JsonUtil.DATA_SEPARATOR)
							continue;

						if (json[start] == JsonUtil.OBJECT_END)
						{
							break;
						}
					}

					throw new EasyJsonException("Bad JSON format");

				}
			}
			while (true);

			++start;
		}

		void _loadArray(string json, ref int start)
		{
            bool allIsNull = true;
            do
            {
                ++start;
                EasyJsonData data = EasyJsonData.__load(json, ref start);

                _array.Add(data);
                if (!data.IsNullValue)
                    allIsNull = false;

                JsonUtil.SkipIgnoredChar(json, ref start, false);
                if (start < json.Length)
                {
                    if (json[start] == JsonUtil.DATA_SEPARATOR)
                        continue;

                    if (json[start] == JsonUtil.ARRAY_END)
                    {
                        break;
                    }
                }

                throw new EasyJsonException("Bad JSON format");
            }
            while (true);

            if (allIsNull)
            {
                _array.Clear();
            }

            ++start;
		}

		void _loadString(string json, ref int start)
		{
			_data = JsonUtil.GetString(json, ref start);
		}

		void _loadData(string json, ref int start)
		{
			_data = JsonUtil.GetData(json, ref start);
			if (_data == JsonUtil.NULL_VALUE)
			{
				_data = null;
			}
		}

        
        Dictionary<string, EasyJsonData> _object;
		List<EasyJsonData> _array;
		string _data;

        int _startIdx;
        int _endIdx;
	}


}

