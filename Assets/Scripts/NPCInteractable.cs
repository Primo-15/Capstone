using UnityEngine;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class NPCInteractable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactDistance = 1f;

    [Header("Prompt (shown when in range)")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private string promptMessage = "talk to Manang";

    [Header("Dialogue (shown after pressing E)")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private string dialogueMessage = "greetings Isay";

    private Transform player;
    private bool dialogueOpen;

    private void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("[NPCInteractable] No object tagged 'Player' found.");

        if (promptText != null) promptText.text = promptMessage;
        if (dialogueText != null) dialogueText.text = dialogueMessage;

        if (promptUI != null) promptUI.SetActive(false);
        if (dialogueUI != null) dialogueUI.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        // Horizontal distance only (ignore height difference)
        Vector3 a = player.position;
        Vector3 b = transform.position;
        a.y = 0f;
        b.y = 0f;
        bool inRange = Vector3.Distance(a, b) <= interactDistance;

        // Prompt is visible only when in range and the dialogue is not open
        if (promptUI != null)
        {
            bool showPrompt = inRange && !dialogueOpen;
            if (promptUI.activeSelf != showPrompt) promptUI.SetActive(showPrompt);
        }

        if (inRange && InteractPressed())
        {
            dialogueOpen = !dialogueOpen;
            if (dialogueUI != null) dialogueUI.SetActive(dialogueOpen);
        }

        // Close the dialogue automatically if the player walks away
        if (!inRange && dialogueOpen)
        {
            dialogueOpen = false;
            if (dialogueUI != null) dialogueUI.SetActive(false);
        }
    }

    private bool InteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
