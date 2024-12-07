using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Plugin.MailMessageQueue
{
	internal static class Utils
	{
		/// <summary>Скомпиленная регулярка для удаления тегов из текста</summary>
		private readonly static Regex RemoveTagsRegex = new Regex(@"</?([^>]+)>", RegexOptions.Compiled);

		/// <summary>Удалить все теги из HTML содержимого</summary>
		/// <param name="html">Текст, из которого удалить все HTML теги</param>
		/// <param name="safeTags">Массив тегов, которые не нужно убирать</param>
		/// <returns>Результат без HTML содержимого</returns>
		public static String RemoveTags(String html, String[] safeTags = null)
		{
			if(String.IsNullOrEmpty(html))
				return html;
			else
			{
				if(safeTags != null)
				{
					Int32 cutLength = 0;
					MatchCollection matches = RemoveTagsRegex.Matches(html);

					foreach(Match match in matches)
					{
						if(safeTags.Any(p => match.Groups[1].Value.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)))
							continue;
						html = html.Remove(match.Index - cutLength, match.Length);
						cutLength += match.Length;
					}
					return html;
				} else
					return RemoveTagsRegex.Replace(html, String.Empty);
			}
		}

		/// <summary>Фатальная ошибка, которую обрабатывать не надо</summary>
		/// <param name="exception">Ошибка для проверки</param>
		/// <returns>Ошибка фатальная и обрабатывать нет смысла</returns>
		public static Boolean IsFatal(Exception exception)
		{
			while(exception != null)
			{
				if((exception is OutOfMemoryException && !(exception is InsufficientMemoryException))//Нет смысла занимать больше памяти
					|| exception is ThreadAbortException//Ошибка происходит при редиректе с одной страницы на другую
					|| exception is AccessViolationException
					|| exception is SEHException)
					return true;
				if(!(exception is TypeInitializationException) && !(exception is TargetInvocationException))
					break;
				exception = exception.InnerException;
			}
			return false;
		}
	}
}