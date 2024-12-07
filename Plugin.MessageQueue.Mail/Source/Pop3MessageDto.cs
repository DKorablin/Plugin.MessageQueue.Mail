using System;
using ActiveUp.Net.Mail;
using SAL.Interface.MessageQueue.Mail;

namespace Plugin.MailMessageQueue.Data
{
	internal class Pop3MessageDto : MailMessageDto
	{
		internal Pop3Client.PopServerUniqueId MessageIndex { get; }

		public Pop3MessageDto(Pop3Client.PopServerUniqueId messageIndex, Message message)
		{
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