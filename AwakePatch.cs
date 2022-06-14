using HarmonyLib;

namespace ShowCurrentFilters
{
    [HarmonyPatch(typeof(scnEditor), "Awake")]
    public static class AwakePatch
    {
        public static void Postfix()
        {
            FilterText.AddOrDelete(true);
        }
    }
}
