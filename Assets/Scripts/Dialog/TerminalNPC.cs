using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerminalNPC : MonoBehaviour
{
    [SerializeField] private List<string> notReadyLines = new List<string>
    {
        "You don't have the full code yet.",
        "Find Solomon's informants scattered through the city."
    };

    [SerializeField] private List<string> readyLines = new List<string>
    {
        "You have the code. Enter it at the terminal.",
        "Don't get it wrong."
    };

    [SerializeField] private List<string> alreadySolvedLines = new List<string>
    {
        "The gate is already open. Go."
    };

    [SerializeField] private string terminalSceneName = "TerminalScene";
    [SerializeField] private float sceneLoadDelay = 10f; // adjust to match how long dialog takes

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;
        if (PuzzleManager.Instance == null || DialogManager.Instance == null) return;

        if (PuzzleManager.Instance.IsSolved())
        {
            DialogManager.Instance.ShowDialog(alreadySolvedLines);
            return;
        }

        if (!PuzzleManager.Instance.AllCluesFound())
        {
            DialogManager.Instance.ShowDialog(notReadyLines);
            return;
        }

        hasTriggered = true;
        DialogManager.Instance.ShowDialog(readyLines);
        StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(terminalSceneName);
    }
}