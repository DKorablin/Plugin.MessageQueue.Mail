using SAL.Interface.MessageQueue.Mail;
using SAL.Interface.MessageQueue.Mail.Message;
using OpenPop.Mime;

namespace Plugin.MailMessageQueue.Data
{
	internal class OpenPop3MessageDto : MailMessageDto
	{
		public MessageInfo MessageInfo { get; }

		public OpenPop3MessageDto(MessageInfo messageInfo, Message message)
		{
			this.MessageInfo = messageInfo;
			base.Message = message.ToMailMessage();
			base.Header = new MailHeaderDto()
			{
				MessageId = message.Headers.MessageId,
				Date = message.Headers.DateSent,
				//SenderIp=?,
			};
		}
	}
}