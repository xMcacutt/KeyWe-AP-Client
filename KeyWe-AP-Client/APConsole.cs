using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace KeyWe_AP_Client;

public class APConsole : MonoBehaviour
{
    private const float MessageHeight = 28f;
    private const float ConsoleHeight = 280f;

    private const float SlideInTime = 0.25f;
    private const float HoldTime = 3.0f;
    private const float FadeOutTime = 0.5f;

    private const float SlideInOffset = -50f;
    private const float FadeUpOffset = 20f;

    private const float PaddingX = 25f;
    private const float PaddingY = 25f;

    private const float MessageSpacing = 6f;

    private static readonly Dictionary<string, string> KeywordColors = new()
    {
        { "summer", "#00ff00" },
        { "fall", "#ff6600" },
        { "winter", "#3399ff" },
        { "warning", "#ff0000"},
        { "movement+", "#ffff00"},
        { "respawn+", "#ffff00"},
        { "dash+", "#ffff00"},
        { "jump+", "#ffff00"},
        { "swim+", "#ffff00"},
        { "chirp+", "#ffff00"},
        { "peck+", "#ffff00"},
        { "random facewear", "#ff00ff"},
        { "random hat", "#ff00ff"},
        { "random skin", "#ff00ff"},
        { "random footwear", "#ff00ff"},
        { "random backwear", "#ff00ff"},
        { "random hairstyle", "#ff00ff"},
        { "random arms", "#ff00ff"}
    };

    private static readonly Regex KeywordRegex = new(
        string.Join("|", KeywordColors.Keys.Select(Regex.Escape)),
        RegexOptions.IgnoreCase
    );

    private readonly Queue<Image> _backgroundPool = new();

    private readonly Queue<LogEntry> _cachedEntries = new();

    private readonly Queue<Text> _textPool = new();
    private readonly List<LogEntry> _visibleEntries = new();

    private Transform _messageParent;

    private bool _showConsole = true;

    public static APConsole Instance { get; private set; }

    private void Update()
    {
        UpdateMessages(Time.deltaTime);
        TryAddNewMessages();
        if (Input.GetKeyDown(KeyCode.F7))
            ToggleConsole();
    }

    public static void Create()
    {
        if (Instance != null)
            return;
        File.WriteAllText(SaveSystem.DataRoot + "ArchieplagoDebugLog.txt", "");
        var consoleObject = new GameObject("ArchipelagoConsoleUI");
        DontDestroyOnLoad(consoleObject);
        Instance = consoleObject.AddComponent<APConsole>();
        Instance.BuildUI();
    }

    private void UpdateMessages(float delta)
    {
        for (var i = _visibleEntries.Count - 1; i >= 0; i--)
        {
            var e = _visibleEntries[i];
            var done = AnimateEntry(e, delta);

            if (done)
            {
                RecycleEntry(e);
                _visibleEntries.RemoveAt(i);
                RecalculateBaseY();
            }
            else
            {
                UpdateEntryVisual(e);
            }
        }
    }

    private void RecalculateBaseY()
    {
        var y = 0f;
        for (var i = _visibleEntries.Count - 1; i >= 0; i--)
        {
            var e = _visibleEntries[i];
            e.baseY = y;
            y += e.height + MessageSpacing;
        }
    }

    private bool AnimateEntry(LogEntry entry, float delta)
    {
        entry.stateTimer += delta;

        switch (entry.state)
        {
            case LogEntry.State.SlideIn:
            {
                var t = Mathf.Clamp01(entry.stateTimer / SlideInTime);
                entry.offsetY = Mathf.Lerp(SlideInOffset, 0f, EaseOutQuad(t));

                if (t >= 1f)
                {
                    entry.state = LogEntry.State.Hold;
                    entry.stateTimer = 0f;
                }
            }
                break;

            case LogEntry.State.Hold:
            {
                entry.offsetY = 0f;
                if (entry.stateTimer >= HoldTime)
                {
                    entry.state = LogEntry.State.FadeOut;
                    entry.stateTimer = 0f;
                }
            }
                break;

            case LogEntry.State.FadeOut:
            {
                var t = Mathf.Clamp01(entry.stateTimer / FadeOutTime);
                entry.offsetY = Mathf.Lerp(0f, FadeUpOffset, t);
                var alpha = 1f - t;
                entry.text.color = new Color(1f, 1f, 1f, alpha);
                entry.background.color = new Color(0f, 0f, 0f, 0.8f * alpha);

                if (t >= 1f)
                    return true;
            }
                break;
        }

        return false;
    }

    private static float EaseOutQuad(float x)
    {
        return 1f - (1f - x) * (1f - x);
    }

    private void TryAddNewMessages()
    {
        if (!_cachedEntries.Any())
            return;

        var maxMessages = Mathf.FloorToInt(ConsoleHeight / MessageHeight);
        if (_visibleEntries.Count >= maxMessages)
            return;

        var entry = _cachedEntries.Dequeue();
        entry.state = LogEntry.State.SlideIn;
        entry.stateTimer = 0f;

        entry.offsetY = SlideInOffset;
        entry.animatedY = entry.baseY + entry.offsetY;

        CreateEntryVisual(entry);

        _visibleEntries.Add(entry);
        RecalculateBaseY();
        entry.animatedY = entry.baseY + entry.offsetY;
    }

    private void CreateEntryVisual(LogEntry entry)
    {
        var bg = GetBackground();
        bg.transform.SetParent(_messageParent, false);

        var bgRect = bg.rectTransform;
        bgRect.anchorMin = new Vector2(0, 0);
        bgRect.anchorMax = new Vector2(0, 0);
        bgRect.pivot = new Vector2(0, 0);
        bgRect.sizeDelta = new Vector2(600f, MessageHeight);

        var text = GetText();
        var tRect = text.rectTransform;
        tRect.SetParent(bg.transform, false);

        tRect.anchorMin = new Vector2(0, 0);
        tRect.anchorMax = new Vector2(1, 1);
        tRect.pivot = new Vector2(0, 0.5f);
        tRect.offsetMin = new Vector2(8f, 4f);
        tRect.offsetMax = new Vector2(-8f, -4f);

        entry.text = text;
        entry.background = bg;
        text.color = new Color(1, 1, 1, 1);
        bg.color = new Color(0, 0, 0, 0.8f);

        UpdateEntryVisual(entry);
    }

    private void UpdateEntryVisual(LogEntry entry)
    {
        entry.text.text = Colorize(entry.message);

        var bgRect = entry.background.rectTransform;
        var textHeight = entry.text.preferredHeight;
        entry.height = Mathf.Max(MessageHeight, textHeight + 8f);
        bgRect.sizeDelta = new Vector2(bgRect.sizeDelta.x, entry.height);

        var targetY = entry.baseY + entry.offsetY;
        entry.animatedY = Mathf.Lerp(entry.animatedY, targetY, Time.deltaTime * 12f);

        entry.background.rectTransform.anchoredPosition =
            new Vector2(0f, entry.animatedY);
    }

    private Text GetText()
    {
        if (_textPool.Count > 0)
        {
            var t = _textPool.Dequeue();
            t.gameObject.SetActive(true);
            return t;
        }

        var go = new GameObject("LogText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        var t2 = go.GetComponent<Text>();
        t2.fontSize = 18;
        t2.color = Color.white;
        t2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        t2.alignment = TextAnchor.MiddleLeft;
        t2.horizontalOverflow = HorizontalWrapMode.Wrap;
        t2.verticalOverflow = VerticalWrapMode.Overflow;

        return t2;
    }

    private Image GetBackground()
    {
        if (_backgroundPool.Count > 0)
        {
            var img = _backgroundPool.Dequeue();
            img.gameObject.SetActive(true);
            return img;
        }

        var go = new GameObject("LogBG");
        var imgNew = go.AddComponent<Image>();
        imgNew.color = new Color(0, 0, 0, 0.8f);
        imgNew.type = Image.Type.Sliced;

        return imgNew;
    }

    private void RecycleEntry(LogEntry entry)
    {
        entry.text.gameObject.SetActive(false);
        entry.background.gameObject.SetActive(false);

        _textPool.Enqueue(entry.text);
        _backgroundPool.Enqueue(entry.background);
    }

    private string Colorize(string input)
    {
        return KeywordRegex.Replace(input, match =>
        {
            var key = match.Value.ToLower();
            return KeywordColors.TryGetValue(key, out var hex)
                ? $"<color={hex}>{match.Value}</color>"
                : match.Value;
        });
    }

    private const bool LogToFile = true;

    public void Log(string text)
    {
        var entry = new LogEntry(text);
        if (LogToFile)
            File.AppendAllLines(SaveSystem.DataRoot + "ArchipelagoDebugLog.txt", [text]);
        _cachedEntries.Enqueue(entry);
    }

    private void ToggleConsole()
    {
        _showConsole = !_showConsole;
        foreach (var e in _visibleEntries)
        {
            if (e?.background != null)
                e.background.gameObject.SetActive(_showConsole);
            if (e?.text != null)
                e.text.gameObject.SetActive(_showConsole);
        }

        _messageParent.gameObject.SetActive(_showConsole);
    }

    private void BuildUI()
    {
        var canvasObject = new GameObject("APConsoleCanvas", typeof(Canvas), typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform);

        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 2000;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        var container = new GameObject("Messages", typeof(RectTransform));
        var rect = container.GetComponent<RectTransform>();
        rect.SetParent(canvasObject.transform, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = new Vector2(PaddingX, PaddingY);

        _messageParent = container.transform;
    }

    [Serializable]
    public class LogEntry(string msg)
    {
        public enum State
        {
            SlideIn,
            Hold,
            FadeOut
        }

        public State state = State.SlideIn;

        public float stateTimer;
        public float offsetY;
        public float baseY;
        public float animatedY;

        public Text text;
        public Image background;

        public string message = msg;
        public float height = MessageHeight;
    }
}