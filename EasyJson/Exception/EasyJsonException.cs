using System;

namespace UniGames.EasyJson
{
	public class EasyJsonException : ApplicationException
	{
		public EasyJsonException()
		{
		}

		public EasyJsonException(string msg)
			:base(msg)
		{
		}

		public EasyJsonException(string msg, Exception e)
			:base(msg, e)
		{
		}
	}
}

