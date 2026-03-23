using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class AnimationSetSwitcher : MonoBehaviour
{
    [Header("Animator target")]
    [SerializeField] private Animator animator;

    [Header("Character animator controllers")]
    [Tooltip("1 -> Boy")]
    [SerializeField] private RuntimeAnimatorController boyController;
    [Tooltip("2 -> Noble")]
    [SerializeField] private RuntimeAnimatorController nobleController;
    [Tooltip("3 -> NinjaGreen")]
    [SerializeField] private RuntimeAnimatorController ninjaGreenController;
    [Tooltip("4 -> NinjaEskimo")]
    [SerializeField] private RuntimeAnimatorController ninjaEskimoController;
    [Tooltip("5 -> Flam")]
    [SerializeField] private RuntimeAnimatorController flamController;

    private RuntimeAnimatorController currentController;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        currentController = animator.runtimeAnimatorController;
    }

    private void Update()
    {
        // Optional legacy fallback for keyboard usage in Editor/playmode (removable if not needed)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SetAnimationController(boyController);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame) SetAnimationController(nobleController);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame) SetAnimationController(ninjaGreenController);
            else if (Keyboard.current.digit4Key.wasPressedThisFrame) SetAnimationController(ninjaEskimoController);
            else if (Keyboard.current.digit5Key.wasPressedThisFrame) SetAnimationController(flamController);
        }
    }

    public void OnSwitch1(InputAction.CallbackContext context)
    {
        Debug.Log("Switch1 input received");
        if (context.started) SetAnimationController(boyController);
    }

    public void OnSwitch2(InputAction.CallbackContext context)
    {
        if (context.started) SetAnimationController(nobleController);
    }

    public void OnSwitch3(InputAction.CallbackContext context)
    {
        if (context.started) SetAnimationController(ninjaGreenController);
    }

    public void OnSwitch4(InputAction.CallbackContext context)
    {
        if (context.started) SetAnimationController(ninjaEskimoController);
    }

    public void OnSwitch5(InputAction.CallbackContext context)
    {
        if (context.started) SetAnimationController(flamController);
    }

    private void SetAnimationController(RuntimeAnimatorController controller)
    {
        if (controller == null)
        {
            Debug.LogWarning("AnimationSetSwitcher: controller is not assigned.");
            return;
        }

        if (animator.runtimeAnimatorController == controller)
        {
            return;
        }

        animator.runtimeAnimatorController = controller;
        currentController = controller;
        Debug.Log($"AnimationSetSwitcher: switched to {controller.name}");
    }

    public void SelectByIndex(int index)
    {
        switch (index)
        {
            case 1:
                SetAnimationController(boyController);
                break;
            case 2:
                SetAnimationController(nobleController);
                break;
            case 3:
                SetAnimationController(ninjaGreenController);
                break;
            case 4:
                SetAnimationController(ninjaEskimoController);
                break;
            case 5:
                SetAnimationController(flamController);
                break;
            default:
                Debug.LogWarning("AnimationSetSwitcher: invalid index " + index);
                break;
        }
    }
}
