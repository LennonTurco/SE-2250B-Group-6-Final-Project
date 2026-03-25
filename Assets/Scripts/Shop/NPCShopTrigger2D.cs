using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCShopTrigger2D : MonoBehaviour
{
    [SerializeField] private string shopSceneName = "Shop";
    [SerializeField] private GameObject promptUI; // optional "Press E" text GO
    // may want to impliment this further later with shop npcs
    
    private bool playerNearby = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(shopSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        if (promptUI != null) promptUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        if (promptUI != null) promptUI.SetActive(false);
    }
}