using UnityEngine;

public abstract class Character : ScriptableObject
{
    public string characterName;

    public abstract void Attack();
    public abstract void ChargedAttack();
}

