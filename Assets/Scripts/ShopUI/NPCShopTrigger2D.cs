using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCShopTrigger2D : MonoBehaviour
{
    public string shopSceneName = "Shop";
    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby)
        {
            PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}