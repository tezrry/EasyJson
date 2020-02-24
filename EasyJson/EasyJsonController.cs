using System;
using System.Collections.Generic;
using System.Reflection;

namespace UniGames.EasyJson
{
	public class EasyJsonController
	{
		static EasyJsonController()
		{
			__defaultControllers = new Dictionary<Type, EasyJsonController>();
			__defaultControllers[typeof(Type)] = new JsonSystemTypeController();
            __defaultControllers[typeof(Dictionary<,>)] = new JsonDictionaryController();
			__defaultControllers[typeof(List<>)] = new JsonListController();
			__defaultControllers[typeof(Array)] = new JsonArrayController();
			__defaultControllers[typeof(String)] = new JsonStringController();
			__defaultControllers[typeof(Int32)] = new JsonInt32Controller();
			__defaultControllers[typeof(Int16)] = new JsonInt16Controller();
			__defaultControllers[typeof(Int64)] = new JsonInt64Controller();
			__defaultControllers[typeof(Byte)] = new JsonByteController();
			__defaultControllers[typeof(Char)] = new JsonCharController();
			__defaultControllers[typeof(Single)] = new JsonFloatController();
			__defaultControllers[typeof(Double)] = new JsonDoubleController();
			__defaultControllers[typeof(Decimal)] = new JsonDecimalController();
			__defaultControllers[typeof(UInt16)] = new JsonUInt16Controller();
			__defaultControllers[typeof(UInt32)] = new JsonUInt32Controller();
			__defaultControllers[typeof(UInt64)] = new JsonUInt64Controller();
			__defaultControllers[typeof(Boolean)] = new JsonBoolController();
			__defaultControllers[typeof(Enum)] = new JsonEnumController();
            __defaultControllers[typeof(DateTime)] = new JsonDateTimeController();
            __defaultControllers[typeof(Nullable<>)] = new JsonNullableController();
            __defaultControllers[typeof(EasyJsonData)] = new JsonDataController();
		}

		static readonly string DYNAMIC_TYPE_NAME = "@type";
		static readonly int MAX_DEPTH = 128;

		static Dictionary<Type, EasyJsonController> __defaultControllers;

		public EasyJsonController()
		{
			_controllers = new Dictionary<Type, EasyJsonController>();
			_metadataCache = new Dictionary<Type, JsonClassMetadata>();
			_depth = 0;
            _checkedData = null;
		}

        protected EasyJsonController MainController
        {
            get
            {
                return EasyJson.GetMainController();
            }
        }

		public virtual string ObjectToJson(object obj)
		{
			EasyJsonWriter writer = new EasyJsonWriter();
            _depth = 0;
			_writeObject(obj, writer);

			return writer.ToString();
		}

		public virtual object JsonToObject(EasyJsonData data, Type t)
		{
			if (data == null)
			{
				return null;
			}

			return _readObject(data, t);
		}

        public void RegisterController(Type t, EasyJsonController controller)
        {
            _controllers[t] = controller;
        }

        public void UnregisterController(Type t)
        {
            _controllers.Remove(t);
        }

        internal object JsonToObject(string json, Type t)
		{
			EasyJsonData data = EasyJsonData.Load(json);
			return JsonToObject(data, t);
		}

        protected EasyJsonController GetController(Type t)
		{
            if (t.IsGenericType)
            {
                t = t.GetGenericTypeDefinition();
            }
			else if (t.IsEnum)
			{
				t = typeof(Enum);
			}
            else if (t.IsArray)
            {
                t = typeof(Array);
            }
            else if (t.IsSubclassOf(typeof(Type)))
            {
                t = typeof(Type);
            }

			EasyJsonController controller = null;
			_controllers.TryGetValue(t, out controller);
			if (controller == null)
			{
                if (MainController != this)
                {
                    controller = MainController.GetController(t);
                }
                else
                {
                    __defaultControllers.TryGetValue(t, out controller);
                }
			}

			return controller;
		}

		JsonClassMetadata _getMetadata(Type t)
		{
			JsonClassMetadata metadata = null;
			_metadataCache.TryGetValue(t, out metadata);
			if (metadata == null)
			{
				metadata = new JsonClassMetadata(t);
				_metadataCache[t] = metadata;
			}

			return metadata;
		}

		void _writeObject(object obj, EasyJsonWriter writer)
		{
			if (obj == null)
			{
				writer.WriteJsonValue(null);
				return;
			}

			if (++_depth > MAX_DEPTH)
			{
				throw new EasyJsonException("Class nested level is beyond max depth, check whether there is a loop");
			}

			Type t = obj.GetType();

			EasyJsonController controller = GetController(t);
			if (controller != null)
			{
				writer.WriteJsonValue(controller.ObjectToJson(obj));
				return;
			}
			
			writer.WriteObjectStart();

			JsonClassMetadata metadata = _getMetadata(t);
			foreach (KeyValuePair<string, FieldInfo> kv in metadata.Fields)
			{
				_writeObject(kv.Key, kv.Value.GetValue(obj), writer);
			}
			foreach (KeyValuePair<string, PropertyInfo> kv in metadata.Properties)
			{
				MethodInfo getMethod = kv.Value.GetGetMethod();
                if (getMethod == null)
                {
                    throw new EasyJsonException("Fail to access getter of " + kv.Value.Name);
                }

				if (getMethod.GetParameters().Length > 0)
					continue;

				_writeObject(kv.Key, getMethod.Invoke(obj, null), writer);
			}

			if (metadata.IsDynamicType)
			{
				_writeObject(DYNAMIC_TYPE_NAME, t, writer);
			}

			writer.WriteObjectEnd();
            writer.WriteJsonValue("");
		}

		void _writeObject(string name, object value, EasyJsonWriter writer)
		{
			writer.WriteName(name);
			_writeObject(value, writer);
		}

		object _readObject(EasyJsonData data, Type t)
		{
			if (data.IsNullValue)
				return null;

            try
            {
                // If user register a specific type controller but still invoke base JsonToObject, there
                // will make a loop: base -> GetController(t) -> child -> base
                if (_checkedData != data)
                {
                    _checkedData = data;

                    if (data.Type == EasyJsonDataType.JsonObject)
                    {
                        EasyJsonData typeData = data[DYNAMIC_TYPE_NAME];
                        if (typeData != null)
                        {
                            JsonSystemTypeController typeController = GetController(typeof(Type)) as JsonSystemTypeController;
                            if (typeController == null)
                            {
                                throw new EasyJsonException("Can't find System.Type controller to process dynamic type");
                            }
                            t = typeController.JsonToObject(typeData, typeof(Type)) as Type;
                        }
                    }

                    EasyJsonController controller = GetController(t);
                    if (controller != null)
                    {
                        return controller.JsonToObject(data, t);
                    }
                }

                if (data.Type != EasyJsonDataType.JsonObject)
                {
                    throw new EasyJsonException("Can't load JSON data for " + t.ToString());
                }

                object inst = Activator.CreateInstance(t);
                JsonClassMetadata metadata = _getMetadata(t);

                foreach (KeyValuePair<string, FieldInfo> kv in metadata.Fields)
                {
                    EasyJsonData fieldData = data[kv.Key];
                    if (fieldData != null)
                    {
                        FieldInfo field = kv.Value;
                        field.SetValue(inst, _readObject(fieldData, field.FieldType));
                    }
                }
                foreach (KeyValuePair<string, PropertyInfo> kv in metadata.Properties)
                {
                    EasyJsonData propertyData = data[kv.Key];
                    if (propertyData != null)
                    {
                        PropertyInfo property = kv.Value;
                        property.SetValue(inst, _readObject(propertyData, property.PropertyType), null);
                    }
                }

                return inst;
            }
            finally
            {
                _checkedData = null;
            }
		}

		Dictionary<Type, JsonClassMetadata> _metadataCache;
		Dictionary<Type, EasyJsonController> _controllers;
		int _depth;
        EasyJsonData _checkedData;
	}
}

