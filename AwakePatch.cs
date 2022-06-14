using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace ShowCurrentFilters
{
    [HarmonyPatch(typeof(scrVfxPlus), "Awake")]
    public static class AwakePatch
    {
        public static void Postfix()
        {
            scrVfxPlus.instance.filterToComp.ToList().ForEach(pair =>
            {
                if (!pair.Value.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Any(f => f.Name != "SCMaterial" && f.Name != "SCShader" && f.Name != "TimeX"))
                    FilterText.onOffTypes.Add(pair.Key);
            });
        }
    }
}
