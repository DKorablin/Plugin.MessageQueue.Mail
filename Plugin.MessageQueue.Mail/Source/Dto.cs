using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using SalMessage = SAL.Interface.MessageQueue.Mail.Message;

namespace Plugin.MailMessageQueue.Data
{
	internal static class Dto
	{
		public static MailMessage Convert(ActiveUp.Net.Mail.Message message)
		{
			MailMessage result = new MailMessage();
			if(message.Attachments != null)
				foreach(Attachment attachment in Dto.Convert(message.Attachments.Cast<ActiveUp.Net.Mail.MimePart>()))
					result.Attachments.Add(attachment);
			result.IsBodyHtml = message.IsHtml;
			result.Body = message.IsHtml ? message.BodyHtml.Text : message.BodyText.Text;
			foreach(var mail in message.Bcc)
				result.Bcc.Add(Dto.Convert(mail));

			foreach(var mail in message.Cc)
				result.CC.Add(Dto.Convert(mail));

			result.Sender = Dto.Convert(message.Sender);
			result.From = Dto.Convert(message.From);

			foreach(var mail in message.To)
				result.To.Add(Dto.Convert(mail));

			result.Subject = message.Subject;
			if(message.ReplyTo != null)
				result.ReplyToList.Add(Dto.Convert(message.ReplyTo));

			switch(message.Priority)
			{
			case ActiveUp.Net.Mail.MessagePriority.High:
				result.Priority = MailPriority.High;
				break;
			case ActiveUp.Net.Mail.MessagePriority.Low:
				result.Priority = MailPriority.Low;
				break;
			case ActiveUp.Net.Mail.MessagePriority.Normal:
				result.Priority = MailPriority.Normal;
				break;
			}
			return result;
		}

		public static IEnumerable<Attachment> Convert(IEnumerable<ActiveUp.Net.Mail.MimePart> attachments)
		{
			Int32 fileIndex = 0;
			foreach(var attachment in attachments)
			{
				MemoryStream stream = new MemoryStream(attachment.BinaryContent);
				String fileName = attachment.Filename;
				if(fileName.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
				{
					String extension = Path.GetExtension(attachment.Filename);
					if(String.IsNullOrEmpty(extension))
						extension = ".unk";
					fileName = "attachment_" + fileIndex.ToString() + extension;
				}
				yield return new Attachment(stream, attachment.Filename, attachment.MimeType) { ContentId = attachment.ContentId };
			}
		}

		public static MailMessage Convert(OpenPop.Mime.Message message)
		{
			return message.ToMailMessage();
			/*MailMessage result = new MailMessage();
			IEnumerable<MessagePart> attachments = message.FindAllAttachments();

			Dictionary<String, Int32> fileNames = new Dictionary<String, Int32>();
			Dictionary<Int32, String> indexFileNames = new Dictionary<Int32, String>();
			Int32 fileIndex = 0;
			foreach(MessagePart attachment in attachments)
			{
				String fullFileName = "file_" + fileIndex.ToString() + ".tmp";
				if(!attachment.FileName.Any(p => Path.GetInvalidFileNameChars().Contains(p)))
					fullFileName = attachment.FileName;

				String fileName = Path.GetFileNameWithoutExtension(fullFileName);
				if(fileNames.ContainsKey(fileName))
					fileNames[fileName]++;
				else
					fileNames.Add(fileName, 0);

				if(fileNames[fileName] > 0)
					fileName += fileNames[fileName].ToString();

				indexFileNames.Add(fileIndex, fileName + Path.GetExtension(fullFileName));
				fileIndex++;
			}

			List<MessagePart> attachmentsArray = message.FindAllAttachments();
			List<Attachment> newContentFiles = new List<Attachment>();
			List<Attachment> fullContentFiles = new List<Attachment>();

			for(Int32 index = 0; index < attachmentsArray.Count; index++)
			{
				if(attachments.Contains(attachmentsArray[index]))
					newContentFiles.Add(Dto.Convert(indexFileNames[index], attachmentsArray[index]));
				result.Attachments.Add(Dto.Convert(indexFileNames[index], attachmentsArray[index]));
			}

			MessagePart textPart = message.FindFirstHtmlVersion();
			if(textPart == null)
				textPart = message.FindFirstPlainTextVersion();

			String body = String.Empty;
			Encoding bodyEncoding = null;
			if(textPart != null)
			{
				bodyEncoding = textPart.BodyEncoding;
				if(!String.IsNullOrWhiteSpace(textPart.ContentType.CharSet))
					bodyEncoding = Encoding.GetEncoding(textPart.ContentType.CharSet);

				result.BodyEncoding = bodyEncoding;
				//result.BodyTransferEncoding;
				result.Body = bodyEncoding.GetString(textPart.Body);
				result.IsBodyHtml = textPart.ContentType.MediaType==System.Net.Mime.MediaTypeNames.Text.Html;
			}
			foreach(var mail in message.Headers.Bcc)
				result.Bcc.Add(Dto.Convert(mail));

			foreach(var mail in message.Headers.Cc)
				result.CC.Add(Dto.Convert(mail));

			result.Sender = Dto.Convert(message.Headers.Sender);
			result.From = Dto.Convert(message.Headers.From);

			foreach(var mail in message.Headers.To)
				result.To.Add(Dto.Convert(mail));

			result.Subject = message.Headers.Subject;
			if(message.Headers.ReplyTo != null)
				result.ReplyToList.Add(Dto.Convert(message.Headers.ReplyTo));

			return result;*/
		}

		public static SalMessage.MailPart Convert(OpenPop.Mime.MessagePart part)
		{
			return new SalMessage.MailPart()
			{
				Body = part.Body,
				BodyEncoding = part.BodyEncoding,
				ContentDescription = part.ContentDescription,
				ContentDisposition = part.ContentDisposition,
				ContentId = part.ContentId,
				ContentType = part.ContentType,
				FileName = part.FileName,
				MessageParts = part.MessageParts.Select(p => Dto.Convert(p)).ToArray(),
			};
		}

		public static SalMessage.MailHeaderDto Convert(OpenPop.Mime.Header.MessageHeader header)
		{
			return new SalMessage.MailHeaderDto()
			{
				Date = header.DateSent,
				MessageId = header.MessageId,
			};
		}

		public static MailAddress Convert(OpenPop.Mime.Header.RfcMailAddress mailAddress)
		{
			if(mailAddress == null)
				return null;

			return mailAddress.HasValidMailAddress
				? mailAddress.MailAddress
				: new MailAddress(mailAddress.Address);
		}

		public static MailAddress Convert(ActiveUp.Net.Mail.Address address)
		{
			if(address == null)
				return null;

			return new MailAddress(address.Email, address.Name);
		}

		public static Attachment Convert(String fileName, OpenPop.Mime.MessagePart part)
		{
			MemoryStream stream = new MemoryStream();
			part.Save(stream);
			return new Attachment(stream, fileName)
			{
				ContentId = part.ContentId,
				ContentType = part.ContentType,
				NameEncoding = part.BodyEncoding,
			};
		}
	}
}