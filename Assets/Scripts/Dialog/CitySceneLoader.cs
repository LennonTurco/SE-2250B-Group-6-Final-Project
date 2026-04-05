using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject solomonPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform playerRespawnPoint;

    [SerializeField] private List<string> solomonSpawnDialog = new List<string>
    {
        "So. You found the code.",
        "Impressive. But it ends here.",
        "Come. Let's finish this."
    };

    private void Awake()
    {
        if (GameState.PuzzleSolved)
        {
            GameState.PuzzleSolved = false;
            Debug.Log("[CitySceneLoader] PuzzleSolved flag found - starting sequence.");
            StartCoroutine(SolomonSequence());
        }
        else
        {
            Debug.Log("[CitySceneLoader] PuzzleSolved flag not found - skipping sequence.");
        }
    }

    private IEnumerator SolomonSequence()
    {
        yield return null;
        yield return null;
        yield return null;

        Debug.Log("[CitySceneLoader] Running solomon sequence.");

        // move player to respawn point near gate
        if (playerRespawnPoint != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = playerRespawnPoint.position;
            else
                Debug.LogWarning("[CitySceneLoader] Player not found.");
        }

        // open the gate
        if (PuzzleManager.Instance != null)
        {
            Debug.Log("[CitySceneLoader] Calling TrySolve.");
            PuzzleManager.Instance.TrySolve();
        }
        else Debug.LogWarning("[CitySceneLoader] PuzzleManager not found.");

        yield return new WaitForSeconds(1f);

        // spawn solomon
        if (solomonPrefab != null && spawnPoint != null)
        {
            Debug.Log("[CitySceneLoader] Spawning Solomon.");
            Instantiate(solomonPrefab, spawnPoint.position, Quaternion.identity);
        }
        else Debug.LogWarning("[CitySceneLoader] Solomon prefab or spawn point not assigned.");

        yield return new WaitForSeconds(0.5f);

        // play entrance dialog
        if (DialogManager.Instance != null)
        {
            Debug.Log("[CitySceneLoader] Playing dialog.");
            DialogManager.Instance.ShowDialog(solomonSpawnDialog);
        }
        else Debug.LogWarning("[CitySceneLoader] DialogManager not found.");
    }
}