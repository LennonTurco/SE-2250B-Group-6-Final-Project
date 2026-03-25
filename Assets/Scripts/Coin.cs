using UnityEngine;

public class Coin : MonoBehaviour
{
    public int pointValue = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger hit by: " + other.name + " Tag: " + other.tag);
        if (other.CompareTag("Player"))
        {
            HUDManager.AddGoldToTotal(pointValue);
            Destroy(gameObject);
        }
    }
}
