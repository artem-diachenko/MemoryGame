using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    private const int DefaultCount = 6;
    private int _cardsCount = DefaultCount;

    //private bool _isInitialized;
    private ImageSource _source;

    public GameObject[] Cards;

    public void SetCardsCount(InputField field)
    {
        if (field == null)
        {
            return;
        }
        
        int count;
        if (!int.TryParse(field.text, out count))
        {
            return;
        }

        if (count >= 1)
        {
            _cardsCount = count;
            StartCoroutine(_source.LoadImages(_cardsCount));
        }
        else
        {
            field.text = (_cardsCount = DefaultCount).ToString();
        }
    }

    private void Awake()
    {
        _source = new ImageSource();
    }

    private void Start()
    {
        StartCoroutine(_source.LoadImages(_cardsCount));
    }

    private void Update()
    {
        if (_source.IsLoaded)
        {
            InitializeCards();
        }
    }

    private void InitializeCards()
    {
        var texture = _source.Images.Last();
        Cards[0].GetComponent<Card>().CardFront = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
        //for (var i = 0; i < 2; i++)
        //{
        //    for (var j = 0; j < _cardsCount; j++)
        //    {
        //        var isInitialized = true;
        //        var position = 0;

        //        while (isInitialized)
        //        {
        //            position = Random.Range(0, Cards.Length);
        //            isInitialized = (Cards[position].GetComponent<Card>().IsInitialized);
        //        }

        //        Cards[position].GetComponent<Card>().Number = j;
        //        Cards[position].GetComponent<Card>().IsInitialized = true;
        //    }
        //}

        //foreach (var card in Cards)
        //{
        //    card.GetComponent<Card>().SetUp();
        //}

        //_isInitialized = true;
    }

    private void Check()
    {
        var shownCards = Cards
            .Where(card => card.GetComponent<Card>().IsShown)
            .Select(card => card.GetComponent<Card>())
            .ToList();

        if (shownCards.Count == 2 && shownCards[0].Number == shownCards[1].Number)
        {
        }
    }
}

internal class ImageSource
{
    private readonly List<Texture2D> _images = new List<Texture2D>();

    public bool IsLoaded { get; private set; }

    public List<Texture2D> Images
    {
        get { return _images; }
    }

    public IEnumerator LoadImages(int count)
    {
        if (count <= 0)
        {
            Debug.LogWarning("Required images count must be positive ");
            yield break;
        }

        IsLoaded = false;

        var www = new WWW(string.Format("{0}/{1}", DataAccess.WebUrlString, DataAccess.ImgSourceFileName));
        yield return www;

        if (www.error != null)
        {
            Debug.LogError("Error .. " + www.error);
            yield break;
        }

        Images.Clear();

        var links = www.text.Split(new[] {"\r", "\n", " "}, StringSplitOptions.RemoveEmptyEntries).Take(count).ToList();
        yield return LoadImages(links);

        IsLoaded = true;

        Debug.LogWarning("Loaded images count: " + Images.Count);
    }

    private IEnumerator LoadImages(IList<string> links)
    {
        foreach (var link in links)
        {
            var www = new WWW(string.Format("{0}/{1}", DataAccess.WebUrlString, link));
            yield return www;

            if (www.error == null)
            {
                Images.Add(www.texture);
            }
        }
    }
}

internal class DataAccess
{
    public static string WebUrlString = @"http://www.novility.com/interview";

    public static string ImgSourceFileName = @"list.txt";
}