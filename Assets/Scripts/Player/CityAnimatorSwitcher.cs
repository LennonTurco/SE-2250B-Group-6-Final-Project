using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class CityAnimationSetSwitcher : MonoBehaviour
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
            animator = GetComponent<Animator>();
        currentController = animator.runtimeAnimatorController;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) SetAnimationController(boyController);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) SetAnimationController(nobleController);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) SetAnimationController(ninjaGreenController);
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) SetAnimationController(ninjaEskimoController);
        else if (Keyboard.current.digit5Key.wasPressedThisFrame) SetAnimationController(flamController);
    }

    // input system callbacks
    public void OnSwitch1(InputAction.CallbackContext context) { if (context.started) SetAnimationController(boyController); }
    public void OnSwitch2(InputAction.CallbackContext context) { if (context.started) SetAnimationController(nobleController); }
    public void OnSwitch3(InputAction.CallbackContext context) { if (context.started) SetAnimationController(ninjaGreenController); }
    public void OnSwitch4(InputAction.CallbackContext context) { if (context.started) SetAnimationController(ninjaEskimoController); }
    public void OnSwitch5(InputAction.CallbackContext context) { if (context.started) SetAnimationController(flamController); }

    private void SetAnimationController(RuntimeAnimatorController controller)
    {
        if (controller == null)
        {
            Debug.LogWarning("CityAnimationSetSwitcher: controller not assigned.");
            return;
        }

        if (animator.runtimeAnimatorController == controller) return;

        // capture current state and params before switching
        int currentStateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float currentStateTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

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
            }
        }

        animator.runtimeAnimatorController = controller;
        currentController = controller;

        // restore params
        foreach (var kvp in paramValues)
        {
            try
            {
                switch (kvp.Value)
                {
                    case bool b: animator.SetBool(kvp.Key, b); break;
                    case float f: animator.SetFloat(kvp.Key, f); break;
                    case int i: animator.SetInteger(kvp.Key, i); break;
                }
            }
            catch (System.ArgumentException) { }
        }

        animator.Play(currentStateHash, 0, currentStateTime);
    }

    public void SelectByIndex(int index)
    {
        switch (index)
        {
            case 1: SetAnimationController(boyController); break;
            case 2: SetAnimationController(nobleController); break;
            case 3: SetAnimationController(ninjaGreenController); break;
            case 4: SetAnimationController(ninjaEskimoController); break;
            case 5: SetAnimationController(flamController); break;
        }
    }
}