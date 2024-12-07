using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.MailMessageQueue.Settings;

namespace Plugin.MailMessageQueue.Data
{
	/// <summary>Коллекция инициализированных источников писем</summary>
	public class MailSourceCollection : IDisposable, IEnumerable<MailSource>
	{
		private readonly Plugin _plugin;
		private readonly List<MailSource> _messageSource = new List<MailSource>();

		/// <summary>Количество запущенных источников данных</summary>
		public Int32 Count { get { return this._messageSource.Count; } }

		internal MailSourceCollection(Plugin plugin)
		{
			this._plugin = plugin;

			foreach(IMessageSourceSettingsItem settings in this._plugin.Settings.Data)
				this.Add(settings);
		}

		/// <summary>Добавить новый источник получения электронной почты исходя из настроек</summary>
		/// <param name="settings">Настройки для провайдера получения почты</param>
		public void Add(IMessageSourceSettingsItem settings)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");

			if(settings is Imap4Settings)
				this._messageSource.Add(new Imap4MessageSource(this._plugin, (Imap4Settings)settings));
			else if(settings is Pop3Settings)
				this._messageSource.Add(new Pop3MessageSource(this._plugin, (Pop3Settings)settings));
			else if(settings is OpenPop3Settings)
				this._messageSource.Add(new OpenPop3MessageSource(this._plugin, (OpenPop3Settings)settings));
			else if(settings is FileSystemSettings)
				this._messageSource.Add(new FileSystemMessageSource(this._plugin, (FileSystemSettings)settings));
			else
				throw new NotImplementedException();
		}

		/// <summary>Удалить источник получения электронной почты исходя из ключа настроек</summary>
		/// <param name="key">Уникальный ключ, который идентифицирует настройки</param>
		public void Remove(IMessageSourceSettingsItem settings)
		{
			if(settings == null)
				throw new ArgumentNullException("settings");

			foreach(MailSource source in this._messageSource)
				if(source.Settings.Key == settings.Key)
				{
					if(!this._messageSource.Remove(source))
						throw new InvalidOperationException();

					source.Dispose();
					break;
				}
		}

		/// <summary>Почистить все ресуры занятые ресурсы</summary>
		public void Dispose()
		{
			foreach(MailSource source in this._messageSource)
				try
				{
					source.Dispose();
				} catch(Exception exc)
				{
					if(Utils.IsFatal(exc))
						throw;
					else
						this._plugin.Trace.TraceData(TraceEventType.Error, -11, exc);
				}
		}

		/// <summary>Получить массив всех инициализированных источников получения почты</summary>
		/// <returns>Источник получения почты</returns>
		public IEnumerator<MailSource> GetEnumerator()
		{
			foreach(MailSource source in this._messageSource)
				yield return source;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}