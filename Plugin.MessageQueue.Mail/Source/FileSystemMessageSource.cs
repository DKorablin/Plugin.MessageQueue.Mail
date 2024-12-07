using System;
using System.Collections.Generic;
using System.IO;
using Plugin.MailMessageQueue.Settings;

namespace Plugin.MailMessageQueue.Data
{
	internal class FileSystemMessageSource : MailSourceBase<FileSystemMessageDto>
	{
		private readonly FileSystemSettings _settings;

		public FileSystemMessageSource(Plugin plugin, FileSystemSettings settings)
			: base(plugin, settings)
		{
			this._settings = settings;
		}

		private IEnumerable<String> GetDirectories()
		{
			yield return this._settings.FilePath;

			/*String currentDirectory = Environment.CurrentDirectory;
			yield return currentDirectory;

			String assemblyPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
			if(Directory.Exists(assemblyPath) && !currentDirectory.Equals(assemblyPath, StringComparison.OrdinalIgnoreCase))
				yield return assemblyPath;*/
		}

		public override IEnumerator<FileSystemMessageDto> GetEnumerator()
		{
			foreach(String directory in this.GetDirectories())
				foreach(String filePath in Directory.EnumerateFiles(directory, "*.msg"))
					yield return new FileSystemMessageDto(filePath);
		}

		public override void DisposeMessage(FileSystemMessageDto message)
		{
			File.Delete(message.FilePath);
		}
	}
}