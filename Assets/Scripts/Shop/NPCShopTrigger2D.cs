using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class NPCShopTrigger2D : MonoBehaviour
{
    [SerializeField] private string shopSceneName = "Shop";
    [SerializeField] private GameObject promptUI; // optional "Press E" text GO
    // may want to impliment this further later with shop npcs
    
    private bool playerNearby = false;

    private System.Collections.Generic.Dictionary<Renderer, bool> rendererStates = new System.Collections.Generic.Dictionary<Renderer, bool>();
    private System.Collections.Generic.Dictionary<Canvas, bool> canvasStates = new System.Collections.Generic.Dictionary<Canvas, bool>();
    
    private Player shopPlayer;
    private bool originalPlayerInvulState;

    void Update()
    {
        if (playerNearby && Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            EnterShopAdditive();
        }
    }

    private void EnterShopAdditive()
    {
        rendererStates.Clear();
        canvasStates.Clear();

        // Loop through every root object in the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        foreach (GameObject go in currentScene.GetRootGameObjects())
        {
            // Hide all Renderers (Sprites, Meshes, etc.)
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                rendererStates[r] = r.enabled;
                r.enabled = false;
            }

            // Hide all Canvases (UI)
            Canvas[] canvases = go.GetComponentsInChildren<Canvas>(true);
            foreach (Canvas c in canvases)
            {
                canvasStates[c] = c.enabled;
                c.enabled = false;
            }
        }

        // Make Player Invulnerable
        shopPlayer = FindFirstObjectByType<Player>();
        if (shopPlayer != null)
        {
            originalPlayerInvulState = shopPlayer.isInvul;
            shopPlayer.isInvul = true;
        }

        // Load shop additively
        SceneManager.LoadScene(shopSceneName, LoadSceneMode.Additive);

        // Subscribe to know when the shop closes
        SceneManager.sceneUnloaded += OnShopUnloaded;
    }

    private void OnShopUnloaded(Scene scene)
    {
        if (scene.name == shopSceneName)
        {
            // Re-enable Renderers based on their original state
            foreach (var kvp in rendererStates)
            {
                if (kvp.Key != null) kvp.Key.enabled = kvp.Value;
            }
            rendererStates.Clear();

            // Re-enable Canvases based on their original state
            foreach (var kvp in canvasStates)
            {
                if (kvp.Key != null) kvp.Key.enabled = kvp.Value;
            }
            canvasStates.Clear();

            // Restore Player Invulnerability State and Apply Pending Stats
            if (shopPlayer != null)
            {
                int count = PlayerPrefs.GetInt("PendingCount", 0);
                for (int i = 0; i < count; i++)
                {
                    string stat = PlayerPrefs.GetString("PendingStat_" + i);
                    float amount = PlayerPrefs.GetFloat("PendingAmount_" + i);
                    shopPlayer.ApplyStat(stat, amount);
                    PlayerPrefs.DeleteKey("PendingStat_" + i);
                    PlayerPrefs.DeleteKey("PendingAmount_" + i);
                }
                PlayerPrefs.DeleteKey("PendingCount");

                HUDManager.Instance?.RefreshHUD();

                shopPlayer.isInvul = originalPlayerInvulState;
                shopPlayer = null;
            }

            // Unsubscribe to prevent memory leaks
            SceneManager.sceneUnloaded -= OnShopUnloaded;
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