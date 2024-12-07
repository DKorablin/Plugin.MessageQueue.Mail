using System;
using ActiveUp.Net.Mail;
using SAL.Interface.MessageQueue.Mail;

namespace Plugin.MailMessageQueue.Data
{
	internal class Imap4MessageDto : MailMessageDto
	{
		public Mailbox Mailbox { get; }
		public Int32 MessageIndex { get; }

		public Imap4MessageDto(Mailbox mailbox, Int32 messageIndex, Message message)
		{
			this.Mailbox = mailbox;
			this.MessageIndex = messageIndex;

			base.Message = Dto.Convert(message);
			base.Header = new SAL.Interface.MessageQueue.Mail.Message.MailHeaderDto()
			{
				Date = message.ReceivedDate,
				MessageId = message.MessageId,
				SenderIp = message.SenderIP,
			};
		}
	}
}