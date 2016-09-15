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

        for (int i = 0; i< handSize; i++)
        {
            Tile t = deck.GetCard();
            float f = i / (handSize - 1.0f);
            t.transform.position = Vector3.Lerp(xMin, xMax, f);
            t.transform.SetParent(transform, true);
            t.gameObject.SetActive(true);
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
            transform.GetChild(i).position = Vector3.Lerp(xMin, xMax, f);
        }

    }
}
