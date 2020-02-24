using System;
using System.Text;

namespace UniGames.EasyJson
{
	internal static class JsonUtil
	{
		public const char STRING_QUOTE = '"';
		public const char OBJECT_START = '{';
		public const char OBJECT_END = '}';
		public const char ARRAY_START = '[';
		public const char ARRAY_END = ']';
		public const char KV_SEPARATOR = ':';
		public const char DATA_SEPARATOR = ',';
		public const char ESCAPE_SYMBOL = '\\';
		public static readonly string NULL_VALUE = "null";

		public static string Escape(string json)
		{
			if (json == null)
				return null;

			char[] targets = new char[]{ STRING_QUOTE, ESCAPE_SYMBOL };
			int idx = json.IndexOfAny(targets);
			if (idx > -1)
			{
				StringBuilder builder = new StringBuilder(json.Length * 2);
				int start = 0;
				do
				{
					builder.Append(json.Substring(start, idx - start));
					builder.Append(ESCAPE_SYMBOL).Append(json[idx]);

					start = ++idx;
					idx = json.IndexOfAny(targets, idx);
				} 
				while (idx > -1);

				if (start < json.Length)
				{
					builder.Append(json.Substring(start, json.Length - start));
				}

				return builder.ToString();
			}

			return json;
		}

		public static string Unescape(string json)
		{
			if (json == null)
				return null;

			int idx = json.IndexOf(ESCAPE_SYMBOL);
			if (idx > -1)
			{
				StringBuilder builder = new StringBuilder(json.Length * 2);
				builder.Append(json.Substring(0, idx));
				if ((++idx) == json.Length)
				{
					throw new EasyJsonException("Invalid escape symbol found");
				}
				builder.Append(json[idx]);

				int start;
				do 
				{
					start = ++idx;
					if (start == json.Length)
					{
						return builder.ToString();
					}

					idx = json.IndexOf(ESCAPE_SYMBOL, start);
					if (idx > -1)
					{
						builder.Append(json.Substring(start, idx - start));
						if ((++idx) == json.Length)
						{
							throw new EasyJsonException("Invalid escape symbol found");
						}
						builder.Append(json[idx]);
					}
					else
					{
						break;
					}
				}
				while (true);

				builder.Append(json.Substring(start, json.Length));
				return builder.ToString();
			}

			return json;
		}

		public static string GetString(string json, ref int start)
		{
			if (json == null)
				return null;

			SkipIgnoredChar(json, ref start, false);
			if (start < json.Length)
			{
				if (json[start] != STRING_QUOTE)
				{
					return null;
				}

				StringBuilder builder = new StringBuilder();
				int s = start + 1;

				for (int i = s; i < json.Length; ++i)
				{
					if (json[i] == ESCAPE_SYMBOL)
					{
						builder.Append(json.Substring(s, i - s));
						if ((++i) == json.Length)
						{
							throw new EasyJsonException("Invalid escape symbol found");
						}
						builder.Append(json[i]);
						s = i + 1;
					}
					else if (json[i] == STRING_QUOTE)
					{
						builder.Append(json.Substring(s, i - s));
						s = i;
						break;
					}
				}

				if (json[s] == STRING_QUOTE)
				{
					start = ++s;
					return builder.ToString();
				}
				else
				{
					throw new EasyJsonException("Bad JSON format");
				}
			}

			return null;
		}

		public static string GetData(string json, ref int start)
		{
			SkipIgnoredChar(json, ref start, false);
			StringBuilder builder = new StringBuilder(16);
			int s = start;
			
			bool reachEnd = false;
			for (int i = start; i < json.Length; ++i)
			{
				switch (json[i])
				{
					case ESCAPE_SYMBOL:
						builder.Append(json.Substring(s, i - s));
						if ((++i) == json.Length)
						{
							throw new EasyJsonException("Bad JSON format");
						}
						builder.Append(json[i]);
						s = i + 1;
						break;
						
					case OBJECT_END:
					case ARRAY_END:
					case DATA_SEPARATOR:
						reachEnd = true;
						break;
						
					case OBJECT_START:
					case ARRAY_START:
					case STRING_QUOTE:
					case KV_SEPARATOR:
						throw new EasyJsonException("Bad JSON format");
				}
				
				if (reachEnd)
				{
					start = i;
					break;
				}
			}
			
			if (!reachEnd)
			{
				start = json.Length;
			}
			
			if (s < start)
			{
				--start;
				JsonUtil.SkipIgnoredChar(json, ref start, true);
				++start;
				
				builder.Append(json.Substring(s, start - s));
				return builder.ToString();
			}
			else
			{
				return null;
			}
		}

		public static void SkipIgnoredChar(string json, ref int start, bool reverse)
		{
			while (start < json.Length)
			{
				switch (json[start])
				{
					case '\x09':
					case '\x0A':
					case '\x0B':
					case '\x0D':
					case '\x20':
						if (reverse)
						{
							--start;
						}
						else
						{
							++start;
						}
						continue;
				}
				
				break;
			}
		}

	}
}

