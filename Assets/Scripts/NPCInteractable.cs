using UnityEngine;
using TMPro;

public class NPCInteractable : MonoBehaviour
{
    // Other scripts (player movement, camera, NPC2) can check or set this
    public static bool InDialogue { get; set; }

    [Header("Interaction")]
    public float interactDistance = 2f;

    [Header("Prompt (shown when in range)")]
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press \"E\" to talk to manang";

    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Before talking to mom")]
    [TextArea(2, 4)]
    public string[] notReadyLines =
    {
        "Isay, I'm busy right now.",
        "Why don't you go talk to your mom first?"
    };

    [Header("Apple quest")]
    [TextArea(2, 4)]
    public string[] dialogueLines =
    {
        "Oh thank you Isay",
        "Can you help me with something?",
        "I need you to gather 5 <b><color=red>apples</color></b> for our visitors later"
    };

    [Header("Apple quest, not finished yet")]
    [TextArea(2, 4)]
    public string[] reminderLines =
    {
        "Please gather me 5 <b><color=red>apples</color></b>, Isay."
    };

    [Header("After collecting all apples")]
    [TextArea(2, 4)]
    public string[] completeLines =
    {
        "Thanks Isay!",
        "You have such a good heart",
        "By the way here's a sinigang as a thanks",
        "You can bring the sinigang to your mom"
    };

    [Header("After the quest is done")]
    [TextArea(2, 4)]
    public string[] afterLines =
    {
        "Bring the sinigang to your mom."
    };

    Transform player;
    string[] currentLines;
    int lineIndex = -1;
    bool IsTalking => lineIndex >= 0;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        if (promptUI) promptUI.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);
        InDialogue = false;
    }

    void OnDisable()
    {
        if (IsTalking) InDialogue = false;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        bool inRange = dist <= interactDistance;

        if (!inRange && IsTalking) EndDialogue(false);

        if (promptUI) promptUI.SetActive(inRange && !IsTalking);
        if (inRange && promptText) promptText.text = promptMessage;

        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!IsTalking) StartDialogue();
            else NextLine();
        }
    }

    void StartDialogue()
    {
        var qm = QuestManager.Instance;

        if (qm == null) currentLines = dialogueLines;
        else if (!qm.MomTalked) currentLines = notReadyLines;   // haven't seen mom yet
        else if (qm.ApplesReady) currentLines = completeLines;   // hand in the apples
        else if (qm.IsFinished) currentLines = afterLines;      // quest already done
        else if (qm.IsActive) currentLines = reminderLines;   // still collecting apples
        else currentLines = dialogueLines;   // first apple-quest talk

        // Hide the quest UI as soon as the turn-in conversation begins
        if (qm != null && currentLines == completeLines)
            qm.CompleteQuest();

        lineIndex = 0;
        InDialogue = true;
        if (dialoguePanel) dialoguePanel.SetActive(true);
        ShowLine();
    }

    void NextLine()
    {
        lineIndex++;
        if (lineIndex >= currentLines.Length) EndDialogue(true);
        else ShowLine();
    }

    void ShowLine()
    {
        if (dialogueText) dialogueText.text = currentLines[lineIndex];
    }

    void EndDialogue(bool finishedAllLines)
    {
        lineIndex = -1;
        InDialogue = false;
        if (dialoguePanel) dialoguePanel.SetActive(false);

        if (!finishedAllLines || QuestManager.Instance == null) return;

        // First apple-quest conversation: start the quest
        if (currentLines == dialogueLines)
            QuestManager.Instance.StartQuest();

        // Turn-in conversation finished: show "Go back to mom"
        else if (currentLines == completeLines)
            QuestManager.Instance.ShowReturnObjective();
    }
}