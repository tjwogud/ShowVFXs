using Overlayer;
using Overlayer.Core;
using Overlayer.Core.Tags;
using System;

namespace ShowVFXs
{
    internal static class OverlayerSupport
    {
        internal static void AddTags()
        {
            TagManager.Load(typeof(OverlayerSupport));
            TextManager.Refresh();
        }

        internal static void RemoveTags()
        {
            TagManager.Unload(typeof(OverlayerSupport));
            TextManager.Refresh();
        }

        [Tag("FilterText")]
        internal static string GetFilterText() => Main.filterText.Text;

        [Tag("FilterIntensity")]
        internal static float GetFilterIntensity(string filter) => Enum.TryParse(filter, true, out Filter value) ? (FilterPatch.Get(value, out float intensity) ? intensity : 0) : -1;

        [Tag("FlashText")]
        internal static string GetFlashText() => Main.flashText.Text;

        [Tag("BloomText")]
        internal static string GetBloomText() => Main.bloomText.Text;
    }
}
