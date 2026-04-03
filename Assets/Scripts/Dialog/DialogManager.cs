using System.Collections.Generic;
using UnityEngine;
using TMPro;

// in-world npc dialog - singleton, one per scene
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    // wire in inspector
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TMP_Text dialogText;

    private Queue<string> lineQueue = new Queue<string>();
    private bool isShowing = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (dialogPanel != null) dialogPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isShowing) return;
        // Z or X advances - player.cs must check IsDisplaying() before attacking
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
            ShowNextLine();
    }

    // called by DialogTrigger on NPCs
    public void ShowDialog(List<string> lines)
    {
        if (lines == null || lines.Count == 0) return;

        lineQueue.Clear();
        foreach (string line in lines) lineQueue.Enqueue(line);

        isShowing = true;
        dialogPanel.SetActive(true);
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (lineQueue.Count == 0) { EndDialog(); return; }
        dialogText.text = lineQueue.Dequeue();
    }

    private void EndDialog()
    {
        isShowing = false;
        dialogPanel.SetActive(false);
        dialogText.text = "";
    }

    // player.cs and other systems check this before processing input
    public bool IsDisplaying() => isShowing;
}