using System;
using System.ComponentModel;
using System.IO;

namespace Plugin.MailMessageQueue.Settings
{
	public class FileSystemSettings : IMessageSourceSettingsItem
	{
		Boolean IMessageSourceSettingsItem.IsValid => Directory.Exists(this.FilePath);

		String IMessageSourceSettingsItem.Key => this.FilePath;

		[DisplayName("File Path")]
		public String FilePath { get; set; }
	}
}