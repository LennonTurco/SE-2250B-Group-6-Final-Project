using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// stateless dialog display service.
// renders a sequence of dialog lines and advances on player input.
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TMPro.TMP_Text dialogText;

    [Header("Events")]
    public UnityEvent OnDialogStarted;
    public UnityEvent OnDialogLineAdvanced;
    public UnityEvent OnDialogFinished;

    private readonly Queue<string> lineQueue = new Queue<string>();
    private bool isShowing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isShowing) return;

        // advance when player presses Z or X
        // Z key is also used for attack — player must check IsDisplaying() before attacking
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
            ShowNextLine();
    }

    // show a dialog sequence
    public void ShowDialog(List<string> dialogLines, Action onFinished = null)
    {
        if (dialogLines == null || dialogLines.Count == 0)
        {
            Debug.LogWarning("[DialogManager] Tried to show empty dialog.");
            return;
        }

        lineQueue.Clear();
        foreach (var line in dialogLines)
            lineQueue.Enqueue(line);

        StartCoroutine(ShowDialogRoutine(onFinished));
    }

    private IEnumerator ShowDialogRoutine(Action onFinished)
    {
        isShowing = true;
        if (dialogPanel != null) dialogPanel.SetActive(true);
        OnDialogStarted?.Invoke();

        ShowNextLine();

        // wait until dialog is done
        while (isShowing)
            yield return null;

        onFinished?.Invoke();
    }

    private void ShowNextLine()
    {
        if (lineQueue.Count == 0)
        {
            EndDialog();
            return;
        }

        string line = lineQueue.Dequeue();
        if (dialogText != null)
            dialogText.text = line;

        OnDialogLineAdvanced?.Invoke();
    }

    private void EndDialog()
    {
        isShowing = false;
        if (dialogPanel != null) dialogPanel.SetActive(false);
        OnDialogFinished?.Invoke();
    }

    // used by PlayerMovement to check before processing attack input
    public bool IsDisplaying() => isShowing;
}