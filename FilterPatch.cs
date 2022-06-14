using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace ShowCurrentFilters
{
    internal static class FilterPatch
    {
        internal static readonly Dictionary<Filter, float> filters = new Dictionary<Filter, float>();

        [HarmonyPatch(typeof(ffxSetFilterPlus), "SetFilter")]
        internal static class SetFilterPatch
        {

            public static void Postfix(Filter f, bool fEnable, float fIntensity)
            {
                if (!scrVfxPlus.instance.filterToComp.ContainsKey(f))
                    return;
                if (!fEnable || fIntensity == 0)
                    filters.Remove(f);
                else
                    filters[f] = fIntensity;
            }
        }

        [HarmonyPatch(typeof(ffxSetFilterPlus), "StartEffect")]
        public static class StartEffectPatch
        {
            public static void Postfix(Filter ___filter, bool ___disableOthers)
            {
                if (___disableOthers)
                {
                    float value = filters.TryGetValue(___filter, out float v) ? v : 0;
                    filters.Clear();
                    if (value != 0)
                        filters.Add(___filter, value);
                }
            }
        }
    }
}
