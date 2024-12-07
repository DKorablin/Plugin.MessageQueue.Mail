using System;

namespace Plugin.MailMessageQueue.Settings
{
	/// <summary>Настройки в JSON формате</summary>
	public class MessageSourceJsonItem
	{
		/// <summary>Тип источника писем</summary>
		public MessageSourceType Type { get; set; }

		/// <summary>Данные для парсинга в конкретную настройку</summary>
		public String Json { get; set; }
	}
}