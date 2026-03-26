using UnityEngine;

/// <summary>
/// Attach to an NPC/sprite to request dialog for a character+context when interacted with.
/// </summary>
public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private CharacterId characterId;
    [SerializeField] private DialogContext context;
    [SerializeField] private bool oneShot;

    private bool hasFired;

    public void Interact()
    {

        if (oneShot && hasFired) return;

        if (DialogManager.Instance == null)
        {
            Debug.LogWarning("[DialogTrigger] DialogManager.Instance is null.");
            return;
        }

        // Prevent restarting dialog if it's already active
        if (DialogManager.Instance.IsShowing()) return;

        Debug.Log("[DialogTrigger] Interact called on " + gameObject.name);
        DialogManager.Instance.ShowDialog(characterId, context);

        hasFired = true;
    }
}
