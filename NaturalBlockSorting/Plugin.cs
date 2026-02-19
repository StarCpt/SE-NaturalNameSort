using HarmonyLib;
using System.Reflection;
using VRage.Plugins;

namespace NaturalBlockSorting;

public class Plugin : IPlugin
{
    public void Init(object gameInstance)
    {
        new Harmony(GetType().FullName).PatchAll(Assembly.GetExecutingAssembly());
    }

    public void Update()
    {
    }

    public void Dispose()
    {
    }
}
