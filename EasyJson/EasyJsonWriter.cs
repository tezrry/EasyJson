using System;
using System.Text;

namespace UniGames.EasyJson
{
	public class EasyJsonWriter
	{
		public EasyJsonWriter()
		{
			_writer = new StringBuilder(128);
		}

		public void WriteName(string name)
		{
			_writer
				.Append(JsonUtil.STRING_QUOTE)
				.Append(name)
				.Append(JsonUtil.STRING_QUOTE)
				.Append(JsonUtil.KV_SEPARATOR);
		}

        public void WriteValue(object value)
        {
            _writer.Append(EasyJson.ToJson(value));
            _writer.Append(JsonUtil.DATA_SEPARATOR);
        }
		
		public void WriteJsonValue(string json)
		{
            if (json == null)
			{
				_writer.Append(JsonUtil.NULL_VALUE);
			}
			else
			{
                _writer.Append(json);
			}

			_writer.Append(JsonUtil.DATA_SEPARATOR);
		}

		public void WriteObjectStart()
		{
			_writer.Append(JsonUtil.OBJECT_START);
		}

		public void WriteObjectEnd()
		{
			if (_writer[_writer.Length - 1] == JsonUtil.DATA_SEPARATOR)
			{
				_writer[_writer.Length - 1] = JsonUtil.OBJECT_END;
			}
			else
			{
				_writer.Append(JsonUtil.OBJECT_END);
			}
		}

		public void WriteArrayStart()
		{
			_writer.Append(JsonUtil.ARRAY_START);
		}

		public void WriteArrayEnd()
		{
			if (_writer[_writer.Length - 1] == JsonUtil.DATA_SEPARATOR)
			{
				_writer[_writer.Length - 1] = JsonUtil.ARRAY_END;
			}
			else
			{
				_writer.Append(JsonUtil.ARRAY_END);
			}
		}

		public override string ToString()
		{
			if (_writer[_writer.Length - 1] == JsonUtil.DATA_SEPARATOR)
			{
				_writer.Remove(_writer.Length - 1, 1);
			}

			return _writer.ToString();
		}

		StringBuilder _writer;
	}
}

