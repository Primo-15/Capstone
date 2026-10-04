using UnityEngine;
using UnityEngine.UIElements;

public class HirayaMainMenu : MonoBehaviour
{
    private UIDocument uiDocument;

    private Button startGameButton;
    private Button continueButton;
    private Button settingsButton;
    private Button aboutButton;
    private Button quitButton;

    private Button stormToggle;
    private Button displayToggle;

    private VisualElement settingsDrawer;
    private Label saveNote;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();

        if (uiDocument == null)
        {
            Debug.LogError("HirayaMainMenu requires a UIDocument component.");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;

        startGameButton = root.Q<Button>("start-game-button");
        continueButton = root.Q<Button>("continue-button");
        settingsButton = root.Q<Button>("settings-button");
        aboutButton = root.Q<Button>("about-button");
        quitButton = root.Q<Button>("quit-button");

        stormToggle = root.Q<Button>("storm-toggle");
        displayToggle = root.Q<Button>("display-toggle");

        settingsDrawer = root.Q<VisualElement>("settings-drawer");
        saveNote = root.Q<Label>("save-note");

        startGameButton.clicked += OnStartGameClicked;
        continueButton.clicked += OnContinueClicked;
        settingsButton.clicked += OnSettingsClicked;
        aboutButton.clicked += OnAboutClicked;
        quitButton.clicked += OnQuitClicked;

        stormToggle.clicked += OnStormToggleClicked;
        displayToggle.clicked += OnDisplayToggleClicked;
    }

    private void OnDisable()
    {
        if (startGameButton != null) startGameButton.clicked -= OnStartGameClicked;
        if (continueButton != null) continueButton.clicked -= OnContinueClicked;
        if (settingsButton != null) settingsButton.clicked -= OnSettingsClicked;
        if (aboutButton != null) aboutButton.clicked -= OnAboutClicked;
        if (quitButton != null) quitButton.clicked -= OnQuitClicked;

        if (stormToggle != null) stormToggle.clicked -= OnStormToggleClicked;
        if (displayToggle != null) displayToggle.clicked -= OnDisplayToggleClicked;
    }

    private void OnStartGameClicked()
    {
        Choose("Your journey begins…");

        // Replace this with your actual scene loading code, for example:
        // UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        Debug.Log("Start Game clicked.");
    }

    private void OnContinueClicked()
    {
        Choose("Returning to Aldren…");

        // Replace this with your save/load system.
        Debug.Log("Continue clicked.");
    }

    private void OnSettingsClicked()
    {
        bool isOpen = settingsDrawer.style.display == DisplayStyle.Flex;
        settingsDrawer.style.display = isOpen
            ? DisplayStyle.None
            : DisplayStyle.Flex;

        // Match the React behavior: the status message is hidden while settings are open.
        saveNote.style.display = isOpen
            ? DisplayStyle.Flex
            : DisplayStyle.None;
    }

    private void OnAboutClicked()
    {
        Choose("Hiraya · A tale of hope beneath gathering skies.");
        Debug.Log("About clicked.");
    }

    private void OnQuitClicked()
    {
        Choose("May fair weather find you, traveler.");
        Debug.Log("Quit clicked.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnStormToggleClicked()
    {
        Choose("Storm ambience adjusted.");
        Debug.Log("Storm ambience set to Gentle.");
    }

    private void OnDisplayToggleClicked()
    {
        Choose("Display set to borderless.");
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    private void Choose(string nextMessage)
    {
        settingsDrawer.style.display = DisplayStyle.None;
        saveNote.style.display = DisplayStyle.Flex;
        saveNote.text = nextMessage;
    }
}
