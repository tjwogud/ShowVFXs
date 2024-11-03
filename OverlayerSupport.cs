using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShowVFXs
{
    public static class OverlayerSupport
    {
        private readonly static List<string> addedTags = new List<string>();
        private static IDictionary tags;
        private static ConstructorInfo tagcctor;
        private static ConstructorInfo attribcctor;

        internal static void AddTags()
        {
            Init();
            typeof(OverlayerTags).GetMethods().ToList().ForEach(info => AddTag(info));
            Reflections.GetType("Overlayer.Core.TextManager").Method("Refresh");
            Main.Logger.Log("added tags!");
        }

        internal static void RemoveTags()
        {
            addedTags.ForEach(tag => tags.Remove(tag));
            addedTags.Clear();
            Reflections.GetType("Overlayer.Core.TextManager").Method("Refresh");
            Main.Logger.Log("removed tags!");
        }

        internal static void Init()
        {
            tags = Reflections.GetType("Overlayer.Tags.TagManager").Get<IDictionary>("tags");
            tagcctor = Reflections.GetType("Overlayer.Tags.OverlayerTag").GetConstructors().First(c => c.GetParameters().Length > 0 && c.GetParameters()[0].ParameterType == typeof(MethodInfo));
            attribcctor = Reflections.GetType("Overlayer.Tags.Attributes.TagAttribute").GetConstructors().First(c => c.GetParameters().Length > 0);
        }

        internal static void AddTag(MethodInfo info, bool notPlaying = false)
        {
            string name = info.Name;
            object attrib = attribcctor.Invoke(new object[] { name });
            attrib.Set("NotPlaying", notPlaying);
            object tag = tagcctor.Invoke(new object[] { info, attrib, null });
            tags[name] = tag;
            addedTags.Add(name);
        }
    }

    public static class OverlayerTags
    {
        public static string FilterText() => Main.filterText.Text;

        public static float FilterIntensity(string filter) => Enum.TryParse(filter, true, out Filter value) ? (FilterPatch.Get(value, out float intensity) ? intensity : 0) : -1;

        public static string FlashText() => Main.flashText.Text;

        public static string AdvFilterText(string name = null) => name.IsNullOrEmpty() ? Main.advFilterText.Text : (FilterPatch.advancedFilters.Contains(name) ? Main.Settings.advFilterOn : Main.Settings.advFilterOff);

        public static string BloomText() => Main.bloomText.Text;
    }
}
