using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SAL.Flatbed;

namespace Plugin.MailMessageQueue.Plugins
{
	internal class MailPluginResolver<T>
	{
		private readonly T[] _instances;

		/// <summary>Интерфейс плагина который является первоисточником всех хендлеров</summary>
		public IPluginDescription Plugin { get; private set; }

		/// <summary>Массив фасадов всех чатов плагина</summary>
		public IEnumerable<T> Instances
		{
			get
			{
				foreach(T instance in this._instances)
					yield return instance;
			}
		}

		/// <summary>Кол-во загруженных фасадов</summary>
		public Int32 Count { get { return this._instances.Length; } }

		/// <summary>Создание экземпляра класса с поиском всех инстансов и обёртка их в фасад</summary>
		/// <param name="plugin">Плагин в котором ищем все инстансы</param>
		public MailPluginResolver(IPluginDescription plugin)
		{
			this.Plugin = plugin;
			Assembly botAssembly = this.Plugin.Instance.GetType().Assembly;

			List<T> instances = new List<T>();
			if(botAssembly.GetReferencedAssemblies().Any(p => p.FullName == typeof(T).Assembly.GetName().FullName))
			{//TODO: Надо проверить что при BindingRedirect сборки цепляются верно (Т.е. если в конфиге редирект с 1.0 на 2.0, то и в коде reference будет на 2.0)
				foreach(Type t in botAssembly.GetTypes())
				{
					Type[] interfaces = t.GetInterfaces();
					if(interfaces.Any(i => i == typeof(T)))
						instances.Add(CreateInstance<T>(t));
				}
			}
			this._instances = instances.ToArray();
		}

		private N CreateInstance<N>(Type type)
		{
			ConstructorInfo ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => p.GetParameters().Length == 0);
			return (N)ctor.Invoke(new Object[] { });
		}
	}
}