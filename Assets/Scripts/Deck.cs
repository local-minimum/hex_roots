using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour {

    [SerializeField]
    List<TileType> cardType;

    [SerializeField]
    List<int> initialCount;

    List<Tile> deck = new List<Tile>();

    bool deckCreated = false;
    [SerializeField]
    Tile prefab;

    int nextCard = 0;

	void Start () {
        CreateCards();
        ShuffleCards();
	}
	
    void CreateCards()
    {
        if (deckCreated)
        {
            return;
        }

        for (int i=0; i<cardType.Count; i++)
        {
            for (int j=0; j<initialCount[i]; j++)
            {
                Tile t = Instantiate(prefab);
                t.SetType(cardType[i]);
                t.transform.SetParent(transform);
                t.gameObject.SetActive(false);
                deck.Add(t);
            }

        }

        deckCreated = true;

    }

    void ShuffleCards()
    {
        int n = deck.Count;
        int l = n;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, l);
            Tile value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

    public Tile GetCard()
    {
        CreateCards();
        int n = deck.Count;

        if (n == 0)
        {
            return null;
        }
        bool hasReshuffled = false;
        if (nextCard >= n)
        {
            hasReshuffled = true;
            nextCard %= n;
            ShuffleCards();
        }
        
        while (!(deck[nextCard].Status == TileEventType.InDeck || deck[nextCard].Status == TileEventType.Placed))
        {
            nextCard++;
            if (nextCard >= n)
            {
                if (hasReshuffled)
                {
                    // No cards remain in deck
                    return null;
                }
                nextCard %= n;
                ShuffleCards();
                hasReshuffled = true;
            }
        } 

        Tile t = deck[nextCard];
        if (t.Status == TileEventType.Placed)
        {
            t = Instantiate(t);
            t.Status = TileEventType.InDeck;
        }
        nextCard++;
        return t;
    }
}
