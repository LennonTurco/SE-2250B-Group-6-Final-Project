using UnityEngine;

/// <summary>
/// Attach to an NPC/sprite to request dialog for a character+context when interacted with.
/// </summary>
public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private CharacterId characterId;
    [SerializeField] private DialogContext context;
    [SerializeField] private bool oneShot;
    [SerializeField] private DialogStrategyRouter router;

    private bool hasFired;

    public void Interact()
    {
        if (oneShot && hasFired) return;

        if (router == null)
        {
            Debug.LogWarning("[DialogTrigger] Missing router reference.");
            return;
        }

        if (DialogManager.Instance == null)
        {
            Debug.LogWarning("[DialogTrigger] DialogManager.Instance is null.");
            return;
        }

        var lines = router.RequestDialog(characterId, context);
        DialogManager.Instance.ShowDialog(lines);

        hasFired = true;
    }
}

