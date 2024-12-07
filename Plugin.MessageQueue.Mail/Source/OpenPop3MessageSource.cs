using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenPop.Common.Logging;
using OpenPop.Mime;
using OpenPop.Pop3;
using Plugin.MailMessageQueue.Settings;

namespace Plugin.MailMessageQueue.Data
{
	internal class OpenPop3MessageSource : MailSourceBase<OpenPop3MessageDto>, ILog
	{
		private readonly OpenPop3Settings _settings;
		private readonly Pop3Client _client;

		public OpenPop3MessageSource(Plugin plugin, OpenPop3Settings settings)
			: base(plugin, settings)
		{
			this._client = new Pop3Client();
			this._settings = settings;
			OpenPop.Common.Logging.DefaultLogger.SetLog(this);
		}

		public override IEnumerator<OpenPop3MessageDto> GetEnumerator()
		{
			this._client.Connect(this._settings.Server, this._settings.Port, this._settings.UseSSL);

			try
			{
				this._client.Authenticate(this._settings.Login, this._settings.Password);

				foreach(MessageInfo messageInfo in this._client.GetMessageInfos())
				{
					if(base.IsErrorMessage(messageInfo.Identifier))
						continue;

					var message = this._client.GetMessage(messageInfo.Number);
					OpenPop3MessageDto parser = new OpenPop3MessageDto(messageInfo, message);
					yield return parser;
				}
			} finally
			{
				this._client.Disconnect();
			}
		}

		public override void DisposeMessage(OpenPop3MessageDto message)
		{
			this._client.DeleteMessage(message.MessageInfo.Number);
		}

		public override void Dispose()
		{
			this._client.Dispose();
			base.Dispose();
		}

		#region ILog
		public void LogDebug(String message)
		{
		}

		public void LogError(String message)
		{
			base.Plugin.Trace.TraceEvent(TraceEventType.Error, 1, message);
		}
		#endregion ILog
	}
}