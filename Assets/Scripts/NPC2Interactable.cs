using UnityEngine;
using TMPro;

public class NPC2Interactable : MonoBehaviour
{
    [Header("Interaction")]
    public float interactDistance = 2f;

    [Header("Prompt (shown when in range)")]
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press \"E\" to talk to mom";

    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("First conversation")]
    [TextArea(2, 4)]
    public string[] dialogueLines =
    {
        "Good afternoon Isay",
        "Can you give this ulam to Manang?"
    };

    [Header("While Isay is away")]
    [TextArea(2, 4)]
    public string[] repeatLines =
    {
        "Please give that ulam to Manang, Isay."
    };

    [Header("After returning from Manang")]
    [TextArea(2, 4)]
    public string[] returnLines =
    {
        "You got an ulam from Manang?",
        "Oh good, we can eat!",
        "Your father is waiting"
    };

    [Header("After the final conversation")]
    [TextArea(2, 4)]
    public string[] afterLines =
    {
        "Hurry, your father is waiting."
    };

    [Header("Quest")]
    public string nextObjective = "Go to Manang";

    Transform player;
    string[] currentLines;
    int lineIndex = -1;
    bool gaveUlam = false;
    bool finalDone = false;
    bool IsTalking => lineIndex >= 0;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        if (promptUI) promptUI.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);
    }

    void OnDisable()
    {
        if (IsTalking) NPCInteractable.InDialogue = false;
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

        if (finalDone) currentLines = afterLines;
        else if (qm != null && qm.ReturnToMom) currentLines = returnLines;
        else if (gaveUlam) currentLines = repeatLines;
        else currentLines = dialogueLines;

        lineIndex = 0;
        NPCInteractable.InDialogue = true;
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
        NPCInteractable.InDialogue = false;
        if (dialoguePanel) dialoguePanel.SetActive(false);

        if (!finishedAllLines || QuestManager.Instance == null) return;

        // First conversation: unlock Manang and update the objective
        if (currentLines == dialogueLines && !gaveUlam)
        {
            gaveUlam = true;
            QuestManager.Instance.FinishMomTalk(nextObjective);
        }

        // Final conversation: the questline is done
        else if (currentLines == returnLines && !finalDone)
        {
            finalDone = true;
            QuestManager.Instance.FinishQuestline();
        }
    }
}