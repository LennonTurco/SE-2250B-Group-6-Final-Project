using UnityEngine;
using UnityEngine.SceneManagement;

// attach to a GO in the first scene only (switched it to the storyline scene)
public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isFirstScene = true;

    private void Awake()
    {
        if (isFirstScene)
        {
            if (SceneManager.GetActiveScene().name == "ending")
            {
                PlayerScoreTracker.ClearPrefsButKeepFinalScore();
            }
            else
            {
                PlayerPrefs.DeleteAll();
                PlayerScoreTracker.ResetScore();
            }

            Debug.Log("[GameManager] Fresh game - stats wiped.");
        }
    }
}
