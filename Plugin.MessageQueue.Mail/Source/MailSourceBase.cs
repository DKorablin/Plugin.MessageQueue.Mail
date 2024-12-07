using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SAL.Interface.MessageQueue.Mail;
using Plugin.MailMessageQueue.Settings;

namespace Plugin.MailMessageQueue.Data
{
	/// <summary>Базовый, строготипизированный, источник получения EMail из разных источников</summary>
	/// <typeparam name="T">Базовый класс, в котором передаётся информация о полученном сообщении</typeparam>
	public abstract class MailSourceBase<T> : MailSource, IEnumerable<T> where T : MailMessageDto
	{
		protected MailSourceBase(Plugin plugin, IMessageSourceSettingsItem settings)
			: base(plugin, settings)
		{
		}

		/// <summary>Получить список всех сообщений с сервера</summary>
		/// <returns>Электронные сообщения с сервера</returns>
		public abstract IEnumerator<T> GetEnumerator();

		public abstract void DisposeMessage(T message);

		IEnumerator IEnumerable.GetEnumerator()
			=> this.GetEnumerator();

		/// <summary>Получить все сообщения из разных источников получения почты</summary>
		public override void ParseMessages()
		{
			try
			{
				foreach(T message in this)
					if(this.ParseMail(message))
						this.DisposeMessage(message);
			} catch(Exception exc)
			{
				if(Utils.IsFatal(exc))
					throw;
				else
					base.Trace.TraceData(TraceEventType.Error, -9, exc);
			}
		}

		/// <summary>Парсинг электронного сообщения</summary>
		/// <param name="message">Сообщение для парсинга</param>
		/// <returns>Сообщение успешно отпарсено</returns>
		public Boolean ParseMail(T message)
			=> base.ParseMail(message);
	}
}