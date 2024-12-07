using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.MailMessageQueue.Settings;
using SAL.Interface.MessageQueue.Mail;

namespace Plugin.MailMessageQueue.Data
{
	/// <summary>Базовый источник получения EMail из разных источников</summary>
	/// <remarks>Данный класс используется в Factory, источники используют класс <see cref="MessageSourceBase"/></remarks>
	public abstract class MailSource : IDisposable
	{
		private List<String> _errorMessageIDs = new List<String>();

		protected Plugin Plugin { get; }

		public IMessageSourceSettingsItem Settings { get; }

		protected TraceSource Trace { get; }

		/// <summary>Инициализровать базовый источник получения Email с инициализации трассировки</summary>
		/// <param name="plugin">Плагин</param>
		/// <param name="settings">Настройки</param>
		public MailSource(Plugin plugin, IMessageSourceSettingsItem settings)
		{
			this.Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
			this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));

			this.Trace = Plugin.CreateTraceSource<Plugin>("." + this.GetType().Name);
		}

		/// <summary>Выполнить парсинг всех новых сообщений в источнике получения почты</summary>
		public abstract void ParseMessages();

		/// <summary>Во время обработки сообщения произошла ошибка, поэтому сообщение отправлено в ошибочное</summary>
		/// <param name="messageId">Идентификатор сообщения для проверки</param>
		/// <returns>Во время обработки сообщения произошла ошибка и до следующего перезапуска сообщение обрабатываться не будет</returns>
		public Boolean IsErrorMessage(String messageId)
			=>  this._errorMessageIDs.Contains(messageId);

		/// <summary>Парсинг электронного сообщения</summary>
		/// <param name="message">Сообщение для парсинга</param>
		/// <returns>Сообщение успешно отпарсено</returns>
		internal Boolean ParseMail(MailMessageDto message)
		{
			if(!this.IsErrorMessage(message.Header.MessageId))
				try
				{
					if(this.Plugin.Target.HandleMessage(message))
						return true;
					else
						this.Trace.TraceEvent(TraceEventType.Warning, -8, "Error on parsing E-Mail Id: {0}", message.Header.MessageId);
				} catch(Exception exc)
				{
					if(Utils.IsFatal(exc))
						throw;
					else
						this.Trace.TraceData(TraceEventType.Error, -10, exc);

					this._errorMessageIDs.Add(message.Header.MessageId);
				}

			return false;
		}

		public virtual void Dispose()
		{
		}
	}
}