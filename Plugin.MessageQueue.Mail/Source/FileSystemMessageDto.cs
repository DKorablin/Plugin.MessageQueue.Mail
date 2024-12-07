using System;
using ActiveUp.Net.Mail;
using SAL.Interface.MessageQueue.Mail;

namespace Plugin.MailMessageQueue.Data
{
	internal class FileSystemMessageDto : MailMessageDto
	{
		public String FilePath { get; }

		public FileSystemMessageDto(String filePath)
		{
			this.FilePath = filePath;
			ActiveUp.Net.Mail.Message message = Parser.ParseMessageFromFile(filePath);
			base.Message = Dto.Convert(message);
			base.Header = new SAL.Interface.MessageQueue.Mail.Message.MailHeaderDto()
			{
				MessageId = message.MessageId,
				Date = message.Date,
				SenderIp = message.SenderIP
			};
		}
	}
}