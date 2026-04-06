using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class JunglePlayerAnimatorSwitcher : MonoBehaviour
{
    [Header("Animator target")]
    [SerializeField] private Animator animator;

    [Header("Character animator controllers")]
    [Tooltip("1 -> Boy")]
    [SerializeField] private RuntimeAnimatorController boyController;
    [Tooltip("2 -> Arms Dealer")]
    [SerializeField] private RuntimeAnimatorController nobleController;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SetAnimationController(boyController);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SetAnimationController(nobleController);
        }
    }

    public void OnSwitch1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SetAnimationController(boyController);
        }
    }

    public void OnSwitch2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SetAnimationController(nobleController);
        }
    }

    private void SetAnimationController(RuntimeAnimatorController controller)
    {
        if (controller == null)
        {
            Debug.LogWarning("JunglePlayerAnimatorSwitcher: controller is not assigned.");
            return;
        }

        if (animator.runtimeAnimatorController == controller)
        {
            return;
        }

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
                Debug.LogWarning($"Parameter {kvp.Key} not found in new controller, skipping.");
            }
        }

        animator.Play(currentStateHash, 0, currentStateTime);
    }
}
