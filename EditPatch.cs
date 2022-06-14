using HarmonyLib;

namespace ShowCurrentFilters
{
    [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
    public static class EditPatch
    {
        public static void Postfix()
        {
            FilterPatch.filters.Clear();
        }
    }
}
