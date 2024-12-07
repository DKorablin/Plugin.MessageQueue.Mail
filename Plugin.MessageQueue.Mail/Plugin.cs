using System;
using System.Diagnostics;
using Plugin.MailMessageQueue.Data;
using Plugin.MailMessageQueue.Plugins;
using Plugin.MailMessageQueue.UI;
using SAL.Flatbed;

namespace Plugin.MailMessageQueue
{
	public class Plugin : IPlugin, IPluginSettings<PluginSettings>
	{
		#region Fields
		private TraceSource _trace;
		private PluginSettings _settings;
		private TimerHandler _timer;
		private MailPluginsCollection _target;
		private MailSourceCollection _source;
		#endregion Fields

		#region Properties
		internal TraceSource Trace
			=> this._trace ?? (this._trace = Plugin.CreateTraceSource<Plugin>());

		internal IHost Host { get; }

		/// <summary>Настройки для взаимодействия из хоста</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Настройки для взаимодействия из плагина</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings(this);
					this.Host.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
				}
				return this._settings;
			}
		}

		internal MailPluginsCollection Target
			=> this._target ?? (this._target = new MailPluginsCollection(this));

		internal MailSourceCollection Source
			=> this._source ?? (this._source = new MailSourceCollection(this));

		#endregion Properties

		public Plugin(IHost host)
			=> this.Host = host ?? throw new ArgumentNullException(nameof(host));

		/// <summary>Получить расширенные настройки с кастомным UI</summary>
		/// <returns></returns>
		public Object GetPluginOptionsControl()
			=> new ConfigCtrl(this);

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			this._timer = new TimerHandler(this);
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			this._timer.Dispose();
			return true;
		}

		internal static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}
	}
}