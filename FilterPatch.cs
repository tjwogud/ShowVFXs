using HarmonyLib;
using System.Collections.Generic;

namespace ShowCurrentFilters
{
    [HarmonyPatch(typeof(ffxSetFilterPlus), "SetFilter")]
    public static class FilterPatch
    {
        public static readonly Dictionary<Filter, float> filters = new Dictionary<Filter, float>();

        public static void Postfix(Filter f, bool fEnable, float fIntensity)
        {
            Main.Logger.Log($"{f}, {fEnable}, {fIntensity}");
            if (!scrVfxPlus.instance.filterToComp.ContainsKey(f))
                return;
            if (!fEnable || fIntensity == 0)
                filters.Remove(f);
            else
                filters[f] = fIntensity;
        }
    }
}
