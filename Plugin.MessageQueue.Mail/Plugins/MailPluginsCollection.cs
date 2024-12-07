using System;
using System.Collections.Generic;
using System.Diagnostics;
using SAL.Flatbed;
using SAL.Interface.MessageQueue.Mail;

namespace Plugin.MailMessageQueue.Plugins
{
	internal class MailPluginsCollection
	{
		private readonly Plugin _plugin;
		private readonly MailPluginResolver<IMessageSaver>[] _mailPlugins;

		public MailPluginsCollection(Plugin plugin)
		{
			this._plugin = plugin;

			this._mailPlugins = this.ResolvePlugins();
		}

		/// <summary>Сохранить содержимое</summary>
		/// <param name="content">Содержимое</param>
		/// <returns>Сообщение успешно обработано</returns>
		public Boolean HandleMessage(MailMessageDto content)
		{
			foreach(MailPluginResolver<IMessageSaver> saver in this._mailPlugins)
				foreach(var instance in saver.Instances)
					if(instance.HandleMessage(content))
						return true;
			return false;
		}

		private MailPluginResolver<IMessageSaver>[] ResolvePlugins()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			List<MailPluginResolver<IMessageSaver>> plugins = new List<MailPluginResolver<IMessageSaver>>();// (this._host.Plugins.FindPluginType<IBotMarker>());
			foreach(IPluginDescription plugin in this._plugin.Host.Plugins)
			{
				MailPluginResolver<IMessageSaver> resolver = new MailPluginResolver<IMessageSaver>(plugin);
				if(resolver.Count > 0)
					plugins.Add(resolver);
			}
			sw.Stop();
			List<String> message = new List<String>()
			{
				$"Loaded {plugins.Count} chat plugins at: {sw.Elapsed}",
			};
			foreach(MailPluginResolver<IMessageSaver> resolver in plugins)
			{
				message.Add($"\tPlugin: {resolver.Plugin.Name}");
				foreach(var instance in resolver.Instances)
					message.Add($"\t\tInstance: {instance.GetType().Name}");
			}
			this._plugin.Trace.TraceEvent(TraceEventType.Start, 2, String.Join(Environment.NewLine, message.ToArray()));

			return plugins.ToArray();
		}
	}
}