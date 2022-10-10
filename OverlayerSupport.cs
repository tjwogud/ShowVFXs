using Overlayer;
using Overlayer.Core;

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

        [Tag("FilterText")]
        internal static string GetFilterText() => Main.filterText.Text;

        [Tag("FlashText")]
        internal static string GetFlashText() => Main.flashText.Text;

        [Tag("BloomText")]
        internal static string GetBloomText() => Main.bloomText.Text;
    }
}
