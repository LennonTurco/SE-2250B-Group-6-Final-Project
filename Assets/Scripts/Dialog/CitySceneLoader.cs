using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach to empty GO in CityScene
// fires solomon spawn sequence when returning from terminal
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
            PlayerPrefs.DeleteKey("PuzzleSolved");
            StartCoroutine(SolomonSequence());
        }
    }

    private IEnumerator SolomonSequence()
    {
        // wait a frame for scene to fully initialize
        yield return null;

        // open the gate
        if (PuzzleManager.Instance != null)
            PuzzleManager.Instance.TrySolve();

        yield return new WaitForSeconds(1f);

        // spawn solomon
        if (solomonPrefab != null && spawnPoint != null)
            Instantiate(solomonPrefab, spawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        // play entrance dialog
        if (DialogManager.Instance != null)
            DialogManager.Instance.ShowDialog(solomonSpawnDialog);
    }
}