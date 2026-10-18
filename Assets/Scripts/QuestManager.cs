using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

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

    public bool MomTalked => momTalked;
    public bool IsActive => questActive;
    public bool ApplesReady => questActive && !questFinished && applesCollected >= applesNeeded;
    public bool IsFinished => questFinished;
    public bool ReturnToMom => returnToMom;

    void Awake()
    {
        Instance = this;
        if (questCompleteObject) questCompleteObject.SetActive(false);

        if (questPanel) questPanel.SetActive(true);
        UpdateUI();
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
    }

    // Called by Manang (NPC1) when the first conversation ends
    public void StartQuest()
    {
        if (questActive) return;
        questActive = true;
        if (questPanel) questPanel.SetActive(true);
        UpdateUI();
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