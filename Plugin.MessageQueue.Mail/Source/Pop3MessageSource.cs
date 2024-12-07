using System;
using System.Collections.Generic;
using ActiveUp.Net.Mail;
using Plugin.MailMessageQueue.Settings;

namespace Plugin.MailMessageQueue.Data
{
	internal class Pop3MessageSource : MailSourceBase<Pop3MessageDto>
	{
		private readonly Pop3Settings _settings;
		private readonly Pop3Client _client;

		public Pop3MessageSource(Plugin plugin, Pop3Settings settings)
			: base(plugin, settings)
		{
			this._client = new Pop3Client();
			this._settings = settings;
		}

		public override IEnumerator<Pop3MessageDto> GetEnumerator()
		{
			if(base.Plugin.Settings.POP3UseSSL)
				this._client.ConnectSsl(this._settings.Server, this._settings.Port);
			else
				this._client.Connect(this._settings.Server, this._settings.Port);

			try
			{
				this._client.Authenticate(this._settings.Login, this._settings.Password, SaslMechanism.Login);

				foreach(Pop3Client.PopServerUniqueId messageIndex in this._client.GetUniqueIds())
				{
					Byte[] data = this._client.RetrieveMessage(messageIndex.Index, false);
					Message message = Parser.ParseMessage(data);
					yield return new Pop3MessageDto(messageIndex, message);
				}
			} finally
			{
				this._client.Disconnect();
			}
		}

		public override void DisposeMessage(Pop3MessageDto message)
		{
			this._client.DeleteMessage(message.MessageIndex.Index);
		}

		public override void Dispose()
		{
			base.Dispose();
		}
	}
}