using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    [Header("Dialog")]
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed = 0.05f;

    [Header("On Finish")]
    public UnityEngine.UI.Image endImage; // drag image component directly
    public string sceneToLoad;
    public float imageDisplayTime = 5f;
    public float fadeDuration = 1f;

    private int index;
    private bool isTyping;
    private Coroutine typingCoroutine;

    void Start()
    {
        textComponent.text = "";

        // hide image at start
        if (endImage != null)
        {
            Color c = endImage.color;
            c.a = 0f;
            endImage.color = c;
            endImage.gameObject.SetActive(false);
        }

        StartDialogue();
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (isTyping)
            {
                // finish line instantly
                StopCoroutine(typingCoroutine);
                textComponent.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartTyping();
    }

    void StartTyping()
    {
        typingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        textComponent.text = "";

        foreach (char c in lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartTyping();
        }
        else
        {
            StartCoroutine(FinishDialogue());
        }
    }

    IEnumerator FinishDialogue()
    {
        // hide text but keep GO active so coroutine keeps running
        textComponent.gameObject.SetActive(false);

        if (endImage != null)
        {
            endImage.gameObject.SetActive(true);

            // fade from 0 to 1
            float elapsed = 0f;
            Color c = endImage.color;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                endImage.color = c;
                yield return null;
            }
            c.a = 1f;
            endImage.color = c;

            // wait then load
            yield return new WaitForSeconds(imageDisplayTime);
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}