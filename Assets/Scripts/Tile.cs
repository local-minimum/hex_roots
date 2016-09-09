using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    [SerializeField]
    HexCubMap map;

    [SerializeField, Range(0, 1)]
    float snapDistance;

    [SerializeField]
    bool trackingMouse;

    Plane plane = new Plane(Vector3.forward, Vector3.up);

    float dist;
    HexPos closest;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (closest)
            {
                Tile t = Instantiate(this);
                t.transform.position = closest.transform.position;
                t.trackingMouse = false;
            }
        }

    }

    void LateUpdate () {
        if (trackingMouse) {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            if (plane.Raycast(ray, out distance))
            {
                transform.position = ray.GetPoint(distance);
                closest = map.GetClosest(transform.position, out dist);
                if (Snapping)
                {
                    transform.position = closest.transform.position;
                }
            }
        }
	}

    bool Snapping
    {
        get
        {
            return dist < map.tileScale * snapDistance;
        }
    }

    void OnDrawGizmosSelected()
    {

        if (closest)
        {
            Gizmos.color = Snapping ? Color.red : Color.gray;
            Gizmos.DrawLine(transform.position, closest.transform.position);
        }
    }

}
