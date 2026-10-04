using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuSceneBuilder
{
    // ──────────────────────────────────────────────────────────
    //  Design tokens
    // ──────────────────────────────────────────────────────────
    static readonly Color BG_COLOR         = new Color(0.051f, 0.063f, 0.094f, 1f); // #0D1018
    static readonly Color TITLE_COLOR      = new Color(1.00f,  1.00f,  1.00f,  1f);
    static readonly Color SUBTITLE_COLOR   = new Color(0.60f,  0.64f,  0.74f,  1f);
    static readonly Color DIVIDER_COLOR    = new Color(0.22f,  0.26f,  0.37f,  1f);
    static readonly Color BUTTON_COLOR     = new Color(0.11f,  0.14f,  0.22f,  1f); // #1C2438
    static readonly Color BUTTON_HL_COLOR  = new Color(0.18f,  0.23f,  0.36f,  1f);
    static readonly Color BUTTON_PR_COLOR  = new Color(0.07f,  0.09f,  0.14f,  1f);
    static readonly Color BTN_TEXT_COLOR   = new Color(1.00f,  1.00f,  1.00f,  1f);

    const float BUTTON_W   = 380f;
    const float BUTTON_H   = 72f;
    const float BTN_SPACING = 20f;

    // ──────────────────────────────────────────────────────────
    //  Entry point
    // ──────────────────────────────────────────────────────────
    public static void Execute()
    {
        // New empty scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        BuildCamera();
        BuildEventSystem();
        BuildCanvas();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
        AssetDatabase.Refresh();
        Debug.Log("[MainMenuSceneBuilder] MainMenu scene created and saved.");
    }

    // ──────────────────────────────────────────────────────────
    //  Scene-level objects
    // ──────────────────────────────────────────────────────────
    static void BuildCamera()
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        var cam = go.AddComponent<Camera>();
        cam.clearFlags      = CameraClearFlags.SolidColor;
        cam.backgroundColor = BG_COLOR;
        cam.orthographic    = false;
        go.AddComponent<AudioListener>();
    }

    static void BuildEventSystem()
    {
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    // ──────────────────────────────────────────────────────────
    //  Canvas + children
    // ──────────────────────────────────────────────────────────
    static void BuildCanvas()
    {
        // ── Canvas root ──────────────────────────────────────
        var canvasGO = new GameObject("Canvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode        = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode    = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        var root = canvasGO.transform;

        // ── Background ───────────────────────────────────────
        var bg  = MakeRect("Background_Panel", root);
        var bgI = bg.AddComponent<Image>();
        bgI.color = BG_COLOR;
        Stretch(bg.GetComponent<RectTransform>());

        // ── Title ─────────────────────────────────────────────
        var titleGO  = MakeRect("Title_Text", root);
        var titleTMP = titleGO.AddComponent<TextMeshProUGUI>();
        titleTMP.text      = "GAME TITLE";
        titleTMP.fontSize  = 96;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.color     = TITLE_COLOR;
        titleTMP.characterSpacing = 8f;

        var titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin        = new Vector2(0f, 1f);
        titleRT.anchorMax        = new Vector2(1f, 1f);
        titleRT.pivot            = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0f, -100f);
        titleRT.sizeDelta        = new Vector2(0f, 140f);

        // ── Subtitle ─────────────────────────────────────────
        var subGO  = MakeRect("Subtitle_Text", root);
        var subTMP = subGO.AddComponent<TextMeshProUGUI>();
        subTMP.text      = "A Prototype Adventure";
        subTMP.fontSize  = 30f;
        subTMP.fontStyle = FontStyles.Italic;
        subTMP.alignment = TextAlignmentOptions.Center;
        subTMP.color     = SUBTITLE_COLOR;

        var subRT = subGO.GetComponent<RectTransform>();
        subRT.anchorMin        = new Vector2(0f, 1f);
        subRT.anchorMax        = new Vector2(1f, 1f);
        subRT.pivot            = new Vector2(0.5f, 1f);
        subRT.anchoredPosition = new Vector2(0f, -256f);
        subRT.sizeDelta        = new Vector2(0f, 50f);

        // ── Divider line ─────────────────────────────────────
        var divGO  = MakeRect("Divider_Line", root);
        var divImg = divGO.AddComponent<Image>();
        divImg.color = DIVIDER_COLOR;

        var divRT = divGO.GetComponent<RectTransform>();
        divRT.anchorMin        = new Vector2(0.5f, 0.5f);
        divRT.anchorMax        = new Vector2(0.5f, 0.5f);
        divRT.pivot            = new Vector2(0.5f, 0.5f);
        divRT.anchoredPosition = new Vector2(0f, 220f);
        divRT.sizeDelta        = new Vector2(440f, 2f);

        // ── Buttons container ─────────────────────────────────
        //  4 buttons × 72 + 3 gaps × 20 = 288 + 60 = 348
        float containerH = 4f * BUTTON_H + 3f * BTN_SPACING;

        var containerGO = MakeRect("Buttons_Container", root);
        var vlg = containerGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing              = BTN_SPACING;
        vlg.childAlignment       = TextAnchor.MiddleCenter;
        vlg.childControlWidth    = false;
        vlg.childControlHeight   = false;
        vlg.childForceExpandWidth  = false;
        vlg.childForceExpandHeight = false;
        vlg.padding              = new RectOffset(0, 0, 0, 0);

        var containerRT = containerGO.GetComponent<RectTransform>();
        containerRT.anchorMin        = new Vector2(0.5f, 0.5f);
        containerRT.anchorMax        = new Vector2(0.5f, 0.5f);
        containerRT.pivot            = new Vector2(0.5f, 0.5f);
        containerRT.anchoredPosition = new Vector2(0f, -30f);
        containerRT.sizeDelta        = new Vector2(BUTTON_W, containerH);

        // ── Buttons ───────────────────────────────────────────
        string[] labels = { "Play", "Options", "Credits", "Quit" };
        foreach (var lbl in labels)
            BuildButton(containerGO.transform, lbl);

        // ── Version stamp ─────────────────────────────────────
        var verGO  = MakeRect("Version_Text", root);
        var verTMP = verGO.AddComponent<TextMeshProUGUI>();
        verTMP.text      = "v0.1 — Prototype Build";
        verTMP.fontSize  = 18f;
        verTMP.alignment = TextAlignmentOptions.BottomRight;
        verTMP.color     = new Color(0.35f, 0.38f, 0.48f, 1f);

        var verRT = verGO.GetComponent<RectTransform>();
        verRT.anchorMin        = new Vector2(1f, 0f);
        verRT.anchorMax        = new Vector2(1f, 0f);
        verRT.pivot            = new Vector2(1f, 0f);
        verRT.anchoredPosition = new Vector2(-24f, 20f);
        verRT.sizeDelta        = new Vector2(360f, 36f);
    }

    // ──────────────────────────────────────────────────────────
    //  Button builder
    // ──────────────────────────────────────────────────────────
    static void BuildButton(Transform parent, string label)
    {
        // Root
        var btnGO  = MakeRect("Button_" + label, parent);
        var btnImg = btnGO.AddComponent<Image>();
        btnImg.color = BUTTON_COLOR;

        var btn = btnGO.AddComponent<Button>();
        var cb  = btn.colors;
        cb.normalColor      = Color.white;
        cb.highlightedColor = ToColorBlockTint(BUTTON_HL_COLOR, BUTTON_COLOR);
        cb.pressedColor     = ToColorBlockTint(BUTTON_PR_COLOR, BUTTON_COLOR);
        cb.selectedColor    = Color.white;
        cb.fadeDuration     = 0.08f;
        btn.colors          = cb;
        btn.targetGraphic   = btnImg;

        var btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.sizeDelta = new Vector2(BUTTON_W, BUTTON_H);

        // Enforce fixed size in layout
        var le = btnGO.AddComponent<LayoutElement>();
        le.preferredWidth   = BUTTON_W;
        le.preferredHeight  = BUTTON_H;
        le.flexibleWidth    = 0;
        le.flexibleHeight   = 0;

        // Left accent bar (decorative stripe)
        var accentGO  = MakeRect("Accent_Bar", btnGO.transform);
        var accentImg = accentGO.AddComponent<Image>();
        accentImg.color = DIVIDER_COLOR;

        var accentRT = accentGO.GetComponent<RectTransform>();
        accentRT.anchorMin        = new Vector2(0f, 0f);
        accentRT.anchorMax        = new Vector2(0f, 1f);
        accentRT.pivot            = new Vector2(0f, 0.5f);
        accentRT.anchoredPosition = Vector2.zero;
        accentRT.sizeDelta        = new Vector2(4f, 0f);

        // Label
        var textGO  = MakeRect("Text_" + label, btnGO.transform);
        var tmp     = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text             = label.ToUpper();
        tmp.fontSize         = 26f;
        tmp.fontStyle        = FontStyles.Bold;
        tmp.alignment        = TextAlignmentOptions.Center;
        tmp.color            = BTN_TEXT_COLOR;
        tmp.characterSpacing = 4f;

        var textRT = textGO.GetComponent<RectTransform>();
        Stretch(textRT);
    }

    // ──────────────────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────────────────
    static GameObject MakeRect(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.AddComponent<RectTransform>();
        go.transform.SetParent(parent, false);
        return go;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin  = Vector2.zero;
        rt.anchorMax  = Vector2.one;
        rt.offsetMin  = Vector2.zero;
        rt.offsetMax  = Vector2.zero;
    }

    // Compute a ColorBlock tint so the image ends up visually correct
    static Color ToColorBlockTint(Color desired, Color baseColor)
    {
        return new Color(
            Mathf.Clamp01(desired.r / Mathf.Max(baseColor.r, 0.001f)),
            Mathf.Clamp01(desired.g / Mathf.Max(baseColor.g, 0.001f)),
            Mathf.Clamp01(desired.b / Mathf.Max(baseColor.b, 0.001f)),
            1f
        );
    }
}
