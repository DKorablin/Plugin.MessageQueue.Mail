using System;

namespace Plugin.MailMessageQueue.Settings
{
	public interface IMessageSourceSettingsItem
	{
		/// <summary>Источник данных валидный</summary>
		Boolean IsValid { get; }

		/// <summary>Уникальный ключ источника получения писем</summary>
		String Key { get; }
	}
}