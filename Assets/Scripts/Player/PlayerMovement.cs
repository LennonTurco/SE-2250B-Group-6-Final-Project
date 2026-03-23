using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    private Player player;
    private Vector2 moveInput;
    private bool isAttacking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAttacking)
        {
            player.rb.linearVelocity = moveInput * player.moveSpeed;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        player.anim.SetBool("isWalking", true);

        if (context.canceled)
        {
            player.anim.SetBool("isWalking", false);
            player.anim.SetFloat("LastInputX", moveInput.x);
            player.anim.SetFloat("LastInputY", moveInput.y);
        }

        moveInput = context.ReadValue<Vector2>();

        player.anim.SetFloat("InputX", moveInput.x);
        player.anim.SetFloat("InputY", moveInput.y);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack input received");
        if (context.started) // when key is pressed down
        {
            isAttacking = true;
            player.anim.SetBool("isAttacking", true);
            if(moveInput != Vector2.zero)
            {
                player.anim.SetFloat("LastInputX", moveInput.x);
                player.anim.SetFloat("LastInputY", moveInput.y);
            }
            player.rb.linearVelocity = Vector2.zero; // stop movement when attacking
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        player.anim.SetBool("isAttacking", false);
    }

}
