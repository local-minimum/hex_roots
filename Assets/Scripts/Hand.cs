using UnityEngine;
using System.Collections.Generic;

public class Hand : MonoBehaviour {

    Plane plane = new Plane(Vector3.forward, Vector3.up);

    [SerializeField, Range(0, 1)]
    float xPostion;

    [SerializeField, Range(0, 1)]
    float bottomOffset;

    [SerializeField, Range(1, 10)]
    int handSize;

    [SerializeField, Range(0, 1)]
    float width;

    List<Tile> tilesInHand = new List<Tile>();

    [SerializeField]
    Deck deck;

    Vector3 xMin;
    Vector3 xMax;

    void Start()
    {
        PositionOnScreen();
        RefillHand();
    }

    int totalCardsHavingPassedHand = 0;

    void RefillHand(int overshoot = 0)
    {
        for (int i = transform.childCount; i < handSize + overshoot; i++)
        {
            totalCardsHavingPassedHand++;
            Tile t = deck.GetCard();
            float f = i / (handSize - 1.0f);
            t.transform.position = Vector3.Lerp(xMin, xMax, f);
            t.transform.SetParent(transform, true);
            t.gameObject.SetActive(true);
            t.name = string.Format("{0} (Tile {1})", t.tileType, totalCardsHavingPassedHand);
            t.map = HexCubMap.current;
            t.Status = TileEventType.Drawn;
        }
    }

    void LateUpdate()
    {
        PositionOnScreen();
    }

    void PositionOnScreen()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * xPostion, Screen.height * bottomOffset));
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            transform.position = ray.GetPoint(distance);
        }

        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * (xPostion - width / 2), Screen.height * bottomOffset));        
        if (plane.Raycast(ray, out distance))
        {
            xMin = ray.GetPoint(distance);
        }

        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * (xPostion + width / 2), Screen.height * bottomOffset));
        if (plane.Raycast(ray, out distance))
        {
            xMax = ray.GetPoint(distance);
        }

        for (int i=0, l=transform.childCount; i< l; i++)
        {
            float f = i / (handSize - 1.0f);
            Transform child = transform.GetChild(i);
            TileEventType status = child.GetComponent<Tile>().Status;
            if (status == TileEventType.InHand || status == TileEventType.HandHovered)
            {
                child.position = Vector3.Lerp(xMin, xMax, f);
            }
        }

    }

    void OnEnable()
    {
        Tile.OnTileEvent += Tile_OnTileEvent;
    }

    void OnDisable()
    {
        Tile.OnTileEvent -= Tile_OnTileEvent;
    }

    void OnDestroy()
    {
        Tile.OnTileEvent -= Tile_OnTileEvent;
    }

    private void Tile_OnTileEvent(Tile tile, TileEventType eventType)
    {
        if (eventType == TileEventType.Placed)
        {
            RefillHand(1);
        }
    }
}
