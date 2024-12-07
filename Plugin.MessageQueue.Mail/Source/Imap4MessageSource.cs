using System;
using System.Collections.Generic;
using ActiveUp.Net.Mail;
using Plugin.MailMessageQueue.Settings;

namespace Plugin.MailMessageQueue.Data
{
	internal class Imap4MessageSource : MailSourceBase<Imap4MessageDto>
	{
		private readonly Imap4Settings _settings;
		private readonly Imap4Client _client;

		public Imap4MessageSource(Plugin plugin, Imap4Settings settings)
			: base(plugin, settings)
		{
			this._settings = settings;
			this._client = new Imap4Client();
		}

		public override IEnumerator<Imap4MessageDto> GetEnumerator()
		{
			this._client.Connect(this._settings.Server, this._settings.Port);

			try
			{
				this._client.LoginFast(this._settings.Login, this._settings.Password);

				Mailbox mailbox = this._client.SelectMailbox(this._settings.DefaultInbox);
				foreach(Int32 messageIndex in mailbox.Search("ALL"))
				{
					Byte[] data = mailbox.Fetch.Message(messageIndex);
					Message message = Parser.ParseMessage(data);
					yield return new Imap4MessageDto(mailbox, messageIndex, message);
				}
			} finally
			{
				this._client.Disconnect();
			}
		}

		public override void DisposeMessage(Imap4MessageDto message)
		{
			message.Mailbox.DeleteMessage(message.MessageIndex, true);
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}