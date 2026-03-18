using UnityEngine;

abstract class Enemy : MonoBehaviour
{
    private float attackInterval;

    public abstract void onAttack();

    public abstract int dropGold();
    void  Update()
    {
        
    }


}