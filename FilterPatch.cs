using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace ShowVFXs
{
    internal static class FilterPatch
    {
        private static readonly List<Filter> filters = new List<Filter>();
        private static readonly List<float> intensities = new List<float>();

        internal static int Count()
        {
            return filters.Count;
        }

        internal static List<KeyValuePair<Filter, float>> GetPairs()
        {
            return filters.ToList().Select(filter => new KeyValuePair<Filter, float>(filter, Get(filter, out float v) ? v : v)).ToList();
        } 

        internal static bool Get(Filter filter, out float intensity)
        {
            intensity = 0;
            int i = filters.IndexOf(filter);
            if (i != -1)
            {
                intensity = intensities[i];
                return true;
            }
            return false;
        }

        private static void Set(Filter filter, float intensity)
        {
            if (Main.onOffTypes.Contains(filter) && intensity != 0)
                intensity = 1;
            int i = filters.IndexOf(filter);
            if (i != -1)
                intensities[i] = intensity;
            else
            {
                filters.Add(filter);
                intensities.Add(intensity);
            }
        }

        private static void Remove(Filter filter)
        {
            int i = filters.IndexOf(filter);
            if (i != -1)
            {
                filters.RemoveAt(i);
                intensities.RemoveAt(i);
            }
        }

        private static void Clear()
        {
            filters.Clear();
            intensities.Clear();
        }

        [HarmonyPatch(typeof(ffxSetFilterPlus), "SetFilter")]
        internal static class SetFilterPatch
        {

            public static void Postfix(Filter f, bool fEnable, float fIntensity)
            {
                if (!scrVfxPlus.instance.filterToComp.ContainsKey(f))
                    return;
                if (!fEnable)
                    Remove(f);
                else
                    Set(f, fIntensity);
            }
        }

        [HarmonyPatch(typeof(ffxSetFilterPlus), "StartEffect")]
        public static class StartEffectPatch
        {
            public static void Postfix(Filter ___filter, bool ___disableOthers)
            {
                if (___disableOthers)
                    if (Get(___filter, out float v))
                    {
                        Clear();
                        Set(___filter, v);
                    } else
                        Clear();
            }
        }

        [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
        public static class AwakeRewindPatch
        {
            public static void Postfix()
            {
                Clear();
            }
        }

        [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
        internal static class SwitchToEditPatch
        {
            internal static void Postfix()
            {
                Clear();
            }
        }
    }
}
