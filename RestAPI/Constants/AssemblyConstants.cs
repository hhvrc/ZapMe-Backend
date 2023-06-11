using System.Reflection;

namespace ZapMe.Constants;

public static class AssemblyConstants
{
    public static readonly string XmlPath = Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly().GetName().Name + ".xml");
}
