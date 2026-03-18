using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAttacking)
        {
        rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        animator.SetBool("isWalking", true);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack input received");
        if (context.started) // when key is pressed down
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            if(moveInput != Vector2.zero)
            {
                animator.SetFloat("LastInputX", moveInput.x);
                animator.SetFloat("LastInputY", moveInput.y);
            }
            rb.linearVelocity = Vector2.zero; // stop movement when attacking
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

}
