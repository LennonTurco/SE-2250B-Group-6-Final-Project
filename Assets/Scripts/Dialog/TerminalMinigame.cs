using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// attach to empty GO in TerminalScene
// wire buttons in inspector: element 0 = button labeled 0, element 1 = button labeled 1, etc.
public class TerminalMinigame : MonoBehaviour
{
    [SerializeField] private List<Button> digitButtons;
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private string citySceneName = "CityScene";
    [SerializeField] private float loadDelay = 2f;

    private int[] correctCode = { 6, 7, 9 };
    private List<int> enteredDigits = new List<int>();

    private void Start()
    {
        feedbackText.text = "";
        UpdateDisplay();

        for (int i = 0; i < digitButtons.Count; i++)
        {
            int digit = i;
            digitButtons[i].onClick.AddListener(() => OnDigitPressed(digit));
        }
    }

    private void OnDigitPressed(int digit)
    {
        if (enteredDigits.Count >= correctCode.Length) return;

        enteredDigits.Add(digit);
        UpdateDisplay();

        if (enteredDigits.Count == correctCode.Length)
            CheckCode();
    }

    private void UpdateDisplay()
    {
        string display = "";
        for (int i = 0; i < correctCode.Length; i++)
            display += (i < enteredDigits.Count ? enteredDigits[i].ToString() : "_") + " ";
        displayText.text = display.Trim();
    }

    private void CheckCode()
    {
        for (int i = 0; i < correctCode.Length; i++)
        {
            if (enteredDigits[i] != correctCode[i])
            {
                feedbackText.text = "ACCESS DENIED. TRY AGAIN.";
                enteredDigits.Clear();
                Invoke(nameof(ResetFeedback), 1.5f);
                return;
            }
        }

        feedbackText.text = "ACCESS GRANTED.";
        PlayerPrefs.SetInt("PuzzleSolved", 1);
        PlayerPrefs.Save();
        Debug.Log($"[TerminalMinigame] PuzzleSolved written. Verify: {PlayerPrefs.GetInt("PuzzleSolved", 0)}");

        foreach (Button b in digitButtons) b.interactable = false;
        Invoke(nameof(LoadCityScene), loadDelay);
    }

    private void ResetFeedback()
    {
        feedbackText.text = "";
        UpdateDisplay();
    }

    private void LoadCityScene()
    {
        SceneManager.LoadScene(citySceneName);
    }
}