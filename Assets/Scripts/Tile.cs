using UnityEngine;
using System.Collections;

public enum TileType
{
	Root,
	Meristem,
	Nodle,
	Hair,
	Storage,
	RootShoot
}

public enum TileEventType
{
    Drawn,
    HandHovered,
    Dragged,
    Placed,
    Disactivated,
    Discarded,
    Destroyed
}

public delegate void TileEvent(Tile tile, TileEventType eventType);

public class Tile : MonoBehaviour {

    public static event TileEvent OnTileEvent;

	public TileType tileType;

    [SerializeField]
    Material[] materials;
    
    public HexCubMap map;

    [SerializeField, Range(0, 1)]
    float snapDistance;

    TileEventType status = TileEventType.Drawn;

    bool mustBePlaced = false;

    public TileEventType Status
    {
        get
        {
            return status;
        }

        set
        {
            if (status != value && OnTileEvent != null)
            {
                OnTileEvent(this, value);
            }
            status = value;
        }
    }

    public void SetType(TileType tileType)
    {
        this.tileType = tileType;
        Renderer rend = GetComponent<Renderer>();
        Material[] mats = rend.materials;
        mats[1] = materials[(int)tileType];
        rend.materials = mats;
    }

    public void Place()
    {
        Status = TileEventType.Placed;
    }

    public void ReActivate()
    {
        mustBePlaced = true;
        Status = TileEventType.Dragged;
    }

    Plane plane = new Plane(Vector3.forward, Vector3.up);

    float dist;
    HexPos closest;

    bool _hovered = false;

    void Update()
    {
        if (_hovered)
        {

            /*
            if (Input.GetMouseButtonDown(0))
            {
                if (closest)
                {
                    Tile t = Instantiate(this);
                    t.transform.position = closest.transform.position;
                    Status = TileEventType.Placed;
                }
            }
            */

        }
    }

    void LateUpdate () {
        if (status == TileEventType.Dragged) {

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

    void OnMouseEnter()
    {
        
        _hovered = true;
        if (OnTileEvent != null)
        {
            OnTileEvent(this, TileEventType.HandHovered);
        }
    }

    void OnMouseExit()
    {
        _hovered = false;
        map.ResetPermissablePosition();
    }
}
