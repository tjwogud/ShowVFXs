using HarmonyLib;
using Localizations;
using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace ShowCurrentFilters
{
    internal class Main
    {
        internal static UnityModManager.ModEntry ModEntry;
        internal static UnityModManager.ModEntry.ModLogger Logger;
        internal static Localization Localization;
        internal static Settings Settings;

        internal static void Setup(UnityModManager.ModEntry modEntry)
        {
            ModEntry = modEntry;
            Logger = modEntry.Logger;
            Harmony harmony = new Harmony(modEntry.Info.Id);
            modEntry.OnToggle = (_, value) =>
            {
                FilterText.AddOrDelete(true);
                if (value)
                    harmony.PatchAll(Assembly.GetExecutingAssembly());
                else
                    harmony.UnpatchAll();
                return true;
            };
            modEntry.OnGUI = OnGUI;
            Logger.Log("Loading Settings...");
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            Logger.Log("Load Completed!");
            Localization = Localization.Load(modEntry, "1QcrRL6LAs8WxJj_hFsEJa3CLM5g3e8Ya0KQlRKXwdlU", 967104647);
        }

        internal static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (!Localization.Loaded)
            {
                GUILayout.Label("Localization is not loaded yet!");
                return;
            }
            if (Localization.Failed)
            {
                GUILayout.Label("Couldn't load Localization!");
                return;
            }
            GUILayout.Label(Localization["scf.gui.position"]);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization["scf.settings.x"] + ":");
            Settings.x = (float)(Math.Round(GUILayout.HorizontalSlider(Settings.x, -0.1f, 1.1f, GUILayout.Width(300)) * 100) / 100);
            GUILayout.Label(Settings.x.ToString());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization["scf.settings.y"] + ":");
            Settings.y = (float)(Math.Round(GUILayout.HorizontalSlider(Settings.y, -0.1f, 1.1f, GUILayout.Width(300)) * 100) / 100);
            GUILayout.Label(Settings.y.ToString());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.Space(20);
            GUILayout.Label(Localization["scf.gui.text"]);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization["scf.settings.textFormat"]);
            Settings.textFormat = GUILayout.TextField(Settings.textFormat);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization["scf.settings.textSeparator"]);
            Settings.textSeparator = GUILayout.TextField(Settings.textSeparator);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization["scf.settings.textEmpty"]);
            Settings.textEmpty = GUILayout.TextField(Settings.textEmpty);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.Space(20);
            GUILayout.Label(Localization["scf.gui.textEffect"]);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization["scf.settings.textSize"]);
            Settings.textSize = (int)Math.Round(GUILayout.HorizontalSlider(Settings.textSize, 1, 50, GUILayout.Width(200)));
            GUILayout.Label(Settings.textSize.ToString());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization["scf.settings.boldText"]);
            Settings.boldText = GUILayout.Toggle(Settings.boldText, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization["scf.settings.shadowText"]);
            Settings.shadowText = GUILayout.Toggle(Settings.shadowText, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization["scf.settings.textAlign"]);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Settings.textAlign == TextAnchor.UpperLeft ? $"<b>{Localization["textAnchor.upperLeft"]}</b>" : Localization["textAnchor.upperLeft"]))
                Settings.textAlign = TextAnchor.UpperLeft;
            if (GUILayout.Button(Settings.textAlign == TextAnchor.UpperCenter ? $"<b>{Localization["textAnchor.upperCenter"]}</b>" : Localization["textAnchor.upperCenter"]))
                Settings.textAlign = TextAnchor.UpperCenter;
            if (GUILayout.Button(Settings.textAlign == TextAnchor.UpperRight ? $"<b>{Localization["textAnchor.upperRight"]}</b>" : Localization["textAnchor.upperRight"]))
                Settings.textAlign = TextAnchor.UpperRight;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Settings.textAlign == TextAnchor.MiddleLeft ? $"<b>{Localization["textAnchor.middleLeft"]}</b>" : Localization["textAnchor.middleLeft"]))
                Settings.textAlign = TextAnchor.MiddleLeft;
            if (GUILayout.Button(Settings.textAlign == TextAnchor.MiddleCenter ? $"<b>{Localization["textAnchor.middleCenter"]}</b>" : Localization["textAnchor.middleCenter"]))
                Settings.textAlign = TextAnchor.MiddleCenter;
            if (GUILayout.Button(Settings.textAlign == TextAnchor.MiddleRight ? $"<b>{Localization["textAnchor.middleRight"]}</b>" : Localization["textAnchor.middleRight"]))
                Settings.textAlign = TextAnchor.MiddleRight;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Settings.textAlign == TextAnchor.LowerLeft ? $"<b>{Localization["textAnchor.lowerLeft"]}</b>" : Localization["textAnchor.lowerLeft"]))
                Settings.textAlign = TextAnchor.LowerLeft;
            if (GUILayout.Button(Settings.textAlign == TextAnchor.LowerCenter ? $"<b>{Localization["textAnchor.lowerCenter"]}</b>" : Localization["textAnchor.lowerCenter"]))
                Settings.textAlign = TextAnchor.LowerCenter;
            if (GUILayout.Button(Settings.textAlign == TextAnchor.LowerRight ? $"<b>{Localization["textAnchor.lowerRight"]}</b>" : Localization["textAnchor.lowerRight"]))
                Settings.textAlign = TextAnchor.LowerRight;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
