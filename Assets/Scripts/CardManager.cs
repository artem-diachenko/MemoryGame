using Assets.Classes;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    private const float Delay = 0.2f;
    private bool _isInitializing;

    private int _steps;
    private Stopwatch _time;

    private int _cardsCount;    
    private ImageSource _source;

    public List<GameObject> Cards;

    public GameObject PrefabCard;
    public GameObject GridPanel;
    public GameObject Status;

    public void StartGame()
    {
        if (_isInitializing)
        {
            return;
        }

        _isInitializing = true;
        _steps = 0;
        _source = new ImageSource();

        Status.GetComponent<Text>().text = string.Empty;

        StartCoroutine(InitializeCards());
    }

    public void SetCardsCount(int count)
    {
        if (count < 0)
        {
            Debug.LogWarning("Required cards count must be positive ");
            return;
        }

        _cardsCount = count;
    }
    
    public IEnumerator CheckField()
    {
        yield return new WaitForSeconds(Delay);

        Status.GetComponent<Text>().text = string.Format(Messages.StepsFormat, ++_steps);

        List<int> shownCards = Cards
            .Where(card => card != null && card.GetComponent<Card>().IsShown && !card.GetComponent<Card>().IsDestroyed)
            .Select(card => card.GetComponent<Card>().Number)
            .ToList();

        if (shownCards.Count == 2 && shownCards[0] == shownCards[1])
        {
            foreach (var card in Cards.Where(card => card != null && card.GetComponent<Card>().IsShown))
            {
                card.GetComponent<Card>().IsDestroyed = true;
                Destroy(card);
            }

            Cards.RemoveAll(x => x != null && x.GetComponent<Card>().Number == shownCards[0]);
        }
        else if (shownCards.Count == 2)
        {
            foreach (var card in Cards.Where(card => card != null && card.GetComponent<Card>().IsShown))
            {
                card.GetComponent<Card>().FlipCard(false);
            }
        }

        if (Cards.Count == 0)
        {
            _time.Stop();
            Status.GetComponent<Text>().text = 
                string.Format(Messages.WinMessageFormat, _steps, _time.Elapsed.Hours, _time.Elapsed.Minutes, _time.Elapsed.Seconds);
        }
    }

    private void Start()
    {
        Cards = new List<GameObject>();

        _time = new Stopwatch();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey("enter") || Input.GetKey("return"))
        {
            if (_cardsCount > 0)
            {
                StartGame();
            }
        }
    }

    private IEnumerator InitializeCards()
    {
        yield return _source.LoadImages(_cardsCount);

        if (Cards.Any())
        {
            foreach (var card in Cards)
            {
                Destroy(card);
            }

            Cards.Clear();
        }

        for (var i = 0; i < _source.Images.Count; i++)
        {
            InitializeCard(i, Instantiate(PrefabCard));
            InitializeCard(i, Instantiate(PrefabCard));
        }
        
        if (_time != null)
        {
            _time.Reset();
            _time.Start();
        }

        _isInitializing = false;
    }

    private void InitializeCard(int number, GameObject instance)
    {
        var distanceX = 400; //TODO: calculate this value based on size of Parent
        var distanceY = 300;

        var frontSprite = Sprite.Create(_source.Images[number], new Rect(0, 0, _source.Images[number].width, _source.Images[number].height), new Vector2(0.5f, 0.5f));

        instance.GetComponent<Card>().CardFront = frontSprite;
        instance.GetComponent<Card>().Number = number;

        instance.GetComponent<Card>().Manager = gameObject;

        instance.transform.SetParent(GridPanel.transform, false);
        instance.transform.localPosition = new Vector3(Random.Range(distanceX * -1, distanceX), Random.Range(distanceY * -1, distanceY), 0);

        instance.AddComponent(typeof(BoxCollider2D));
        instance.GetComponent<BoxCollider2D>().size = new Vector3(frontSprite.rect.width, frontSprite.rect.height, 0);

        for (var i = 0; i < 10000; i++)
        {
            if (Cards.Any(c => c.GetComponent<BoxCollider2D>().bounds.Intersects(instance.GetComponent<BoxCollider2D>().bounds)))
            {
                instance.transform.localPosition = new Vector3(Random.Range(distanceX * -1, distanceX), Random.Range(distanceY * -1, distanceY), 0);
                continue;
            }

            break;
        }

        Cards.Add(instance);
    }
}