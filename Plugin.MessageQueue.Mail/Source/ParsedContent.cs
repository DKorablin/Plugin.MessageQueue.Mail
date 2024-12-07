using SAL.Interface.MessageQueue.Mail;

namespace Plugin.MailMessageQueue.Data
{
	public class ParsedContent : MailMessageDto
	{
		public System.Net.Mail.Attachment[] Attachments { get; set; }

		public System.Net.Mail.Attachment[] FullAttachments { get; set; }
	}
}