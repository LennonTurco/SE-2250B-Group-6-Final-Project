using UnityEngine;

public class TestDialogInteractor : MonoBehaviour
{
    [SerializeField] private DialogTrigger target;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && target != null)
        {
            target.Interact();
        }
    }
}

