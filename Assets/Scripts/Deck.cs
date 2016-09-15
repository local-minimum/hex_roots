using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour {

    [SerializeField]
    List<TileType> cardType;

    [SerializeField]
    List<int> initialCount;

    List<Tile> deck = new List<Tile>();

    [SerializeField]
    Tile prefab;

    int nextCard = 0;

	void Start () {
        CreateCards();
        ShuffleCards();
	}
	
    void CreateCards()
    {
        if (deck.Count > 0)
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
        if (nextCard >= n)
        {
            nextCard %= n;
            ShuffleCards();
        }
        Tile t = deck[nextCard];
        nextCard++;
        return t;
    }
}
