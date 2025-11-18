using System;
using System.Collections;
using System.Linq;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KeyWe_AP_Client;

public class LoginMenuHandler : MonoBehaviour
{
    private static MessageWindow messageWindow;
    private static string _password = "";
    private static ushort _port;
    private static string _server = "archipelago.gg";
    private static bool _showLogin;
    private static string _slotName = "";
    private static string _status = "";

    public static bool IsTyping;
    private GameObject _introClipboard;
    private Rect _windowRect = new(100, 100, 600, 500);


    private void Start()
    {
        StartCoroutine(CheckProfileSelect());
        ConnectionInfoHandler.Load(ref _server, ref _port, ref _slotName, ref _password);
    }

    private IEnumerator CheckProfileSelect()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _introClipboard = GameObject.Find("/Director/Attract/Clipboard");
            if (_introClipboard == null || _showLogin) yield return null;
            _showLogin = true;
            Destroy(_introClipboard);
        }
    }

    private static void Connect()
    {
        ConnectionInfoHandler.Save(_server, _port, _slotName, _password);
        PluginMain.ArchipelagoHandler = new ArchipelagoHandler(_server, _port, _slotName, _password);
        PluginMain.ArchipelagoHandler.InitConnect();
        _status = "Connecting...";
    }

    [HarmonyPatch(typeof(Menu))]
    public class Menu_Patch
    {
        [HarmonyPatch("Focus")]
        [HarmonyPrefix]
        public static bool Focus(Menu __instance, bool isFocused, GameObject control = null)
        {
            return !IsTyping;
        }
    }

    [HarmonyPatch(typeof(MessageWindow))]
    public class MessageWindow_Patch
    {
        [HarmonyPatch("Opened")]
        [HarmonyPostfix]
        public static void Opened_Postfix(MessageWindow __instance)
        {
            if (ArchipelagoHandler.IsConnected || ArchipelagoHandler.IsConnecting)
                return;
            var windowTransform = __instance.textDisplay.gameObject.transform;
            __instance.textDisplay.text = "";
            __instance.canvas.GetComponentInChildren<TMP_Text>().text = "Archipelago";
            __instance.loneYupButton.GetComponentInChildren<TMP_Text>().text = "Connect";
            var template = __instance.canvas.GetComponentInChildren<TextMeshProUGUI>();
            AddLabel(windowTransform, "Server", new Vector3(-2.15f, 1.5f, 0.0f), template);
            AddLabel(windowTransform, "Port", new Vector3(-2.38f, 0.75f, 0.0f), template);
            AddLabel(windowTransform, "Slot", new Vector3(-2.45f, 0f, 0.0f), template);
            AddLabel(windowTransform, "Password", new Vector3(-2.0f, -0.75f, 0.0f), template);
            var serverField = AddInputField(windowTransform, "ServerInput", new Vector3(1f, 1.5f, 0.0f),
                template, text => _server = text);
            __instance.FirstSelected = serverField.gameObject;
            EventSystem.current.SetSelectedGameObject(serverField.gameObject);
            serverField.text = _server;
            var portField = AddInputField(windowTransform, "PortInput", new Vector3(1f, 0.75f, 0.0f),
                template, text => { ushort.TryParse(text, out _port); });
            portField.text = _port.ToString();
            var slotField = AddInputField(windowTransform, "SlotInput", new Vector3(1f, 0f, 0.0f),
                template, text => _slotName = text);
            slotField.text = _slotName;
            var passwordField = AddInputField(windowTransform, "PasswordInput", new Vector3(1f, -0.75f, 0.0f),
                template, text => _password = text, true);
            passwordField.text = _password;
        }

        private static void AddLabel(Transform reference, string label, Vector3 localPosition, TMP_Text template)
        {
            var textObject = new GameObject($"AP {label} Label");
            textObject.transform.SetParent(reference, false);
            var newText = textObject.AddComponent<TextMeshProUGUI>();
            newText.text = label;
            SetTextSettings(template, newText);
            var rect = newText.GetComponent<RectTransform>();
            rect.localPosition = localPosition;
        }

        private static TMP_InputField AddInputField(Transform parent, string name,
            Vector3 localPosition, TMP_Text template, Action<string> onValueChanged, bool isPassword = false)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);

            root.AddComponent<CanvasRenderer>();

            var rect = root.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(4f, 0.5f);
            rect.localPosition = localPosition;

            var bg = root.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.4f);

            var inputField = root.AddComponent<TMP_InputField>();
            var viewport = new GameObject("TextViewport", typeof(RectTransform));
            viewport.transform.SetParent(root.transform, false);

            var viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = new Vector2(0, 0);
            viewportRect.anchorMax = new Vector2(1, 1);
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            inputField.textViewport = viewportRect;
            inputField.targetGraphic = bg;
            inputField.onSelect.AddListener(_ => IsTyping = true);
            inputField.onDeselect.AddListener(_ => IsTyping = false);
            var textObject = new GameObject("Text");
            textObject.transform.SetParent(viewport.transform, false);

            var text = textObject.AddComponent<TextMeshProUGUI>();
            SetTextSettings(template, text);
            text.enableWordWrapping = false;
            text.alignment = TextAlignmentOptions.MidlineLeft;

            var textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.025f, 0);
            textRect.anchorMax = new Vector2(0.975f, 1);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            inputField.textComponent = text;
            if (isPassword)
            {
                inputField.inputType = TMP_InputField.InputType.Password;
                inputField.contentType = TMP_InputField.ContentType.Password;
            }
            else
            {
                inputField.contentType = TMP_InputField.ContentType.Standard;
            }

            inputField.onValueChanged.AddListener(onValueChanged.Invoke);
            return inputField;
        }

        private static void SetTextSettings(TMP_Text template, TMP_Text text)
        {
            text.font = template.font;
            text.fontSize = 0.35f;
            text.color = template.color;
            text.alignment = template.alignment;
            text.enableWordWrapping = template.enableWordWrapping;
            text.margin = template.margin;
            text.fontMaterial = new Material(template.fontMaterial);
        }

        [HarmonyPatch("OnYupSelected")]
        [HarmonyPrefix]
        public static bool Yup_Prefix(MessageWindow __instance)
        {
            if (ArchipelagoHandler.IsConnected)
                return true;
            ArchipelagoHandler.OnConnect += () => { __instance.CloseAnimationStart(); };
            Connect();
            return false;
        }
    }

    [HarmonyPatch(typeof(StartScreen))]
    public class StartScreen_Patch
    {
        [HarmonyPatch(typeof(StartScreen), "Opened")]
        [HarmonyPostfix]
        private static void Postfix(StartScreen __instance)
        {
            if (ArchipelagoHandler.IsConnected || ArchipelagoHandler.IsConnecting)
                return;
            var messageWindowPrefab = Resources.FindObjectsOfTypeAll<MessageWindow>().FirstOrDefault();
            var ui = SystemHandler.Get<UIController>();
            messageWindow = ui.OpenMenu(messageWindowPrefab, m => { });
        }

        [HarmonyPatch("GetInput")]
        [HarmonyPrefix]
        private static bool OnGetInput()
        {
            return false;
        }
    }
}