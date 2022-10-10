using ADOFAI;
using HarmonyLib;
using Localizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace ShowVFXs
{
    internal class Main
    {
        internal static UnityModManager.ModEntry ModEntry;
        internal static UnityModManager.ModEntry.ModLogger Logger;
        internal static Localization Localization;
        internal static Settings Settings;

        internal static List<Filter> onOffTypes = new List<Filter>();
        private static bool inited = false;

        internal static VfxText filterText;
        internal static VfxText flashText;
        internal static VfxText bloomText;
        internal static VfxText entireText;

        internal static void Setup(UnityModManager.ModEntry modEntry)
        {
            ModEntry = modEntry;
            Logger = modEntry.Logger;
            Harmony harmony = new Harmony(modEntry.Info.Id);
            modEntry.OnToggle = (_, value) =>
            {
                if (value)
                {
                    filterText = new GameObject("FilterText").AddComponent<VfxText>();
                    filterText.Init(
                        () =>
                        {
                            if (FilterPatch.Count() == 0)
                                return Settings.filterShowTextEmpty ? Settings.filterTextEmpty : "";
                            else
                            {
                                var query = FilterPatch.GetPairs().Select(pair =>
                                        onOffTypes != null && onOffTypes.Contains(pair.Key)
                                            ? Settings.filterOnOffTextFormat
                                                .Replace("{name}", RDString.GetEnumValue(pair.Key))
                                            : Settings.filterTextFormat
                                                .Replace("{name}", RDString.GetEnumValue(pair.Key))
                                                .Replace("{value}", (Math.Round((decimal)pair.Value * (decimal)Math.Pow(10, 2 + Settings.filterIntensityDecimal)) / (decimal)Math.Pow(10, Settings.filterIntensityDecimal)).ToString()));
                                if (Settings.filterTextOrder == FilterTextOrder.Ascending || Settings.filterTextOrder == FilterTextOrder.Descending)
                                    query = query.OrderBy(t => t);
                                if (Settings.filterTextOrder == FilterTextOrder.Descending || Settings.filterTextOrder == FilterTextOrder.ReverseAdded)
                                    query = query.Reverse();
                                return string.Join(Settings.filterTextSeparator, query);
                            }
                        },
                        () => Settings.filterX,
                        () => Settings.filterY,
                        () => Settings.filterTextSize,
                        () => Settings.filterBoldText,
                        () => Settings.filterShadowText,
                        () => Settings.filterTextAlign
                    );
                    filterText.enabled = Settings.filterEnable;
                    flashText = new GameObject("FlashText").AddComponent<VfxText>();
                    flashText.Init(
                        () =>
                        {
                            if (scrCamera.instance?.flashPlusRendererFg == null || scrCamera.instance?.flashPlusRendererBg == null)
                                return "";
                            Color fg = scrCamera.instance.flashPlusRendererFg.material.color;
                            Color bg = scrCamera.instance.flashPlusRendererBg.material.color;
                            string fgRgb = string.Format("{0:X2}{1:X2}{2:X2}", (byte)(Mathf.Clamp01(fg.r) * 255f), (byte)(Mathf.Clamp01(fg.g) * 255f), (byte)(Mathf.Clamp01(fg.b) * 255f));
                            string fgRgba = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (byte)(Mathf.Clamp01(fg.r) * 255f), (byte)(Mathf.Clamp01(fg.g) * 255f), (byte)(Mathf.Clamp01(fg.b) * 255f), (byte)(Mathf.Clamp01(fg.a) * 255f));
                            string fgTxt = fg.a == 0
                                ? Settings.flashShowForegroundTextEmpty ? Settings.flashForegroundTextEmpty : ""
                                : Settings.flashForegroundTextFormat
                                .Replace("{red}", ((int)(Mathf.Clamp01(fg.r) * 255)).ToString())
                                .Replace("{blue}", ((int)(Mathf.Clamp01(fg.b) * 255)).ToString())
                                .Replace("{green}", ((int)(Mathf.Clamp01(fg.g) * 255)).ToString())
                                .Replace("{alpha}", ((int)(Mathf.Clamp01(fg.a) * 255)).ToString())
                                .Replace("{rgb}", fgRgb)
                                .Replace("{rgba}", fgRgba);
                            string bgRgb = string.Format("{0:X2}{1:X2}{2:X2}", (byte)((int)(Mathf.Clamp01(bg.r) * 255)), (byte)((int)(Mathf.Clamp01(bg.g) * 255)), (byte)((int)(Mathf.Clamp01(bg.b) * 255)));
                            string bgRgba = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (byte)((int)(Mathf.Clamp01(bg.r) * 255)), (byte)((int)(Mathf.Clamp01(bg.g) * 255)), (byte)((int)(Mathf.Clamp01(bg.b) * 255)), (byte)((int)(Mathf.Clamp01(bg.a) * 255)));
                            string bgTxt = bg.a == 0
                                ? Settings.flashShowBackgroundTextEmpty ? Settings.flashBackgroundTextEmpty : ""
                                : Settings.flashBackgroundTextFormat
                                .Replace("{red}", ((int)(Mathf.Clamp01(bg.r) * 255)).ToString())
                                .Replace("{blue}", ((int)(Mathf.Clamp01(bg.b) * 255)).ToString())
                                .Replace("{green}", ((int)(Mathf.Clamp01(bg.g) * 255)).ToString())
                                .Replace("{alpha}", ((int)(Mathf.Clamp01(bg.a) * 255)).ToString())
                                .Replace("{rgb}", bgRgb)
                                .Replace("{rgba}", bgRgba);
                            return Settings.flashTextOrder == FlashPlane.Foreground ? (fgTxt + Settings.flashTextSeparator + bgTxt) : (bgTxt + Settings.flashTextSeparator + fgTxt);
                        },
                        () => Settings.flashX,
                        () => Settings.flashY,
                        () => Settings.flashTextSize,
                        () => Settings.flashBoldText,
                        () => Settings.flashShadowText,
                        () => Settings.flashTextAlign
                    );
                    flashText.enabled = Settings.flashEnable;
                    VideoBloom bloom = null;
                    bloomText = new GameObject("BloomText").AddComponent<VfxText>();
                    bloomText.Init(
                        () =>
                        {
                            if (bloom == null)
                                bloom = scrCamera.instance.GetComponent<VideoBloom>();
                            if (bloom == null || !bloom.enabled)
                                return Settings.bloomShowTextEmpty ? Settings.bloomTextEmpty : "";
                            string rgb = string.Format("{0:X2}{1:X2}{2:X2}", (byte)(Mathf.Clamp01(bloom.Tint.r) * 255f), (byte)(Mathf.Clamp01(bloom.Tint.g) * 255f), (byte)(Mathf.Clamp01(bloom.Tint.b) * 255f));
                            string rgba = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", (byte)(Mathf.Clamp01(bloom.Tint.r) * 255f), (byte)(Mathf.Clamp01(bloom.Tint.g) * 255f), (byte)(Mathf.Clamp01(bloom.Tint.b) * 255f), (byte)(Mathf.Clamp01(bloom.Tint.a) * 255f));
                            return Settings.bloomTextFormat
                                .Replace("{threshold}", (Math.Round((decimal)bloom.Threshold * (decimal)Math.Pow(10, 2 + Settings.bloomThresholdDecimal)) / (decimal)Math.Pow(10, Settings.bloomThresholdDecimal)).ToString())
                                .Replace("{intensity}", (Math.Round((decimal)bloom.MasterAmount * (decimal)Math.Pow(10, 2 + Settings.bloomIntensityDecimal)) / (decimal)Math.Pow(10, Settings.bloomIntensityDecimal)).ToString())
                                .Replace("{red}", ((int)(Mathf.Clamp01(bloom.Tint.r) * 255)).ToString())
                                .Replace("{blue}", ((int)(Mathf.Clamp01(bloom.Tint.b) * 255)).ToString())
                                .Replace("{green}", ((int)(Mathf.Clamp01(bloom.Tint.g) * 255)).ToString())
                                .Replace("{alpha}", ((int)(Mathf.Clamp01(bloom.Tint.a) * 255)).ToString())
                                .Replace("{rgb}", rgb.ToString())
                                .Replace("{rgba}", rgba.ToString());
                        },
                        () => Settings.bloomX,
                        () => Settings.bloomY,
                        () => Settings.bloomTextSize,
                        () => Settings.bloomBoldText,
                        () => Settings.bloomShadowText,
                        () => Settings.bloomTextAlign
                    );
                    bloomText.enabled = Settings.bloomEnable;
                    entireText = new GameObject("EntireText").AddComponent<VfxText>();
                    entireText.Init(
                        () => Settings.entireTextFormat.Replace("{filterText}", filterText.Text).Replace("{flashText}", flashText.Text).Replace("{bloomText}", bloomText.Text),
                        () => Settings.entireX,
                        () => Settings.entireY,
                        () => Settings.entireTextSize,
                        () => Settings.entireBoldText,
                        () => Settings.entireShadowText,
                        () => Settings.entireTextAlign
                    );
                    entireText.enabled = Settings.entireEnable;
                    try
                    {
                        AccessTools.Method(typeof(OverlayerSupport), "AddTags").Invoke(null, null);
                    }
                    catch (Exception) {
                    }
                    harmony.PatchAll(Assembly.GetExecutingAssembly());
                }
                else
                {
                    UnityEngine.Object.Destroy(filterText);
                    UnityEngine.Object.Destroy(flashText);
                    UnityEngine.Object.Destroy(bloomText);
                    UnityEngine.Object.Destroy(entireText);
                    if (UnityModManager.modEntries.Select(m => m.Info.Id).Contains("Overlayer"))
                        AccessTools.Method(typeof(OverlayerSupport), "RemoveTags").Invoke(null, null);
                    harmony.UnpatchAll(modEntry.Info.Id);
                }
                return true;
            };
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = _ => {
                Logger.Log("Saving Settings...");
                Settings.Save(modEntry);
                Logger.Log("Save Completed!");
            };
            modEntry.OnLateUpdate = (_, __) =>
            {
                if (!inited)
                {
                    inited = true;
                    List<string> enable = GCS.levelEventsInfo[GCS.levelEventTypeString[LevelEventType.SetFilter]].propertiesInfo["intensity"].enableIfVals.Where(tuple => tuple.Item1 == "filter").Select(tuple => tuple.Item2).ToList();
                    foreach (var v in Enum.GetValues(typeof(Filter)))
                        if (!enable.Contains(v.ToString()))
                            onOffTypes.Add((Filter)v);
                    modEntry.OnLateUpdate = null;
                }
            };
            Logger.Log("Loading Settings...");
            Settings = new Settings();
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            Logger.Log("Load Completed!");
            Localization = Localization.Load("1QcrRL6LAs8WxJj_hFsEJa3CLM5g3e8Ya0KQlRKXwdlU", 967104647, modEntry);
        }

        private static bool filterGui = false;
        private static bool flashGui = false;
        private static bool bloomGui = false;
        private static bool entireGui = false;

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

            float Slider(string name, float value, float min, float max, int decimals = -1, params GUILayoutOption[] options)
            {
                GUILayout.Label(name);
                GUILayout.BeginHorizontal();
                float v = GUILayout.HorizontalSlider(value, min, max, options);
                if (decimals > -1)
                    v = (float)(Math.Round(v * Math.Pow(10, decimals)) / Math.Pow(10, decimals));
                GUILayout.Label(decimals <= -1 ? v.ToString() : string.Format($"{{0:0.{Math.Pow(10, decimals).ToString().Substring(1)}}} ", v));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return v;
            }
            string TextField(string name, string value, params GUILayoutOption[] options)
            {
                GUILayout.Label(name);
                return GUILayout.TextField(value, options);
            }
            bool Toggle(string name, string content, bool value, params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(name);
                bool v = GUILayout.Toggle(value, content, options);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                return v;
            }
            bool ToggleableTextField(string name, string toggleContent, bool toggleValue, string textFieldValue, out string v, GUILayoutOption[] toggleOptions, GUILayoutOption[] textFieldOptions)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(name);
                bool bv = GUILayout.Toggle(toggleValue, toggleContent, toggleOptions);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                v = GUILayout.TextField(textFieldValue, textFieldOptions);
                return bv;
            }
            int Toolbar(string name, int value, string[] values, params GUILayoutOption[] options)
            {
                GUILayout.Label(name);
                return GUILayout.Toolbar(value, values, options);
            }
            TextAnchor Anchor(TextAnchor value)
            {
                GUILayout.Label(Localization["sv.settings.textAlign"]);
                return (TextAnchor)GUILayout.SelectionGrid((int)value,
                    new string[] {
                        Localization["textAnchor.upperLeft"], Localization["textAnchor.upperCenter"], Localization["textAnchor.upperRight"],
                        Localization["textAnchor.middleLeft"], Localization["textAnchor.middleCenter"], Localization["textAnchor.middleRight"],
                        Localization["textAnchor.lowerLeft"], Localization["textAnchor.lowerCenter"], Localization["textAnchor.lowerRight"],
                    }, 3);
            }

            GUILayout.BeginHorizontal();
            filterGui = GUILayout.Toggle(filterGui, filterGui ? "▼" : "▶", GUI.skin.label);
            Settings.filterEnable = filterText.enabled = GUILayout.Toggle(Settings.filterEnable, Localization["sv.gui.filter"]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (filterGui)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.position"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.filterX = Slider(Localization["sv.settings.x"], Settings.filterX, 0, 1, 2, GUILayout.Width(300));
                GUILayout.Space(10);
                Settings.filterY = Slider(Localization["sv.settings.y"], Settings.filterY, 0, 1, 2, GUILayout.Width(300));
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.text"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.filterTextFormat = TextField(Localization["sv.settings.filter.textFormat"], Settings.filterTextFormat);
                GUILayout.Space(10);
                Settings.filterIntensityDecimal = (int)Slider(Localization["sv.settings.filter.intensityDecimal"], Settings.filterIntensityDecimal, 0, 5, 0, GUILayout.Width(200));
                GUILayout.Space(10);
                Settings.filterOnOffTextFormat = TextField(Localization["sv.settings.filter.onOffTextFormat"], Settings.filterOnOffTextFormat);
                GUILayout.Space(10);
                Settings.filterTextSeparator = TextField(Localization["sv.settings.textSeparator"], Settings.filterTextSeparator);
                GUILayout.Space(10);
                Settings.filterShowTextEmpty = ToggleableTextField(Localization["sv.settings.filter.textEmpty"], "", Settings.filterShowTextEmpty, Settings.filterTextEmpty, out string v, new GUILayoutOption[0], new GUILayoutOption[0]);
                Settings.filterTextEmpty = v;
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.textEffect"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.filterTextSize = (int)Slider(Localization["sv.settings.textSize"], Settings.filterTextSize, 1f, 50f, 0, GUILayout.Width(200));
                GUILayout.Space(10);
                Settings.filterBoldText = Toggle(Localization["sv.settings.boldText"], "", Settings.filterBoldText);
                GUILayout.Space(10);
                Settings.filterShadowText = Toggle(Localization["sv.settings.shadowText"], "", Settings.filterShadowText);
                GUILayout.Space(10);
                Settings.filterTextOrder = (FilterTextOrder)Toolbar(Localization["sv.settings.filter.textOrder"], (int)Settings.filterTextOrder,
                    new string[] {
                        Localization["filterTextOrder.ascending"], Localization["filterTextOrder.descending"], Localization["filterTextOrder.added"], Localization["filterTextOrder.reverseAdded"]
                    }
                );
                GUILayout.Space(10);
                Settings.filterTextAlign = Anchor(Settings.filterTextAlign);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            flashGui = GUILayout.Toggle(flashGui, flashGui ? "▼" : "▶", GUI.skin.label);
            Settings.flashEnable = flashText.enabled = GUILayout.Toggle(Settings.flashEnable, Localization["sv.gui.flash"]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (flashGui)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.position"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.flashX = Slider(Localization["sv.settings.x"], Settings.flashX, 0, 1, 2, GUILayout.Width(300));
                GUILayout.Space(10);
                Settings.flashY = Slider(Localization["sv.settings.y"], Settings.flashY, 0, 1, 2, GUILayout.Width(300));
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.text"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.flashForegroundTextFormat = TextField(Localization["sv.settings.flash.foregroundTextFormat"], Settings.flashForegroundTextFormat);
                GUILayout.Space(10);
                Settings.flashBackgroundTextFormat = TextField(Localization["sv.settings.flash.backgroundTextFormat"], Settings.flashBackgroundTextFormat);
                GUILayout.Space(10);
                Settings.flashTextSeparator = TextField(Localization["sv.settings.textSeparator"], Settings.flashTextSeparator);
                GUILayout.Space(10);
                Settings.flashShowForegroundTextEmpty = ToggleableTextField(Localization["sv.settings.flash.foregroundTextEmpty"], "", Settings.flashShowForegroundTextEmpty, Settings.flashForegroundTextEmpty, out string v, new GUILayoutOption[0], new GUILayoutOption[0]);
                Settings.flashForegroundTextEmpty = v;
                GUILayout.Space(10);
                Settings.flashShowBackgroundTextEmpty = ToggleableTextField(Localization["sv.settings.flash.backgroundTextEmpty"], "", Settings.flashShowBackgroundTextEmpty, Settings.flashBackgroundTextEmpty, out v, new GUILayoutOption[0], new GUILayoutOption[0]);
                Settings.flashBackgroundTextEmpty = v;
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.textEffect"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.flashTextSize = (int)Slider(Localization["sv.settings.textSize"], Settings.flashTextSize, 1f, 50f, 0, GUILayout.Width(200));
                GUILayout.Space(10);
                Settings.flashBoldText = Toggle(Localization["sv.settings.boldText"], "", Settings.flashBoldText);
                GUILayout.Space(10);
                Settings.flashShadowText = Toggle(Localization["sv.settings.shadowText"], "", Settings.flashShadowText);
                GUILayout.Space(10);
                Settings.flashTextOrder = (FlashPlane)Toolbar(Localization["sv.settings.flash.textOrder"], (int)Settings.flashTextOrder,
                    new string[] {
                        Localization["sv.settings.flash.textOrder.foreAndBack"], Localization["sv.settings.flash.textOrder.backAndFore"]
                    }
                );
                GUILayout.Space(10);
                Settings.flashTextAlign = Anchor(Settings.flashTextAlign);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            bloomGui = GUILayout.Toggle(bloomGui, bloomGui ? "▼" : "▶", GUI.skin.label);
            Settings.bloomEnable = bloomText.enabled = GUILayout.Toggle(Settings.bloomEnable, Localization["sv.gui.bloom"]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (bloomGui)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.position"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.bloomX = Slider(Localization["sv.settings.x"], Settings.bloomX, 0, 1, 2, GUILayout.Width(300));
                GUILayout.Space(10);
                Settings.bloomY = Slider(Localization["sv.settings.y"], Settings.bloomY, 0, 1, 2, GUILayout.Width(300));
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.text"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.bloomTextFormat = TextField(Localization["sv.settings.bloom.textFormat"], Settings.bloomTextFormat);
                GUILayout.Space(10);
                Settings.bloomThresholdDecimal = (int)Slider(Localization["sv.settings.bloom.thresholdDecimal"], Settings.bloomThresholdDecimal, 0, 6, 0, GUILayout.Width(200));
                GUILayout.Space(10);
                Settings.bloomIntensityDecimal = (int)Slider(Localization["sv.settings.bloom.intensityDecimal"], Settings.bloomIntensityDecimal, 0, 4, 0, GUILayout.Width(200));
                GUILayout.Space(10);
                Settings.bloomShowTextEmpty = ToggleableTextField(Localization["sv.settings.bloom.textEmpty"], "", Settings.bloomShowTextEmpty, Settings.bloomTextEmpty, out string v, new GUILayoutOption[0], new GUILayoutOption[0]);
                Settings.bloomTextEmpty = v;
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.textEffect"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.bloomTextSize = (int)Slider(Localization["sv.settings.textSize"], Settings.bloomTextSize, 1f, 50f, 0, GUILayout.Width(200));
                GUILayout.Space(10);
                Settings.bloomBoldText = Toggle(Localization["sv.settings.boldText"], "", Settings.bloomBoldText);
                GUILayout.Space(10);
                Settings.bloomShadowText = Toggle(Localization["sv.settings.shadowText"], "", Settings.bloomShadowText);
                GUILayout.Space(10);
                Settings.bloomTextAlign = Anchor(Settings.bloomTextAlign);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            entireGui = GUILayout.Toggle(entireGui, entireGui ? "▼" : "▶", GUI.skin.label);
            Settings.entireEnable = entireText.enabled = GUILayout.Toggle(Settings.entireEnable, Localization["sv.gui.entire"]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (entireGui)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.position"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.entireX = Slider(Localization["sv.settings.x"], Settings.entireX, 0, 1, 2, GUILayout.Width(300));
                GUILayout.Space(10);
                Settings.entireY = Slider(Localization["sv.settings.y"], Settings.entireY, 0, 1, 2, GUILayout.Width(300));
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.text"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.entireTextFormat = TextField(Localization["sv.settings.entire.textFormat"], Settings.entireTextFormat);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(10);

                GUILayout.BeginVertical();
                GUILayout.Label(Localization["sv.gui.textEffect"]);
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUI.skin.box);
                Settings.entireTextSize = (int)Slider(Localization["sv.settings.textSize"], Settings.entireTextSize, 1f, 50f, 0, GUILayout.Width(200));
                GUILayout.Space(10);
                Settings.entireBoldText = Toggle(Localization["sv.settings.boldText"], "", Settings.entireBoldText);
                GUILayout.Space(10);
                Settings.entireShadowText = Toggle(Localization["sv.settings.shadowText"], "", Settings.entireShadowText);
                GUILayout.Space(10);
                Settings.entireTextAlign = Anchor(Settings.entireTextAlign);
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
            if (GUILayout.Button(Localization["sv.settings.help"], GUILayout.Width(70)))
                Application.OpenURL(Localization["sv.settings.helpLink"]);
        }
    }
}