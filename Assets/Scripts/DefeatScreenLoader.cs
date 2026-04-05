using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatScreenLoader : MonoBehaviour
{
    [SerializeField] private float delay = 10f;
    [SerializeField] private string nextScene = "LavaScene";

    private void OnEnable()
    {
        Invoke(nameof(LoadNextScene), delay);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
