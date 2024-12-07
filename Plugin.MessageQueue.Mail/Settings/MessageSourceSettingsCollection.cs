using System;
using System.Collections.Generic;

namespace Plugin.MailMessageQueue.Settings
{
	public class MessageSourceSettingsCollection : IEnumerable<IMessageSourceSettingsItem>
	{
		private readonly Plugin _plugin;
		/// <summary>Словарь хранилища настроек таймеров</summary>
		private readonly List<IMessageSourceSettingsItem> _messageSource = new List<IMessageSourceSettingsItem>();

		/// <summary>Получить маппинг по индексу</summary>
		/// <param name="index">Индекс по которому получить маппинг</param>
		/// <returns>Маппинг по индексу или null</returns>
		public IMessageSourceSettingsItem this[Int32 index]
		{
			get => index < 0 || index >= this._messageSource.Count
				? null
				: this._messageSource[index];
			set => this._messageSource[index] = value;
		}

		/// <summary>Создание нового маппинга</summary>
		/// <returns>Созданный новый маппинг</returns>
		public IMessageSourceSettingsItem NewMapping(MessageSourceType type)
		{
			IMessageSourceSettingsItem result;
			switch(type)
			{
			case MessageSourceType.FileSystem:
				throw new NotImplementedException();
			case MessageSourceType.Imap4:
				result = new Imap4Settings();
				break;
			case MessageSourceType.OpenPop3:
				result = new OpenPop3Settings();
				break;
			case MessageSourceType.Pop3:
				result = new Pop3Settings();
				break;
			default:
				throw new NotImplementedException();
			}

			return result;
		}

		public MessageSourceType GetType(IMessageSourceSettingsItem item)
		{
			if(item is OpenPop3Settings)
				return MessageSourceType.OpenPop3;
			else if(item is Pop3Settings)
				return MessageSourceType.Pop3;
			else if(item is Imap4Settings)
				return MessageSourceType.Imap4;
			else if(item is FileSystemSettings)
				return MessageSourceType.FileSystem;
			else
				throw new NotImplementedException();
		}

		/// <summary>Создание экземпляра коллекции маппингов таймеров с компилятором</summary>
		/// <param name="plugin">Плагин</param>
		/// <param name="json">JSON из которого создать коллекцию</param>
		public MessageSourceSettingsCollection(Plugin plugin, String json)
		{
			this._plugin = plugin;

			if(json != null)
				foreach(MessageSourceJsonItem jsonItem in Serializers.JavaScriptDeserialize<MessageSourceJsonItem[]>(json))
				{
					IMessageSourceSettingsItem item;
					switch(jsonItem.Type)
					{
					case MessageSourceType.FileSystem:
						item = Serializers.JavaScriptDeserialize<FileSystemSettings>(jsonItem.Json);
						break;
					case MessageSourceType.Imap4:
						item = Serializers.JavaScriptDeserialize<Imap4Settings>(jsonItem.Json);
						break;
					case MessageSourceType.OpenPop3:
						item = Serializers.JavaScriptDeserialize<OpenPop3Settings>(jsonItem.Json);
						break;
					case MessageSourceType.Pop3:
						item = Serializers.JavaScriptDeserialize<Pop3Settings>(jsonItem.Json);
						break;
					default:
						throw new NotImplementedException();
					}
					this._messageSource.Add(item);
				}
		}

		/// <summary>Добавить или изменить элемент в списке</summary>
		/// <param name="item">Элемент для добавления</param>
		public void AddOrUpdate(IMessageSourceSettingsItem item)
		{
			Int32 index = this._messageSource.IndexOf(item);
			if(index > -1)
				this._messageSource[index] = item;
			else
				this._messageSource.Add(item);
		}

		/// <summary>Удалить маппинг таймера к методу, с функцией удаления метода из компилятора</summary>
		/// <param name="item">Элемент маппинга таймера с компилятором</param>
		/// <returns>Маппинг удалён успешно или маппинг не найден</returns>
		public Boolean Remove(IMessageSourceSettingsItem item)
		{
			Int32 index = this._messageSource.IndexOf(item);
			if(index > -1)
			{
				this._plugin.Source.Remove(item);

				this._messageSource.RemoveAt(index);
				return true;
			}
			return false;
		}

		/// <summary>Сконвертировать маппинги в JSON</summary>
		/// <returns>Строковое представление объекта</returns>
		public String ToJson()
		{
			if(this._messageSource != null && this._messageSource.Count > 0)
			{
				MessageSourceJsonItem[] data = new MessageSourceJsonItem[this._messageSource.Count];
				Int32 loop = 0;
				foreach(IMessageSourceSettingsItem item in this)
				{
					data[loop++] = new MessageSourceJsonItem() { Type = this.GetType(item), Json = Serializers.JavaScriptSerialize(item), };
				}
				return Serializers.JavaScriptSerialize(data);
			} else return null;
		}

		public IEnumerator<IMessageSourceSettingsItem> GetEnumerator()
		{
			foreach(IMessageSourceSettingsItem item in this._messageSource)
				yield return item;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			=> this.GetEnumerator();
	}
}