using UnityEngine;

public class StewBowl : MonoBehaviour
{
    Renderer[] renderers;
    bool lastShow = true;   // forces the first update to apply

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    void Update()
    {
        bool show = QuestManager.Instance != null && QuestManager.Instance.StewVisible;
        if (show == lastShow) return;

        lastShow = show;
        foreach (var r in renderers) r.enabled = show;
    }
}