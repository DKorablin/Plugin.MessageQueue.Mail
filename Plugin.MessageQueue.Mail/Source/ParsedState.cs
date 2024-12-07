using System;

namespace Plugin.MailMessageQueue.Data
{
	/// <summary>Статус парсинга сообщения</summary>
	internal enum ParsedState : int
	{
		/// <summary>Неопределёный статус, сообщение ещё не парсилось</summary>
		Undefined,
		/// <summary>Не нашлось обработчика сообщения</summary>
		NotSupported,
		/// <summary>Сообщение обработано успешно</summary>
		Success,
		/// <summary>Ошибка при разборке сообщения</summary>
		IncorrectMessage
	}
}