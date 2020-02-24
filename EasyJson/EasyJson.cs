using System;
using System.Collections.Generic;
using System.Threading;

namespace UniGames.EasyJson
{
	public static class EasyJson
	{
		static EasyJson()
		{
			__controllers = new Dictionary<int, EasyJsonController>();
		}

		public static string ToJson(object obj)
		{
			EasyJsonController controller = GetMainController();
			return controller.ObjectToJson(obj);
		}

		public static T ToObject<T>(string json)
		{
			EasyJsonController controller = GetMainController();
			return (T)(controller.JsonToObject(json, typeof(T)));
		}

        public static T ToObject<T>(EasyJsonData jsonData)
        {
            EasyJsonController controller = GetMainController();
            return (T)(controller.JsonToObject(jsonData, typeof(T)));
        }

        public static object ToObject(EasyJsonData jsonData, Type t)
        {
            EasyJsonController controller = GetMainController();
            return controller.JsonToObject(jsonData, t);
        }

		public static void RegisterController(Type t, EasyJsonController controller)
		{
			EasyJsonController current = GetMainController();
			current.RegisterController(t, controller);
		}

		public static void UnregisterController(Type t)
		{
			EasyJsonController current = GetMainController();
			current.UnregisterController(t);
		}

		internal static EasyJsonController GetMainController()
		{
			EasyJsonController controller = null;
			lock (__controllers)
			{
				int id = Thread.CurrentThread.ManagedThreadId;
				__controllers.TryGetValue(id, out controller);
				if (controller == null)
				{
					controller = new EasyJsonController();
					__controllers[id] = controller;
				}
			}

			return controller;
		}

		static Dictionary<int, EasyJsonController> __controllers;
	}
}

