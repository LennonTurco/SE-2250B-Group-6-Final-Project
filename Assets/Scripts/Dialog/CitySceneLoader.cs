using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject solomonPrefab;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private List<string> solomonSpawnDialog = new List<string>
    {
        "So. You found the code.",
        "Impressive. But it ends here.",
        "Come. Let's finish this."
    };

    private void Start()
    {
        if (PlayerPrefs.GetInt("PuzzleSolved", 0) == 1)
        {
            Debug.Log("[CitySceneLoader] PuzzleSolved key found - starting sequence.");
            PlayerPrefs.DeleteKey("PuzzleSolved");
            StartCoroutine(SolomonSequence());
        }
        else
        {
            Debug.Log("[CitySceneLoader] PuzzleSolved key not found - skipping sequence.");
        }
    }

    private IEnumerator SolomonSequence()
    {
        // wait 3 frames for all Awake/Start calls to finish
        yield return null;
        yield return null;
        yield return null;

        Debug.Log("[CitySceneLoader] Running solomon sequence.");

        // open the gate
        if (PuzzleManager.Instance != null)
        {
            Debug.Log("[CitySceneLoader] Calling TrySolve.");
            PuzzleManager.Instance.TrySolve();
        }
        else
        {
            Debug.LogWarning("[CitySceneLoader] PuzzleManager not found.");
        }

        yield return new WaitForSeconds(1f);

        // spawn solomon
        if (solomonPrefab != null && spawnPoint != null)
        {
            Debug.Log("[CitySceneLoader] Spawning Solomon.");
            Instantiate(solomonPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("[CitySceneLoader] Solomon prefab or spawn point not assigned.");
        }

        yield return new WaitForSeconds(0.5f);

        // play dialog
        if (DialogManager.Instance != null)
        {
            Debug.Log("[CitySceneLoader] Playing dialog.");
            DialogManager.Instance.ShowDialog(solomonSpawnDialog);
        }
        else
        {
            Debug.LogWarning("[CitySceneLoader] DialogManager not found.");
        }
    }
}