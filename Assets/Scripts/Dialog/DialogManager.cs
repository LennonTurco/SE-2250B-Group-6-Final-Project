using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// in-world npc dialog - singleton, one per scene
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TMP_Text dialogText;

    [SerializeField] private float textSpeed = 0.03f; // seconds per character

    private Queue<string> lineQueue = new Queue<string>();
    private bool isShowing = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

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

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
        {
            if (isTyping)
            {
                // skip to end of current line
                StopCoroutine(typingCoroutine);
                isTyping = false;
                dialogText.maxVisibleCharacters = dialogText.text.Length;
            }
            else
            {
                ShowNextLine();
            }
        }
    }

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
        string line = lineQueue.Dequeue();
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = line;
        dialogText.maxVisibleCharacters = 0;

        foreach (char _ in line)
        {
            dialogText.maxVisibleCharacters++;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    private void EndDialog()
    {
        isShowing = false;
        isTyping = false;
        dialogPanel.SetActive(false);
        dialogText.text = "";
        dialogText.maxVisibleCharacters = 0;
    }

    public bool IsDisplaying() => isShowing;
}