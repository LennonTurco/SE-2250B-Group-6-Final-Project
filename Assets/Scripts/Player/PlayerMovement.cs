using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    protected Player player;
    protected Vector2 moveInput;
    protected bool isAttacking = false;
    
    [Header("Combat Settings")]
    [SerializeField] protected GameObject tossedCoinPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(!isAttacking)
        {
            player.rb.linearVelocity = moveInput * player.moveSpeed;
        }
    }

    public virtual void OnMove(InputAction.CallbackContext context)
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
    public virtual void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started) // when key is pressed down
        {   
            // added this line to block attacks if dialog is open
            if (DialogManager.Instance != null && DialogManager.Instance.IsDisplaying()) return;

            isAttacking = true;
            player.anim.SetBool("isAttacking", true);
            if(moveInput != Vector2.zero)
            {
                player.anim.SetFloat("LastInputX", moveInput.x);
                player.anim.SetFloat("LastInputY", moveInput.y);
            }
            player.rb.linearVelocity = Vector2.zero; // stop movement when attacking
            
            SpawnTossedCoin();
        }
    }

    protected virtual void SpawnTossedCoin()
    {
        if (tossedCoinPrefab == null) return;

        Vector2 spawnDir = moveInput;
        if (spawnDir == Vector2.zero)
        {
            spawnDir = new Vector2(player.anim.GetFloat("LastInputX"), player.anim.GetFloat("LastInputY"));
            if (spawnDir == Vector2.zero) spawnDir = Vector2.down;
        }

        GameObject coinObj = Instantiate(tossedCoinPrefab, player.transform.position, Quaternion.identity);
        TossedCoin coin = coinObj.GetComponent<TossedCoin>();
        if (coin != null)
        {
            coin.dx = spawnDir.x;
            coin.dy = spawnDir.y;
            coin.collisionDamage = player.attackDamage; // use player's actual attack damage (prev didnt add player stats)
        }
    }

    public virtual void EndAttack()
    {
        isAttacking = false;
        player.anim.SetBool("isAttacking", false);
    }

}