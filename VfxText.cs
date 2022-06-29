using System;
using UnityEngine;
using UnityEngine.UI;

namespace ShowVFXs
{
    internal class VfxText : MonoBehaviour
    {
        private GameObject textObject;
        private RectTransform rect;
        private Text text;
        private Shadow shadow;

        private Func<string> textGetter;
        private Func<float> xGetter;
        private Func<float> yGetter;
        private Func<int> textSizeGetter;
        private Func<bool> boldGetter;
        private Func<bool> shadowGetter;
        private Func<TextAnchor> alignGetter;
        private bool inited = false;

        public new bool enabled = true;

        private void Awake()
        {
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
            text.alignByGeometry = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            shadow = obj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            shadow.effectDistance = new Vector2(2f, -2f);
            rect.anchoredPosition = Vector2.zero;
        }

        internal void Init(Func<string> textGetter, Func<float> xGetter, Func<float> yGetter, Func<int> textSizeGetter, Func<bool> boldGetter, Func<bool> shadowGetter, Func<TextAnchor> alignGetter)
        {
            if (inited)
                return;
            this.textGetter = textGetter;
            this.xGetter = xGetter;
            this.yGetter = yGetter;
            this.textSizeGetter = textSizeGetter;
            this.boldGetter = boldGetter;
            this.shadowGetter = shadowGetter;
            this.alignGetter = alignGetter;
            inited = true;
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
            if (!inited)
                return;
            textObject.SetActive(enabled && (!scrController.instance?.paused).GetValueOrDefault() && (scrConductor.instance?.isGameWorld).GetValueOrDefault());
            if (!textObject.activeSelf)
                return;
            text.text = textGetter.Invoke()?.Replace("{newLine}", "\n");
            text.fontSize = textSizeGetter.Invoke();
            text.fontStyle = boldGetter.Invoke() ? FontStyle.Bold : FontStyle.Normal;
            shadow.enabled = shadowGetter.Invoke();
            text.alignment = alignGetter.Invoke();
            if (lastSize != text.fontSize || lastText != text.text)
                Resize();
            float x = xGetter.Invoke(), y = yGetter.Invoke();
            if (lastX != x || lastY != y)
            {
                Vector2 vector = new Vector2(x, y);
                rect.anchorMin = vector;
                rect.anchorMax = vector;
                rect.pivot = vector;
            }
            lastX = x;
            lastY = y;
            lastSize = text.fontSize;
            lastText = text.text;
        }

        private void Resize()
        {
            Vector2 vector = new Vector2((float)(Math.Ceiling(text.preferredWidth / 10) * 10), (float)(Math.Ceiling(text.preferredHeight * 10) / 10));
            rect.sizeDelta = vector;
            text.rectTransform.sizeDelta = vector;
        }

        internal string Text => enabled ? text.text : textGetter.Invoke()?.Replace("{newLine}", "\n");
    }
}
