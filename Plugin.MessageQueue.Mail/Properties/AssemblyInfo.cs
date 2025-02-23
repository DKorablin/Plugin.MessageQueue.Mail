using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("53ee6768-4e0c-4391-a512-0a94b534f901")]
[assembly: System.CLSCompliant(false)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://github.com/DKorablin/Plugin.MessageQueue.Mail")]
#else

[assembly: AssemblyDescription("Message queue plugin based on mail messages")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2020")]
#endif