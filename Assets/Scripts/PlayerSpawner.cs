using System.Collections;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private IEnumerator Start()
    {
        Debug.Log($"[PlayerSpawner] nextSpawnId = '{SpawnData.nextSpawnId}'");

        if (string.IsNullOrEmpty(SpawnData.nextSpawnId)) yield break;

        string id = SpawnData.nextSpawnId;
        SpawnData.nextSpawnId = null;

        SpawnPoint target = null;
        foreach (SpawnPoint sp in FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None))
        {
            if (sp.id == id) { target = sp; break; }
        }

        if (target == null)
        {
            Debug.LogWarning($"[PlayerSpawner] No SpawnPoint with id '{id}' in this scene.");
            yield break;
        }

        // Wait one frame so other scripts' Start() can't override the move
        yield return null;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.SetPositionAndRotation(target.transform.position, target.transform.rotation);

        if (cc != null) cc.enabled = true;
        Physics.SyncTransforms();

        Debug.Log($"[PlayerSpawner] Moved player to {target.transform.position}");
    }
}