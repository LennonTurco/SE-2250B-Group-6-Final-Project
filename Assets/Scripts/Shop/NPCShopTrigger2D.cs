using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class NPCShopTrigger2D : MonoBehaviour
{
    [SerializeField] private string shopSceneName = "Shop";
    [SerializeField] private GameObject promptUI; // optional "Press E" text GO
    // may want to impliment this further later with shop npcs
    
    private bool playerNearby = false;

    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("[NPCShopTrigger2D] Player pressed E");
        }
        if (playerNearby && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // save player pos before leaving
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
                PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
            }

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