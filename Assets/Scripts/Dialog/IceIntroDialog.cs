using UnityEngine;
using TMPro;
using System.Collections;

public class IceIntroDialog : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI introText;

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.03f;

    private string fullText = "Welcome to Level 3. Your goal is to defeat Jose the Ice Mage. Start by collecting gold, exchange for fishing rods to fish. Mike the Polar Bear who lives in a special igloo will give you a pickaxe and tell you how to find Jose in exchange for 3 fish.";

    private void Start()
    {
        Enemy.isPaused = true;

        if (introText != null)
            StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        introText.text = "";

        foreach (char c in fullText)
        {
            introText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        yield return new WaitForSeconds(2f);

        Enemy.isPaused = false;
        gameObject.SetActive(false);
    }
}