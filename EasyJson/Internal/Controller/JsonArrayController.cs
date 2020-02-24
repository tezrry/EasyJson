using System;

namespace UniGames.EasyJson
{
	internal class JsonArrayController : EasyJsonController
	{
		public JsonArrayController()
		{
		}

		public override string ObjectToJson(object obj)
		{
			if (obj == null)
				return null;

			Array arr = obj as Array;
			int[] idx = new int[arr.Rank];

			EasyJsonWriter writer = new EasyJsonWriter();
			_writeArray(arr, idx, 0, writer);

			return writer.ToString();
		}

		public override object JsonToObject(EasyJsonData data, Type t)
		{
			if (data.IsNullValue)
				return null;

			int rank = t.GetArrayRank();
			int[] lengths = new int[rank];

			EasyJsonData tmpData = data;
			for (int i = 0; i < rank; ++i)
			{
				if (tmpData.Type != EasyJsonDataType.JsonArray)
				{
					throw new EasyJsonException("Invalid JSON data type " + tmpData.Type.ToString() + ", JSON ARRAY expected");
				}

                if (0 ==(lengths[i] = tmpData.Count))
                    break;
                
				tmpData = tmpData[0];
			}

            Type et = t.GetElementType();
			Array inst = Array.CreateInstance(et, lengths);
			int[] idx = new int[rank];
            _readArray(inst, data, idx, 0, et);

			return inst;
		}

		void _writeArray(Array arr, int[] startIdx, int dim, EasyJsonWriter writer)
		{
			writer.WriteArrayStart();
			for (int i = 0; i < arr.GetLength(dim); ++i)
			{
				startIdx[dim] = i;
				if (dim == arr.Rank - 1)
				{
					//writer.WriteJsonValue(base.ObjectToJson(arr.GetValue(startIdx)));
                    writer.WriteValue(arr.GetValue(startIdx));
				}
				else
				{
					_writeArray(arr, startIdx, dim + 1, writer);
					writer.WriteJsonValue("");
				}
			}
			writer.WriteArrayEnd();
		}

		void _readArray(Array inst, EasyJsonData data, int[] startIdx, int dim, Type t)
		{
			for (int i = 0; i < inst.GetLength(dim); ++i)
			{
				startIdx[dim] = i;
				if (dim == inst.Rank - 1)
				{
                    inst.SetValue(base.JsonToObject(data[i], t), startIdx);
				}
				else
				{
					_readArray(inst, data[i], startIdx, dim + 1, t);
				}
			}

			startIdx[dim] = 0;
		}
	}
}

