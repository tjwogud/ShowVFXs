using HarmonyLib;
using Overlayer;
using Overlayer.Core;
using Overlayer.Core.Tags;
using System;

namespace ShowVFXs
{
    public static class OverlayerSupport
    {
        internal static void AddTags()
        {
            Reflections.GetType("Overlayer.Core.TagManager").Method("Load", new object[] { Reflections.GetType("ShowVFXs.OverlayerTags") }, new Type[] { typeof(Type) });
            Reflections.GetType("Overlayer.Core.TextManager").Method("Refresh");
        }

        internal static void RemoveTags()
        {
            Reflections.GetType("Overlayer.Core.TagManager").Method("Unload", new object[] { Reflections.GetType("ShowVFXs.OverlayerTags") }, new Type[] { typeof(Type) });
            Reflections.GetType("Overlayer.Core.TextManager").Method("Refresh");
        }
    }

    public static class OverlayerTags
    {

        [Tag("FilterText")]
        public static string GetFilterText() => Main.filterText.Text;

        [Tag("FilterIntensity")]
        public static float GetFilterIntensity(string filter) => Enum.TryParse(filter, true, out Filter value) ? (FilterPatch.Get(value, out float intensity) ? intensity : 0) : -1;

        [Tag("FlashText")]
        public static string GetFlashText() => Main.flashText.Text;

        [Tag("BloomText")]
        public static string GetBloomText() => Main.bloomText.Text;
    }
}
