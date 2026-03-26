using UnityEngine;
using UnityEngine.InputSystem;

public class TestDialogInteractor : MonoBehaviour
{
    [SerializeField] private DialogTrigger target;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame && target != null)
        {
            target.Interact();
        }
    }
}

