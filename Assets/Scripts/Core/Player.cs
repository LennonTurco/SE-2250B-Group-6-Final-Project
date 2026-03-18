using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int gold;

    public int Gold => gold;

    public bool IsDead()
    {
        return false;
    }

    public void Respawn()
    {
        // placeholder
    }

    public void OnCharacterUnlocked(Character character)
    {
        // placeholder
    }

    public void SpendGold(int amount)
    {
        gold = Mathf.Max(0, gold - amount);
    }
}

