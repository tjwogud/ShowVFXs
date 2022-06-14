using HarmonyLib;

namespace ShowCurrentFilters
{
    [HarmonyPatch(typeof(ADOStartup), "Startup")]
    internal static class StartupPatch
    {
        internal static void Postfix()
        {
            FilterText.LoadOnOffTypes();
        }
    }
}
