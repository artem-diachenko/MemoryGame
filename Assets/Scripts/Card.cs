using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public GameObject Manager;

    public Sprite CardBack;
    public Sprite CardFront;

    public bool IsDestroyed;
    public bool IsShown;
    public int Number;
    
    public void FlipCard(bool checkFieldRequired)
    {
        GetComponent<Image>().sprite = IsShown ? CardBack : CardFront;
        IsShown = !IsShown;

        if (checkFieldRequired)
        {
            StartCoroutine(Manager.GetComponent<CardManager>().CheckField());
        }
    }

    private void Awake()
    {
        GetComponent<Image>().sprite = CardBack;
    }
}