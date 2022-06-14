using ADOFAI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ShowCurrentFilters
{
    internal class FilterText : MonoBehaviour
    {
        internal static List<Filter> onOffTypes;

        private GameObject textObject;
        private RectTransform rect;
        private Text text;
        private Shadow shadow;

        private void Awake()
        {
            instance = gameObject;
            DontDestroyOnLoad(this);
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10000;
            CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            textObject = new GameObject("Canvas");
            textObject.transform.SetParent(transform);
            textObject.AddComponent<Canvas>();
            rect = textObject.GetComponent<RectTransform>();
            GameObject obj = new GameObject("Text");
            obj.transform.SetParent(textObject.transform);
            text = obj.AddComponent<Text>();
            text.text = "테스트텍스트";
            text.color = Color.white;
            text.alignByGeometry = true;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            shadow = obj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            shadow.effectDistance = new Vector2(2f, -2f);
            Vector2 vector = new Vector2(Main.Settings.x, Main.Settings.y);
            rect.anchorMin = vector;
            rect.anchorMax = vector;
            rect.pivot = vector;
            rect.anchoredPosition = Vector2.zero;
        }

        private void Start()
        {
            text.font = RDString.fontData.font;
        }

        private float lastX = float.MaxValue;
        private float lastY = float.MaxValue;
        private int lastSize = int.MaxValue;
        private string lastText = null;

        private void LateUpdate()
        {
            textObject.SetActive((!scrController.instance?.paused).GetValueOrDefault() && (scrConductor.instance?.isGameWorld).GetValueOrDefault());
            if (!textObject.activeSelf)
                return;
            if (FilterPatch.filters.Count == 0)
                text.text = Main.Settings.textEmpty;
            else
                text.text = string.Join(Main.Settings.textSeparator == "{newLine}" ? "\n" : Main.Settings.textSeparator, FilterPatch.filters.Select(pair =>
                    onOffTypes != null && onOffTypes.Contains(pair.Key)
                        ? Main.Settings.textFormat2.Replace("{name}", RDString.GetEnumValue(pair.Key)).Replace("{value}", Main.Localization["scf.filterOn"])
                        : Main.Settings.textFormat1.Replace("{name}", RDString.GetEnumValue(pair.Key)).Replace("{value}", (pair.Value * 100).ToString())));
            text.fontSize = Main.Settings.textSize;
            text.fontStyle = Main.Settings.boldText ? FontStyle.Bold : FontStyle.Normal;
            text.alignment = Main.Settings.textAlign;
            shadow.enabled = Main.Settings.shadowText;
            if (lastSize != text.fontSize || lastText != text.text)
            {
                Vector2 vector = new Vector2(text.preferredWidth, text.preferredHeight);
                rect.sizeDelta = vector;
                text.rectTransform.sizeDelta = vector;
            }
            if (lastX != Main.Settings.x || lastY != Main.Settings.y)
            {
                Vector2 vector = new Vector2(Main.Settings.x, Main.Settings.y);
                rect.anchorMin = vector;
                rect.anchorMax = vector;
                rect.pivot = vector;
            }
            lastX = Main.Settings.x;
            lastY = Main.Settings.y;
            lastSize = text.fontSize;
            lastText = text.text;
        }

        internal static void AddOrDelete(bool value)
        {
            if (value)
            {
                if (instance != null)
                    return;
                GameObject obj = new GameObject("FilterText");
                obj.AddComponent<FilterText>();
            } else
            {
                if (instance != null)
                    Destroy(instance);
            }
        }

        internal static void LoadOnOffTypes()
        {
            if (onOffTypes != null)
                return;
            onOffTypes = new List<Filter>();
            List<string> enable = GCS.levelEventsInfo[GCS.levelEventTypeString[LevelEventType.SetFilter]].propertiesInfo["intensity"].enableIfVals.Where(tuple => tuple.Item1 == "filter").Select(tuple => tuple.Item2).ToList();
            foreach (var v in Enum.GetValues(typeof(Filter)))
                if (!enable.Contains(v.ToString()))
                    onOffTypes.Add((Filter)v);
        }

        private static GameObject instance;
    }
}
