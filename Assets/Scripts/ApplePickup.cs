using UnityEngine;

public class ApplePickup : MonoBehaviour
{
    public float pickupDistance = 2f;

    [Header("Prompt")]
    public GameObject promptUI;
    public float sideOffset = 0.6f;    // how far left of the apple the label floats
    public float heightOffset = 0.1f;  // nudge up/down if needed

    Transform player;
    Camera cam;

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        cam = Camera.main;

        if (promptUI) promptUI.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        bool questActive = QuestManager.Instance != null && QuestManager.Instance.IsActive;
        bool inRange = Vector3.Distance(player.position, transform.position) <= pickupDistance;
        bool canPickUp = questActive && inRange;

        if (promptUI)
        {
            promptUI.SetActive(canPickUp);

            if (canPickUp && cam != null)
            {
                // Left of the apple as seen from the camera, facing the camera
                Vector3 pos = transform.position - cam.transform.right * sideOffset;
                pos.y += heightOffset;
                promptUI.transform.position = pos;
                promptUI.transform.rotation =
                    Quaternion.LookRotation(promptUI.transform.position - cam.transform.position);
            }
        }

        if (canPickUp && Input.GetKeyDown(KeyCode.E))
        {
            QuestManager.Instance.AddApple();
            Destroy(gameObject);
        }
    }
}