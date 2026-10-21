using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UI")]
    public GameObject questPanel;               // QuestText
    public TextMeshProUGUI questText;
    public GameObject questCompleteObject;      // QuestCompleteText

    [Header("Quest")]
    public string startingObjective = "Talk to mom";
    public string returnObjective = "Go back to mom";
    public int applesNeeded = 5;

    int applesCollected = 0;
    bool momTalked = false;        // true after the player talked to mom (NPC2)
    bool questActive = false;      // true once Manang has given the apple quest
    bool questFinished = false;    // true once the apples were handed in
    bool returnToMom = false;      // true after Manang's final dialogue
    bool questlineDone = false;    // true after mom's final dialogue
    bool stewDelivered = false;    // NEW (stew): true once mom has received the sinigang

    public bool MomTalked => momTalked;
    public bool QuestlineDone => questlineDone;
    public bool IsActive => questActive;
    public bool ApplesReady => questActive && !questFinished && applesCollected >= applesNeeded;
    public bool IsFinished => questFinished;
    public bool ReturnToMom => returnToMom;

    // NEW (stew): the bowl shows after mom's first talk until Manang's quest begins,
    // and again after Manang hands over the sinigang until mom receives it
    public bool StewVisible =>
        (momTalked && !questActive) ||
        (returnToMom && !stewDelivered);

    // StewBowl (and anything else) can listen for state changes
    public event System.Action OnStateChanged;

    // Remembers collected pickups so they stay gone when a scene reloads
    readonly HashSet<string> collectedPickups = new HashSet<string>();
    public bool IsPickupCollected(string id) => collectedPickups.Contains(id);
    public void MarkPickupCollected(string id) => collectedPickups.Add(id);

    void Awake()
    {
        // A QuestManager already exists (e.g. coming back to GameWorld): destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (questCompleteObject) questCompleteObject.SetActive(false);
        if (questPanel) questPanel.SetActive(true);
        UpdateUI();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Main menu: remove the manager (and its UI) so a new game starts fresh
        if (scene.name == mainMenuSceneName)
            Destroy(gameObject);
    }

    // Called by mom (NPC2) when her first conversation ends
    public void FinishMomTalk(string nextObjective)
    {
        if (momTalked) return;
        momTalked = true;
        startingObjective = nextObjective;   // e.g. "Go to Manang"
        if (!questActive && !questFinished)
        {
            if (questPanel) questPanel.SetActive(true);
            UpdateUI();
        }
        OnStateChanged?.Invoke();            // stew appears
    }

    // Called by Manang (NPC1) when the first conversation ends
    public void StartQuest()
    {
        if (questActive) return;
        questActive = true;
        if (questPanel) questPanel.SetActive(true);
        UpdateUI();
        OnStateChanged?.Invoke();            // stew disappears
    }

    public void AddApple()
    {
        if (!questActive || questFinished) return;

        applesCollected = Mathf.Min(applesCollected + 1, applesNeeded);
        UpdateUI();

        if (applesCollected >= applesNeeded && questCompleteObject)
            questCompleteObject.SetActive(true);
    }

    // Called when the turn-in conversation with Manang begins: hide the quest UI
    public void CompleteQuest()
    {
        if (questFinished) return;
        questFinished = true;
        if (questPanel) questPanel.SetActive(false);
        if (questCompleteObject) questCompleteObject.SetActive(false);
    }

    // Called when the turn-in conversation ends: bring the UI back with the new objective
    public void ShowReturnObjective()
    {
        if (returnToMom) return;
        returnToMom = true;
        if (questPanel) questPanel.SetActive(true);
        UpdateUI();
        OnStateChanged?.Invoke();            // NEW (stew): bowl reappears for the trip back to mom
    }

    // NEW (stew): called when the conversation with mom about the sinigang begins
    public void DeliverStew()
    {
        if (stewDelivered) return;
        stewDelivered = true;
        OnStateChanged?.Invoke();            // bowl disappears
    }

    // Called when mom's final conversation ends: this questline is done
    public void FinishQuestline()
    {
        if (questlineDone) return;
        questlineDone = true;
        if (questPanel) questPanel.SetActive(false);
    }

    void UpdateUI()
    {
        if (!questText) return;

        if (returnToMom)
        {
            questText.text = $"Quest:\n{returnObjective}";
            return;
        }

        if (!questActive)
        {
            questText.text = $"Quest:\n{startingObjective}";
            return;
        }

        string line = $"Collect apples {applesCollected}/{applesNeeded}";

        if (applesCollected >= applesNeeded)
            line = $"<s><color=#777777>{line}</color></s>";

        questText.text = $"Quest:\n{line}";
    }
}