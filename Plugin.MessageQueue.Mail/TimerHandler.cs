using System;
using Plugin.MailMessageQueue.Data;
using Plugin.MailMessageQueue.Plugins;
using SAL.Interface.MessageQueue.Mail;

namespace Plugin.MailMessageQueue
{
	internal class TimerHandler : TimerHandlerBase
	{
		/// <summary>Источник парсинга сообщений</summary>
		public enum ParseSource
		{
			/// <summary>Брать сообщения с сервера POP3</summary>
			POP3Server,
			/// <summary>Брать сообщения с файловой системы</summary>
			FileSystem,
		}

		private readonly Plugin _plugin;

		public TimerHandler(Plugin plugin)
			: base(plugin.Host, "Plugin.MailMessageQueue")
			=> this._plugin = plugin;

		protected override void OnInvokeTimer(Object state, EventArgs args)
		{
			foreach(MailSourceBase<MailMessageDto> source in this._plugin.Source)
				source.ParseMessages();
		}
	}
}