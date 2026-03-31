using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    // assign these in inspector - order must match
    public Sprite[] characterSprites;   // the actual player sprites
    public Sprite[] portraitSprites;    // the face portraits for UI
    public string[] characterNames;     // "Tennon", "Ninja", "LavaQueen", etc.

    public Image portraitImage;         // the UI image in bottom left
    public SpriteRenderer playerRenderer;

    int currentIndex = 0;

    void Start()
    {
        // load last used character if saved
        currentIndex = PlayerPrefs.GetInt("CurrentCharacter", 0);
        ApplyCharacter();
    }

    // call this from input or a UI button
    public void SwitchCharacter(int direction)
    {
        // direction: +1 for next, -1 for prev
        currentIndex = (currentIndex + direction + characterNames.Length) % characterNames.Length;
        PlayerPrefs.SetInt("CurrentCharacter", currentIndex);
        ApplyCharacter();
    }

    void ApplyCharacter()
    {
        playerRenderer.sprite = characterSprites[currentIndex];
        portraitImage.sprite = portraitSprites[currentIndex];
    }

    // if other scripts need to know who's active
    public string GetCurrentName()
    {
        return characterNames[currentIndex];
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }
}