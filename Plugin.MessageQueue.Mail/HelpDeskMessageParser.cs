using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Interface.MailMessageQueue.Message;
using OpenPop.Mime;
using OpenPop.Mime.Header;
using Plugin.MailMessageQueue.Data;
using Plugin.MailMessageQueue.Plugins;

namespace Plugin.MailMessageQueue
{
	/// <summary>Результат парсинга</summary>
	internal class HelpDeskMessageParserResult
	{
		/// <summary>Результат выполнения парсинга</summary>
		public ParsedState Status { get; set; }
		/// <summary>Ошибка в случае возникновения</summary>
		public Exception Exception { get; set; }
		/// <summary>Сообщение, которое парсилось</summary>
		public Message MailMessage { get; set; }
		/// <summary>Информация о сообщении</summary>
		public MessageInfo MessageInfo { get; set; }
	}

	/// <summary>Парсер сообщений электронной почты</summary>
	internal class HelpDeskMessageParser
	{
		private static Regex newMessageLineRegex = new Regex(@"[\-]{2,20}[\=]{2,20}\[.*\][\=]{2,20}[\-]{2,20}", RegexOptions.Compiled);
		private static Regex htmlBodyRegex = new Regex(@"<body[^>]*>.*</body[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
		private static Regex scriptTagRegex = new Regex(@"(<script[^*>]?)+?[^.]*?(</script>)+?", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
		private static Regex styleTagRegex = new Regex(@"(<style[^*>]?)+?[^.]*?(</style>)+?", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
		private static Regex imageIdRegex = new Regex("<img[^>]+src=\"cid:([^\"]+)\"[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
		private static Regex htmlTagsRegex = new Regex(@"</?([^>]+)>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
		private static Regex htmlNewLineTagsRegex = new Regex(@"<(br|p|h|div)+([^>]*)>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
		private static Regex textEmptyLineTagsRegex = new Regex(@"^\s+$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static Regex specialHtmlCharsRegex = new Regex(@"&.{2,5};", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static Regex removeHeadersRegex = new Regex(@"^From:[^\n]*\n[\r\n]*Sent:[^\n]*\n[\r\n]*To:[^\n]*\n[\r\n]*Subject:[^\n]*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
		private static Regex removeHeadersRegex2 = new Regex(@"^(Понедельник|Вторник|Среда|Четверг|Пятница|Суббота|Воскресенье), [0-9]{1,2} (января|февраля|марта|апреля|мая|июня|июля|августа|сентября|октября|ноября|декабря) [0-9]{4}, [0-9]{2}:[0-9]{2} \+[0-9]{2}:[0-9]{2} от hd@exist.ru:", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
		private static Regex removeHeadersRegex3 = new Regex(@"^[0-9]{2}\.[0-9]{2}\.[0-9]{4} [0-9]{2}:[0-9]{2}, hd@exist.ru пишет:", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
		private static Char[] invalidPathChars = Path.GetInvalidFileNameChars();


		//private String imageIdRegexString = "<img[^>]+src=\"cid:{0}([^\"]?)\"[^>]*>";

		/// <summary>Класс для работы с данными</summary>
		public MailPluginsStorage Storage { get; private set; }

		/// <summary>Конструктор</summary>
		/// <param name="messageSaver">Класс для работы с итоговыми данными</param>
		public HelpDeskMessageParser(MailPluginsStorage storage)
		{
			this.Storage = storage;
		}

		/// <summary>Проверяет - существует ли заявка с таким идентификатором в БД</summary>
		/// <param name="questionId"></param>
		/// <returns></returns>
		private Boolean IsQuestionExists(String questionId)
		{
			return true;
		}

		/// <summary>Получить слова письма вместе с местами, где они встретились</summary>
		/// <param name="text">Текст для анализа</param>
		/// <returns>Список слов и мест, где они встретились</returns>
		private Tuple<String, Int32>[] GetWordsIndex(String text)
		{
			List<Tuple<String, Int32>> result = new List<Tuple<String, Int32>>();
			String[] words = Utils.RemoveTags(text).Split(new Char[] { ' ', '\r', '\n', ',', ':', '!', '?', '-' }, StringSplitOptions.RemoveEmptyEntries);
			Int32 lastIndex = 0;
			foreach(String word in words)
			{
				lastIndex = text.IndexOf(word, lastIndex);
				result.Add(new Tuple<String, Int32>(word, lastIndex));
			}
			return result.ToArray();
		}

		/// <summary>Берём тело html и вычищаем изнутри скрипты и стили</summary>
		/// <param name="htmlString">Целевая строка</param>
		/// <returns>Тело HTML без скриптов и стилей</returns>
		private String GetHtmlBodyContent(String htmlString)
		{
			String htmlBody = htmlString;
			if(htmlBody.IndexOf("<body", StringComparison.InvariantCultureIgnoreCase) > -1)
				htmlBody = htmlBodyRegex.Match(htmlString).Value;
			return styleTagRegex.Replace(scriptTagRegex.Replace(htmlBody, String.Empty), String.Empty);
		}

		/// <summary>Заменить изображения в html на текст "см. приложенный файл filename"</summary>
		/// <param name="htmlString">Строка html</param>
		/// <param name="attachmentsList">Список присоединённых файлов</param>
		/// <param name="indexFileNames">Индексы файлов</param>
		/// <param name="imageFilesId">Идентификаторы файлов</param>
		/// <returns></returns>
		private String ReplaceImageReferences(String htmlString, IEnumerable<MessagePart> attachmentsList, Dictionary<Int32, String> indexFileNames, out List<String> imageFilesId)
		{
			imageFilesId = new List<String>();
			StringBuilder result = new StringBuilder();
			MatchCollection imageRefs = imageIdRegex.Matches(htmlString);
			Int32 lastMatchIndex = 0;

			MessagePart[] attachments = attachmentsList.ToArray();

			foreach(Match imageRef in imageRefs)
			{
				if(!imageRef.Success)
					continue;

				String value = imageRef.Result("$1");

				MessagePart part = attachments.FirstOrDefault(p => p.ContentId == value);
				if(part != null)
				{
					Int32 fileNameIndex = Array.IndexOf(attachments, part);
					String fileName = indexFileNames[fileNameIndex];
					result.Append(htmlString.Substring(lastMatchIndex, imageRef.Index - lastMatchIndex));
					result.AppendFormat("см. вложение [{0}]/{1}/", fileName, part.ContentId);
					imageFilesId.Add(part.ContentId);
					lastMatchIndex = imageRef.Index + imageRef.Length;
				}
			}

			if(lastMatchIndex == 0)
				return htmlString;

			if(lastMatchIndex < htmlString.Length)
				result.Append(htmlString.Substring(lastMatchIndex));

			return result.ToString();
		}

		/// <summary>Отправка сообщения на обработку парсером</summary>
		/// <param name="mailMessage">Сообщение электронной почты</param>
		/// <param name="messageInfo">Информация о сообщении</param>
		/// <returns>Результат в виде <see cref="T:HelpDeskMailParser.HelpDeskMessageParserResult"/></returns>
		public Task<HelpDeskMessageParserResult> ParseMessage(Message mailMessage, MessageInfo messageInfo)
		{
			return Task.Factory.StartNew<HelpDeskMessageParserResult>(() =>
			{
				HelpDeskMessageParserResult result = new HelpDeskMessageParserResult
				{
					MailMessage = mailMessage,
					MessageInfo = messageInfo
				};

				try
				{
					IEnumerable<MessagePart> attachments = mailMessage.FindAllAttachments();

					Dictionary<String, Int32> fileNames = new Dictionary<String, Int32>();
					Dictionary<Int32, String> indexFileNames = new Dictionary<Int32, String>();
					Int32 fileIndex = 0;
					foreach(MessagePart attachment in attachments)
					{
						String fullFileName = "file_" + fileIndex.ToString() + ".tmp";
						if(!attachment.FileName.Any(p => invalidPathChars.Contains(p)))
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

					MessagePart textPart = mailMessage.FindFirstHtmlVersion();
					if(textPart == null)
						textPart = mailMessage.FindFirstPlainTextVersion();

					String mailMessageString = String.Empty;

					if(textPart != null)
					{
						Encoding encoding = textPart.BodyEncoding;
						if(!String.IsNullOrWhiteSpace(textPart.ContentType.CharSet))
							encoding = Encoding.GetEncoding(textPart.ContentType.CharSet);

						mailMessageString = encoding.GetString(textPart.Body);
					}

					List<MessagePart> attachmentsArray = mailMessage.FindAllAttachments();
					List<Attachment> newContentFiles = new List<Attachment>();
					List<Attachment> fullContentFiles = new List<Attachment>();

					for(Int32 index = 0; index < attachmentsArray.Count; index++)
					{
						if(attachments.Contains(attachmentsArray[index]))
							newContentFiles.Add(Dto.Convert(indexFileNames[index], attachmentsArray[index]));
						fullContentFiles.Add(Dto.Convert(indexFileNames[index], attachmentsArray[index]));
					}

					ParsedContent content = new ParsedContent
					{
						Message = Dto.Convert(mailMessage),
						Header = Dto.Convert(mailMessage.Headers),
						Attachments = fullContentFiles.ToArray(),
					};

					result.Status = this.Storage.SaveMessage(content)
						? ParsedState.Success
						: ParsedState.NotSupported;
					return result;

				} catch(Exception ex)
				{
					if(Utils.IsFatal(ex))
						throw;

					result.Status = ParsedState.Error;
					result.Exception = ex;
					return result;
				}
			}, mailMessage);
		}
	}
}