using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SceneInteractable : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Inside house";
    [SerializeField] private string spawnId;
    [SerializeField] private float interactDistance = 1.5f;
    [SerializeField] private GameObject promptUI;

    private Transform player;

    private void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        else Debug.LogWarning("No object tagged 'Player' found.");

        if (promptUI != null) promptUI.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        // Ignore height difference so only horizontal distance counts
        Vector3 a = player.position;
        Vector3 b = transform.position;
        a.y = 0f;
        b.y = 0f;
        bool inRange = Vector3.Distance(a, b) <= interactDistance;

        if (promptUI != null && promptUI.activeSelf != inRange)
            promptUI.SetActive(inRange);

        if (inRange && InteractPressed())
        {
            SpawnData.nextSpawnId = spawnId;
            Debug.Log($"[Interact] '{name}' in scene '{gameObject.scene.name}' sending spawnId = '{spawnId}', loading '{sceneToLoad}'");
            SceneManager.LoadScene(sceneToLoad);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}