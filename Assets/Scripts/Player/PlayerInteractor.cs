using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attach this to the Player GameObject to handle interactions with the world.
/// Ensure your Player has a PlayerInput component configured to call 'OnInteract'.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    private DialogTrigger currentTarget;

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Trigger interaction only if we have a registered target in range
        if (context.started && currentTarget != null)
        {
            Debug.Log("Interact input received - triggering dialog");
            currentTarget.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Automatically register the nearest DialogTrigger when entering its zone
        if (other.TryGetComponent(out DialogTrigger trigger))
        {
            currentTarget = trigger;
            Debug.Log($"In range of {other.name} (DialogTrigger registered)");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Unregister only if the object leaving is our active target
        if (other.TryGetComponent(out DialogTrigger trigger) && currentTarget == trigger)
        {
            currentTarget = null;
            Debug.Log($"Left range of {other.name} (DialogTrigger unregistered)");
        }
    }
}
