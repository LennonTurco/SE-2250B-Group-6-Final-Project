using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Image))]
public class CharacterPortraitLava : MonoBehaviour
{
    [Header("Portraits - 0:Boy 1:ArmsDealer 2:Ninja 3:Ice 4:Lava")]
    [SerializeField] Sprite[] portraits = new Sprite[4];

    [Header("Name display")]
    [SerializeField] TMP_Text nameText;

    Image img;
    string[] names = { "Boy", "Arms Dealer", "Ninja", "Ice Mage"};

    void Awake()
    {
        img = GetComponent<Image>();
    }

    void Start()
    {
        if (portraits[0] != null)
            img.sprite = portraits[0];
        if (nameText != null)
            nameText.text = names[0];
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) Swap(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) Swap(1);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) Swap(2);
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) Swap(3);
    }

    void Swap(int i)
    {
        if (i < 0 || i >= portraits.Length || portraits[i] == null) return;
        img.sprite = portraits[i];
        img.enabled = false;
        img.enabled = true;
        if (nameText != null)
            nameText.text = names[i];
    }
}