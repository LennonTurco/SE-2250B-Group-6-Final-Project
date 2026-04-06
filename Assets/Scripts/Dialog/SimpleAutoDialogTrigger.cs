using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SimpleAutoDialogTrigger : MonoBehaviour
{
    [TextArea]
    [SerializeField] private string line = "LOL! I love wasting your time! The real challenge is southeast of here!";
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool marksJungleSkeletonHintSeen = false;

    private bool hasTriggered;

    private void Reset()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnce && hasTriggered)
            return;

        if (DialogManager.Instance == null)
            return;

        hasTriggered = true;
        DialogManager.Instance.ShowDialog(new List<string> { line });

        if (marksJungleSkeletonHintSeen)
        {
            JungleProgressState.MarkSkeletonHintSeen();
            HUDManager.Instance?.RefreshSceneObjective();
        }
    }
}
