using System;
using System.Web.Script.Serialization;

namespace Plugin.MailMessageQueue
{
	/// <summary>Сериализация</summary>
	internal static class Serializers
	{
		private static JavaScriptSerializer _serializer;

		private static JavaScriptSerializer Serializer
		{
			get
			{
				if(_serializer == null)
				{
					_serializer = new JavaScriptSerializer();
					//_serializer.RegisterConverters(new JavaScriptConverter[] { new TimeSpanJsonConverter(), new WorkHoursJsonConverter(), });
				}
				return _serializer;
			}
		}

		/// <summary>Десериализовать строку в объект</summary>
		/// <typeparam name="T">Тип объекта</typeparam>
		/// <param name="json">Строка в формате JSON</param>
		/// <returns>Десериализованный объект</returns>
		internal static T JavaScriptDeserialize<T>(String json)
			=> String.IsNullOrEmpty(json)
				? default
				: Serializers.Serializer.Deserialize<T>(json);

		/// <summary>Сериализовать объект</summary>
		/// <param name="item">Объект для сериализации</param>
		/// <returns>Строка в формате JSON</returns>
		internal static String JavaScriptSerialize(Object item)
		{
			return item == null
				? null
				: Serializers.Serializer.Serialize(item);
		}
	}
}