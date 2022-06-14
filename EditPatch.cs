using HarmonyLib;

namespace ShowCurrentFilters
{
    [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
    internal static class EditPatch
    {
        internal static void Postfix()
        {
            FilterPatch.filters.Clear();
        }
    }
}
