using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string id = "HouseExit";

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}