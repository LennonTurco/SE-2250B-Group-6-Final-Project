using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class JungleCharacterPortrait : MonoBehaviour
{
    [Header("Portraits - 0:Boy 1:ArmsDealer")]
    [SerializeField] private Sprite[] portraits = new Sprite[2];

    [Header("Name display")]
    [SerializeField] private TMP_Text nameText;

    private readonly string[] names = { "Boy", "Arms Dealer" };
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    private void Start()
    {
        if (portraits.Length > 0 && portraits[0] != null)
        {
            img.sprite = portraits[0];
        }

        if (nameText != null)
        {
            nameText.text = names[0];
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Swap(0);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Swap(1);
        }
    }

    private void Swap(int index)
    {
        if (index < 0 || index >= portraits.Length || portraits[index] == null)
        {
            return;
        }

        img.sprite = portraits[index];
        img.enabled = false;
        img.enabled = true;

        if (nameText != null)
        {
            nameText.text = names[index];
        }
    }
}
