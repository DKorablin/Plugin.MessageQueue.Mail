using System;
using System.Diagnostics;
using SAL.Flatbed;

namespace Plugin.MailMessageQueue.Plugins
{
	internal abstract class TimerHandlerBase : IDisposable
	{
		/// <summary>Плагин таймера</summary>
		private static class TimersPlugin
		{
			/// <summary>ID плагина с таймерами</summary>
			public const String Name = "69c79417-c168-434a-a597-4e224237a527";

			/// <summary>Публичные методы плагина</summary>
			public static class Methods
			{
				/// <summary>Зарегистрировать таймер</summary>
				public const String RegisterTimer = "RegisterTimer";

				/// <summary>Удалить регистрацию таймера</summary>
				public const String UnregisterTimer = "UnregisterTimer";
			}
		}

		private TraceSource _trace;
		private readonly IHost _host;
		private readonly String _handlerName;
		private readonly String _timerName;

		protected TraceSource Trace { get => this._trace ?? (this._trace = TimerHandlerBase.CreateTraceSource(this._handlerName)); }

		public TimerHandlerBase(IHost host, String timerName)
		{
			Type thisType = this.GetType();
			this._host = host;
			this._handlerName = thisType.Assembly.GetName().Name + "." + thisType.Name;
			this._timerName = timerName;

			this._host.Plugins.PluginsLoaded += new EventHandler(Host_PluginsLoaded);
		}

		/// <summary>Регистрация таймера через плагин</summary>
		private void RegisterTimer()
		{
			IPluginDescription plugin = this._host.Plugins[TimersPlugin.Name];
			if(plugin == null)
				this.Trace.TraceEvent(TraceEventType.Error, 10, "Required plugin ID={0} not found", TimersPlugin.Name);
			else
			{
				plugin.Type.GetMember<IPluginMethodInfo>(TimersPlugin.Methods.RegisterTimer)
					.Invoke(this._handlerName, this._timerName, (EventHandler<EventArgs>)this.OnInvokeTimer, null);
			}
		}

		/// <summary>Удалить регистрацию таймера</summary>
		private void UnregisterTimer()
		{
			IPluginDescription plugin = this._host.Plugins[TimersPlugin.Name];
			plugin?.Type
				.GetMember<IPluginMethodInfo>(TimersPlugin.Methods.UnregisterTimer)
				.Invoke(this._handlerName);
		}

		protected abstract void OnInvokeTimer(Object state, EventArgs args);

		private static TraceSource CreateTraceSource(String name)
		{
			TraceSource result = new TraceSource(typeof(TimerHandlerBase).Assembly.GetName().Name + "." + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}

		private void Host_PluginsLoaded(Object sender, EventArgs e)
		{
			this._host.Plugins.PluginsLoaded -= new EventHandler(this.Host_PluginsLoaded);

			this.RegisterTimer();
		}

		public virtual void Dispose()
			=> this.UnregisterTimer();
	}
}