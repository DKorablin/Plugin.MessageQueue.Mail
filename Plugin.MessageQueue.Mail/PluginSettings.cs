using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Plugin.MailMessageQueue.Settings;

namespace Plugin.MailMessageQueue
{
	public class PluginSettings : INotifyPropertyChanged
	{
		private readonly Plugin _plugin;
		private MessageSourceSettingsCollection _data;
		private Boolean _POP3UseSSL = true;
		private Int32 _mailParserThreadCount = 5;

		/// <summary>Количество потоков обработки писем</summary>
		[Category("General")]
		[Description("MailParser threads count")]
		[DefaultValue(5)]
		public Int32 MailParserThreadCount
		{
			get
			{
#if DEBUG
					return 1;
#else
					return this._mailParserThreadCount;
#endif
			}
			set { this.SetField(ref this._mailParserThreadCount, value > 0 ? value : 1, nameof(MailParserThreadCount)); }
		}

		[Category("General")]
		[Description("Use SSL connection")]
		[DefaultValue(true)]
		public Boolean POP3UseSSL
		{
			get => this._POP3UseSSL;
			set => this.SetField(ref this._POP3UseSSL, value, nameof(POP3UseSSL));
		}

		internal MessageSourceSettingsCollection Data
		{
			get { return this._data ?? (this._data = this.GetSettings()); }
			private set { this._data = value; }
		}

		internal PluginSettings(Plugin plugin)
			=> this._plugin = plugin;

		private MessageSourceSettingsCollection GetSettings()
		{
			using(Stream stream = this._plugin.Host.Plugins.Settings(this._plugin).LoadAssemblyBlob("DataJson"))
				if(stream != null)
					using(StreamReader reader = new StreamReader(stream))
						return new MessageSourceSettingsCollection(this._plugin, reader.ReadToEnd());

			return new MessageSourceSettingsCollection(this._plugin, null);
		}

		public void SaveSettings()
			=> this.SaveSettings(this.Data);

		private void SaveSettings(MessageSourceSettingsCollection collection)
		{
			String json = collection.ToJson();
			if(json == null)
				this._plugin.Host.Plugins.Settings(this._plugin).SaveAssemblyBlob("DataJson", null);
			else
			{
				Byte[] payload = Encoding.UTF8.GetBytes(json);
				using(MemoryStream stream = new MemoryStream(payload))
					this._plugin.Host.Plugins.Settings(this._plugin).SaveAssemblyBlob("DataJson", stream);
			}
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private Boolean SetField<T>(ref T field, T value, String propertyName)
		{
			if(EqualityComparer<T>.Default.Equals(field, value))
				return false;

			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
		#endregion INotifyPropertyChanged
	}
}