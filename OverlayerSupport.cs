using Overlayer;
using TagLib;
using TagLib.Tags;

namespace ShowVFXs
{
    internal static class OverlayerSupport
    {
        internal static void AddTags()
        {
            TagManager.AllTags.LoadTags(typeof(OverlayerSupport));
            RefreshTexts();
        }

        internal static void RemoveTags()
        {
            TagManager.AllTags.RemoveTag("FilterText");
            TagManager.AllTags.RemoveTag("FlashText");
            TagManager.AllTags.RemoveTag("BloomText");
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
