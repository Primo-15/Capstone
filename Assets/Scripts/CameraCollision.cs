using UnityEngine;

// Runs before the Movement script so it always sees the un-pushed camera position
[DefaultExecutionOrder(-100)]
public class CameraCollision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;          // leave empty to auto-find the Player tag

    [Header("Settings")]
    [SerializeField] private float pivotHeight = 1.5f;  // point on the player the camera looks back from
    [SerializeField] private float cameraRadius = 0.2f; // thickness of the collision check
    [SerializeField] private float padding = 0.1f;      // gap kept between camera and wall
    [SerializeField] private float minDistance = 0.3f;  // never closer to the pivot than this
    [SerializeField] private float returnSpeed = 6f;    // how fast the camera eases back out
    [SerializeField] private LayerMask collisionMask = ~0;

    private Vector3 desiredLocal;
    private float currentDistance = float.MaxValue;

    private void Start()
    {
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
            else Debug.LogWarning("[CameraCollision] No object tagged 'Player' found.");
        }

        desiredLocal = transform.localPosition;
    }

    private void Update()
    {
        // Undo last frame's collision push so other scripts start from the clean position
        transform.localPosition = desiredLocal;
    }

    private void LateUpdate()
    {
        if (target == null || transform.parent == null) return;

        // Whatever the camera's local position is now (after other scripts ran) is the desired spot
        desiredLocal = transform.localPosition;

        Vector3 pivot = target.position + Vector3.up * pivotHeight;
        Vector3 desiredWorld = transform.parent.TransformPoint(desiredLocal);

        Vector3 toCamera = desiredWorld - pivot;
        float fullDistance = toCamera.magnitude;
        if (fullDistance < 0.01f) return;
        Vector3 dir = toCamera / fullDistance;

        // Nearest wall between the pivot and the camera, ignoring the player itself
        float allowed = fullDistance;
        RaycastHit[] hits = Physics.SphereCastAll(pivot, cameraRadius, dir, fullDistance,
                                                  collisionMask, QueryTriggerInteraction.Ignore);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.IsChildOf(target) || hit.transform == target) continue;
            if (hit.distance <= 0f) continue; // started inside a collider, ignore
            float d = Mathf.Max(minDistance, hit.distance - padding);
            if (d < allowed) allowed = d;
        }

        // Snap in instantly when blocked, ease back out when clear
        if (allowed < currentDistance) currentDistance = allowed;
        else currentDistance = Mathf.Lerp(currentDistance, allowed, returnSpeed * Time.deltaTime);
        currentDistance = Mathf.Min(currentDistance, fullDistance);

        transform.position = pivot + dir * currentDistance;
    }
}