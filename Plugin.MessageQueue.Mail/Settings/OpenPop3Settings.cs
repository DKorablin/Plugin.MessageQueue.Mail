using System;
using System.ComponentModel;

namespace Plugin.MailMessageQueue.Settings
{
	public class OpenPop3Settings : IMessageSourceSettingsItem
	{
		Boolean IMessageSourceSettingsItem.IsValid
			=> !String.IsNullOrWhiteSpace(this.Server)
				&& !String.IsNullOrWhiteSpace(this.Login)
				&& !String.IsNullOrWhiteSpace(this.Password);

		String IMessageSourceSettingsItem.Key => this.EmailAddress;

		/// <summary>Сервер POP3 для просмотра сообщений</summary>
		[Category("POP3")]
		public String Server { get; set; }

		/// <summary>Порт POP3 сервера</summary>
		[Category("POP3")]
		[DefaultValue(110)]
		public Int32 Port { get; set; } = 110;

		/// <summary>Использовать SSL при подключении к серверу</summary>
		[Category("POP3")]
		[DisplayName("Use SSL")]
		[DefaultValue(true)]
		public Boolean UseSSL { get; set; } = true;

		/// <summary>Имя пользователя для доступа к ящику электронной почты</summary>
		[Category("POP3")]
		public String Login { get; set; }

		/// <summary>Пароль пользователя для доступа к ящику электронной почты</summary>
		[Category("POP3")]
		[PasswordPropertyText]
		public String Password { get; set; }

		[Category("POP3")]
		public String EmailAddress
		{
			get => String.IsNullOrWhiteSpace(this.Server) || String.IsNullOrWhiteSpace(this.Login)
					? null
					: this.Login + "@" + this.Server;
			set
			{
				if(String.IsNullOrWhiteSpace(value))
				{
					this.Login = null;
					this.Server = null;
				} else
				{
					String[] keyValue = value.Split(new Char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
					if(keyValue.Length == 2)
					{
						this.Login = keyValue[0];
						this.Server = keyValue[1];
					}
				}
			}
		}
	}
}