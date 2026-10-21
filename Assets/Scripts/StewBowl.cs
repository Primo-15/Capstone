using UnityEngine;

public class StewBowl : MonoBehaviour
{
    Renderer[] renderers;
    bool? lastShow = null;   // null = nothing applied yet, so the first frame always sets the state

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
    }

    void Update()
    {
        bool show = QuestManager.Instance != null && QuestManager.Instance.StewVisible;
        if (lastShow == show) return;

        lastShow = show;
        foreach (var r in renderers) r.enabled = show;
    }
}