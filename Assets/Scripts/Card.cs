using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Sprite CardBack;
    public Sprite CardFront;
    public bool IsInitialized;
    public bool IsShown;
    public int Number;

    /// <summary>
    /// Change state of card
    /// </summary>
    public void FlipCard()
    {
        GetComponent<Image>().sprite = IsShown ? CardBack : CardFront;
        IsShown = !IsShown;
    }

    private void Start()
    {
        GetComponent<Image>().sprite = CardBack;
    }

    private void Update()
    {
    }
}