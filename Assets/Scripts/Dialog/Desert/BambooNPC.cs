using System.Collections.Generic;
using UnityEngine;

public class BambooNPC : MonoBehaviour
{
    [SerializeField] private Transform bambooMan;
    [SerializeField] private List<string> bambooLines = new List<string>
    {
        "Hello! I guard the path to the Jungle!",
        "Master Antuna told me to not let anyone through!",
        "Unless you convince him, I can't let you through!",
        "Where is he? Well why don't you ask his pet, the Mega Sphinx?"
    };

    [SerializeField] private List<string> antunaDefeatedLines = new List<string>
    {
        "Wow! You actually defeated Master Antuna!",
        "A deal is a deal. You may pass!"
    };

    private bool hasMoved = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        AntunaBoss antuna = FindFirstObjectByType<AntunaBoss>();

        if (antuna != null && !antuna.isDead && !hasMoved)
        {
            // Antuna is alive
            DialogManager.Instance.ShowDialog(bambooLines);
        }
        else
        {
            // Antuna was destroyed.
            DialogManager.Instance.ShowDialog(antunaDefeatedLines);
            
            if (!hasMoved)
            {
                hasMoved = true;
                Debug.Log("Bamboo Man is leaving");
                StartCoroutine(moveBambooMan());
            }
        }
    }

    private System.Collections.IEnumerator moveBambooMan()
    {
        if (bambooMan == null) yield break;

        Vector2 targetPos = bambooMan.position + new Vector3(0, 20f, 0);
        Vector2 startPos = bambooMan.position;
        
        float duration = 5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Linear movement
            bambooMan.position = Vector2.Lerp(startPos, targetPos, t);
            
            yield return null;
        }

        Destroy(bambooMan.gameObject);
    }
}