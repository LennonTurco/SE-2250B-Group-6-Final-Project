using System.Collections.Generic;
using UnityEngine;

public class TutorialNPC : MonoBehaviour
{

    [SerializeField] private List<string> tutorialLines = new List<string>
    {
        "Use WASD or the arrow keys to move! Press [Z] to attack and [X] to interact!"
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        DialogManager.Instance.ShowDialog(tutorialLines);
    }
}