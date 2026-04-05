using UnityEngine;

// attach to a GO in the first scene only (switched it to the storyline scene)
public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isFirstScene = true;

    private void Awake()
    {
        if (isFirstScene)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("[GameManager] Fresh game - stats wiped.");
        }
    }
}