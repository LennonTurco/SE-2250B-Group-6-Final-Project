/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("Dialog Data Configuration")]
    [Tooltip("Assets in 'Resources/DialogSets' will be loaded automatically.")]
    [SerializeField] private List<CharacterDialogSetSO> characterDialogSets = new List<CharacterDialogSetSO>();

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
        {
            dialogPanel.SetActive(false);
        }

        // --- AUTOMATIC LOADING ---
        // Automatically find and load all CharacterDialogSetSO assets in any 'Resources' folder
        var loadedSets = Resources.LoadAll<CharacterDialogSetSO>("");
        if (loadedSets.Length > 0)
        {
            foreach (var set in loadedSets)
            {
                if (!characterDialogSets.Contains(set))
                    characterDialogSets.Add(set);
            }
            Debug.Log($"[DialogManager] Automatically loaded {loadedSets.Length} dialog sets from Resources.");
        }
    }

    private void Update()
    {
        if (!isShowing) return;

        // Advance when player presses Z or X using the New Input System
        if (Keyboard.current != null && (Keyboard.current.zKey.wasPressedThisFrame || Keyboard.current.xKey.wasPressedThisFrame))
        {
            ShowNextLine();
        }
    }

    /// <summary>
    /// Looks up dialog lines via CharacterId and DialogContext from the assigned data sets.
    /// </summary>
    public void ShowDialog(CharacterId charId, DialogContext context, Action onFinished = null)
    {
        Debug.Log("[DialogManager] ShowDialog called on " + charId + " with context " + context);
        var dataSet = characterDialogSets.Find(set => set != null && set.character == charId);
        if (dataSet == null)
        {
            Debug.LogWarning($"[DialogManager] No dialog set found for character {charId}. Cannot show {context}.");
            return;
        }

        var lines = dataSet.GetLines(context);
        ShowDialog(lines, onFinished);
    }

    /// <summary>
    /// Show a explicit list of dialog lines.
    /// </summary>
    public void ShowDialog(List<string> dialogLines, Action onFinished = null)
    {
        Debug.Log("[DialogManager] ShowDialog called on " + dialogLines);
        if (dialogLines == null || dialogLines.Count == 0)
        {
            Debug.LogWarning("[DialogManager] Tried to show empty dialog.");
            return;
        }

        lineQueue.Clear();
        foreach (var line in dialogLines)
        {
            lineQueue.Enqueue(line);
        }

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
        {
            yield return null;
        }

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
        {
            dialogText.text = line;
        }

        OnDialogLineAdvanced?.Invoke();
    }

    private void EndDialog()
    {
        isShowing = false;
        if (dialogPanel != null) dialogPanel.SetActive(false);
        OnDialogFinished?.Invoke();
    }

    public bool IsShowing()
    {
        return isShowing;
    }
}
*/