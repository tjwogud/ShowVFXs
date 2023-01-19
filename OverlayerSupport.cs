using Overlayer;
using Overlayer.Core;

namespace ShowVFXs
{
    internal static class OverlayerSupport
    {
        internal static void AddTags()
        {
            Overlayer.Main.AllTags.LoadTags(typeof(OverlayerSupport));
            RefreshTexts();
        }

        internal static void RemoveTags()
        {
            Overlayer.Main.AllTags.RemoveTag("FilterText");
            Overlayer.Main.AllTags.RemoveTag("FlashText");
            Overlayer.Main.AllTags.RemoveTag("BloomText");
            RefreshTexts();
        }

        internal static void RefreshTexts()
        {
            OText.Texts.ForEach(otext => otext.Apply());
        }

        [Tag("FilterText")]
        internal static string GetFilterText() => Main.filterText.Text;

        [Tag("FlashText")]
        internal static string GetFlashText() => Main.flashText.Text;

        [Tag("BloomText")]
        internal static string GetBloomText() => Main.bloomText.Text;
    }
}
