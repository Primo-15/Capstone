using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplePickup : MonoBehaviour
{
    public float pickupDistance = 2f;

    [Header("Prompt")]
    public GameObject promptUI;
    public float sideOffset = 0.6f;    // how far left of the apple the label floats
    public float heightOffset = 0.1f;  // nudge up/down if needed

    Transform player;
    Camera cam;
    string pickupId;

    void Start()
    {
        // Unique ID from scene + starting position, stable across scene reloads
        Vector3 p0 = transform.position;
        pickupId = $"{gameObject.scene.name}_{p0.x:F2}_{p0.y:F2}_{p0.z:F2}";

        // Already picked up earlier: remove it right away
        if (QuestManager.Instance != null && QuestManager.Instance.IsPickupCollected(pickupId))
        {
            if (promptUI) promptUI.SetActive(false);
            enabled = false;
            Destroy(gameObject);
            return;
        }

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
            QuestManager.Instance.MarkPickupCollected(pickupId);

            if (promptUI) promptUI.SetActive(false);
            Destroy(gameObject);
        }
    }
}