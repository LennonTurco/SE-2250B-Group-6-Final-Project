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
    private Player player;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        currentController = animator.runtimeAnimatorController;
        player = GetComponentInParent<Player>();
        if (player == null)
            player = FindFirstObjectByType<Player>();
    }

    private void Update()
    {
        // Optional legacy fallback for keyboard usage in Editor/playmode (removable if not needed)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SetAnimationController(boyController, 1);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame) SetAnimationController(nobleController, 2);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame) SetAnimationController(ninjaGreenController, 3);
            else if (Keyboard.current.digit4Key.wasPressedThisFrame) SetAnimationController(ninjaEskimoController, 4);
            else if (Keyboard.current.digit5Key.wasPressedThisFrame) SetAnimationController(flamController, 5);
        }
    }

    public void OnSwitch1(InputAction.CallbackContext context)
    {
        Debug.Log("Switch1 input received");
        if (context.started) SetAnimationController(boyController, 1);
    }

    public void OnSwitch2(InputAction.CallbackContext context)
    {
        if (context.started) SetAnimationController(nobleController, 2);
    }

    public void OnSwitch3(InputAction.CallbackContext context)
    {
        if (context.started) SetAnimationController(ninjaGreenController, 3);
    }

    public void OnSwitch4(InputAction.CallbackContext context)
    {
        if (context.started) SetAnimationController(ninjaEskimoController, 4);
    }

    public void OnSwitch5(InputAction.CallbackContext context)
    {
        if (context.started) SetAnimationController(flamController, 5);
    }

    private void SetAnimationController(RuntimeAnimatorController controller, int requiredUnlockLevel = 1)
    {
        if (controller == null)
        {
            Debug.LogWarning("AnimationSetSwitcher: controller is not assigned.");
            return;
        }

        if (player != null && player.numCharactersUnlocked < requiredUnlockLevel)
        {
            Debug.Log($"AnimationSetSwitcher: Cannot switch to {controller.name}. Requires unlock level {requiredUnlockLevel} but have {player.numCharactersUnlocked}.");
            return;
        }

        if (animator.runtimeAnimatorController == controller)
        {
            return;
        }

        // Capture current state and parameters before switching
        int currentStateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float currentStateTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        // Store parameter values
        var parameters = animator.parameters;
        var paramValues = new System.Collections.Generic.Dictionary<string, object>();
        foreach (var param in parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    paramValues[param.name] = animator.GetBool(param.name);
                    break;
                case AnimatorControllerParameterType.Float:
                    paramValues[param.name] = animator.GetFloat(param.name);
                    break;
                case AnimatorControllerParameterType.Int:
                    paramValues[param.name] = animator.GetInteger(param.name);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    // Triggers are consumed, so we don't restore them
                    break;
            }
        }

        // Switch the controller
        animator.runtimeAnimatorController = controller;
        currentController = controller;

        // Restore parameters (assuming new controller has same parameters)
        foreach (var kvp in paramValues)
        {
            try
            {
                switch (kvp.Value)
                {
                    case bool b:
                        animator.SetBool(kvp.Key, b);
                        break;
                    case float f:
                        animator.SetFloat(kvp.Key, f);
                        break;
                    case int i:
                        animator.SetInteger(kvp.Key, i);
                        break;
                }
            }
            catch (System.ArgumentException)
            {
                // Parameter doesn't exist in new controller, skip
                Debug.LogWarning($"Parameter {kvp.Key} not found in new controller, skipping.");
            }
        }

        // Restore state (if it exists in the new controller)
        animator.Play(currentStateHash, 0, currentStateTime);

        Debug.Log($"AnimationSetSwitcher: switched to {controller.name} and restored state");
    }

    public void SelectByIndex(int index)
    {
        switch (index)
        {
            case 1:
                SetAnimationController(boyController, 1);
                break;
            case 2:
                SetAnimationController(nobleController, 2);
                break;
            case 3:
                SetAnimationController(ninjaGreenController, 3);
                break;
            case 4:
                SetAnimationController(ninjaEskimoController, 4);
                break;
            case 5:
                SetAnimationController(flamController, 5);
                break;
            default:
                Debug.LogWarning("AnimationSetSwitcher: invalid index " + index);
                break;
        }
    }
}
