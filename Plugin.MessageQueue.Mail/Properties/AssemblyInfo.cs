using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("53ee6768-4e0c-4391-a512-0a94b534f901")]
[assembly: System.CLSCompliant(false)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://github.com/DKorablin/Plugin.MessageQueue.Mail")]
#else

[assembly: AssemblyTitle("Plugin.MessageQueue.Mail")]
[assembly: AssemblyDescription("Message queue plugin based on mail messages")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.MessageQueue.Mail")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2020")]
#endif