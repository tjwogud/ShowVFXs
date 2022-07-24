using Overlayer;
using Overlayer.Tags;

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

        [Tag("FilterText", "Shows filter")]
        internal static string GetFilterText() => Main.filterText.Text;

        [Tag("FlashText", "Shows flash")]
        internal static string GetFlashText() => Main.flashText.Text;

        [Tag("BloomText", "Shows bloom")]
        internal static string GetBloomText() => Main.bloomText.Text;
    }
}
