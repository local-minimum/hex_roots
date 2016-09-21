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
    InDeck,
    Drawn,
    InHand,
    HandHovered,
    HandHoverCancel,
    Dragged,
    Placed,
    DragCancel,
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

    TileEventType status = TileEventType.InDeck;

    bool mustBePlaced = false;
    float statusChangeTime;

    public TileEventType Status
    {
        get
        {
            return status;
        }

        set
        {
            statusChangeTime = Time.realtimeSinceStartup;
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

    public void ReActivate()
    {
        mustBePlaced = true;
        _hovered = this;
        Status = TileEventType.HandHovered;
        Status = TileEventType.Dragged;
    }

    public void Place(HexPos pos)
    {
        mustBePlaced = true;
        closest = pos;
        Status = TileEventType.Placed;        
    }

    Plane plane = new Plane(Vector3.forward, Vector3.up);

    float dist;
    HexPos closest;

    public HexPos Placement {
        get
        {
            return closest;
        }
    }

    static Tile _hovered = null;

    bool hovered
    {
        get
        {
            return _hovered == this;
        }
    }

    [SerializeField, Range(0, 1)]
    float clickMaxTime = 0.5f;

    bool dragByClick = false;

    void Update()
    {
        if (status == TileEventType.Drawn || status == TileEventType.HandHoverCancel || status == TileEventType.DragCancel)
        {
            Status = TileEventType.InHand;
        }

        if (hovered)
        {

            if (status == TileEventType.HandHovered && Input.GetMouseButtonDown(0))
            {
                dragByClick = false;
                Status = TileEventType.Dragged;
            } else if(status == TileEventType.Dragged && Input.GetMouseButtonUp(0))
            {
                if (Time.realtimeSinceStartup - statusChangeTime < clickMaxTime)
                {
                    dragByClick = true;
                }
                else {
                    ClearSelfHovered();
                    if (Snapping)
                    {
                        Status = TileEventType.Placed;
                    }
                    else
                    {
                        Status = TileEventType.DragCancel;
                    }
                }
            } else if (status == TileEventType.Dragged && dragByClick && Input.GetMouseButtonUp(0))
            {
                ClearSelfHovered();
                if (Snapping)
                {
                    Status = TileEventType.Placed;
                } else
                {
                    Status = TileEventType.DragCancel;
                }
            }

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
                if (Snapping && closest != null)
                {
                    transform.position = closest.transform.position;
                }
            }
        } else if (status == TileEventType.InHand)
        {
            closest = null;
        }
	}

    bool Snapping
    {
        get
        {
            return (closest != null) && (dist < map.tileScale * snapDistance);
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

    void OnMouseOver()
    {
        if (_hovered == null && status == TileEventType.InHand)
        {
            _hovered = this;
            Status = TileEventType.HandHovered;
        }
    }

    void ClearSelfHovered()
    {
        if (hovered)
        {
            _hovered = null;
            map.ResetPermissablePosition();
        }
    }

    void OnMouseExit()
    {
        if (hovered && status != TileEventType.Dragged) {
            ClearSelfHovered();
            Status = TileEventType.HandHoverCancel;
        }
    }

    void OnEnable()
    {
        OnTileEvent += Tile_OnTileEvent;
    }

    void OnDisable()
    {
        OnTileEvent -= Tile_OnTileEvent;
        if (hovered)
        {
            _hovered = null;
        }
    }

    void OnDestroy()
    {
        OnTileEvent -= Tile_OnTileEvent;
        if (hovered)
        {
            _hovered = null;
        }
    }


    private void Tile_OnTileEvent(Tile tile, TileEventType eventType)
    {
        if (tile == this && eventType == TileEventType.Placed)
        {
            if (closest)
            {
                closest.occupant = this;
            } else
            {
                Status = TileEventType.DragCancel;
            }
        }
    }
}
