using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBackButton : MonoBehaviour
{
    public void GoBack()
    {
        // If loaded additively, there will be more than 1 scene open
        if (SceneManager.sceneCount > 1)
        {
            SceneManager.UnloadSceneAsync(gameObject.scene.name);
        }
        else
        {
            // Fallback for isolated testing of the shop scene
            string previousScene = PlayerPrefs.GetString("PreviousScene");

            if (!string.IsNullOrEmpty(previousScene))
            {
                SceneManager.LoadScene(previousScene);
            }
            else
            {
                Debug.LogWarning("No previous scene saved!");
            }
        }
    }
}