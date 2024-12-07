using System;
using System.ComponentModel;

namespace Plugin.MailMessageQueue.Settings
{
	public class Imap4Settings : IMessageSourceSettingsItem
	{
		private String _searchQuery = "ALL";

		Boolean IMessageSourceSettingsItem.IsValid
			=> !String.IsNullOrWhiteSpace(this.Server)
				&& !String.IsNullOrWhiteSpace(this.Login)
				&& !String.IsNullOrWhiteSpace(this.Password);

		String IMessageSourceSettingsItem.Key => this.EmailAddress;

		/// <summary>Порт для подключения к IMAP4 серверу (По умолчанию: 993)</summary>
		[Category("IMAP4")]
		[DefaultValue(993)]
		public Int32 Port { get; set; } = 993;

		[Category("IMAP4")]
		public String Server { get; set; }

		[Category("IMAP4")]
		public String Login { get; set; }

		[Category("IMAP4")]
		[PasswordPropertyText]
		public String Password { get; set; }

		[Category("IMAP4")]
		[DisplayName("Default Inbox")]
		[DefaultValue("INBOX")]
		public String DefaultInbox { get; set; } = "INBOX";

		[Category("IMAP4")]
		[DisplayName("Search Query")]
		[DefaultValue("ALL")]
		public String SearchQuery
		{
			get => this._searchQuery;
			set
			{
				if(!String.IsNullOrWhiteSpace(value))
					this._searchQuery = value;
			}
		}

		[Category("IMAP4")]
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