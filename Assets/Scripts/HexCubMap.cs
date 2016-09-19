using UnityEngine;
using System.Collections.Generic;

public class HexCubMap : MonoBehaviour {

    static HexCubMap _instance;

    public static HexCubMap current
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HexCubMap>();
            }
            return _instance;
        }      
    }

    [Range(0, 10)]
    public float tileScale;

    float almostZero = 0.0000001f;

    [SerializeField, Range(1, 100)]
    int gridWidth = 15;

    [SerializeField, Range(1, 100)]
    int gridHeight = 10;
    
    [SerializeField]
    HexPos hexPosPrefab;

    [SerializeField]
    List<HexPos> hexes = new List<HexPos>();

    [SerializeField]    
    List<Vector3> startPositions = new List<Vector3>();

    [SerializeField]
    List<TileType> startPositionTypes = new List<TileType>();

    [SerializeField]
    Tile tilePrefab;

    public void ResetPermissablePosition()
    {
        for (int i = 0, l = hexes.Count; i < l; i++)
        {
            hexes[i].acceptingTile = false;
        }
    }

    bool IsCubePosition(Vector3 cubePosition)
    {
        return Mathf.Abs(cubePosition.x + cubePosition.y + cubePosition.z) < almostZero;
    }

    Vector2 GetPositionInLocalPlane(Vector3 cubePosition)
    {
        if (IsCubePosition(cubePosition))
        {
            float x = tileScale * 3 / 2 * cubePosition.z;
            float y = tileScale * Mathf.Sqrt(3) * (cubePosition.x + cubePosition.z / 2);
            return new Vector2(x, y);
        } else
        {
            throw new System.InvalidOperationException(string.Format("Supplied position {0} not on cube map", cubePosition));
        }
    }

    public static float tileDistance(Vector3 cubePositionA, Vector3 cubePositionB)
    {
        Vector3 delta = cubePositionA - cubePositionB;
        return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
    }

    IEnumerable<Vector3> GenerateCubeCoordinates()
    {        
        int x_offset = (gridWidth - gridWidth % 2) / 2;
        int q_min = -x_offset / 2;
        int q_max = q_min + Mathf.Max((gridHeight - gridHeight % 2) / 2 + gridHeight % 2 + x_offset + 1, (gridWidth - gridWidth % 2) / 2 + gridWidth % 2);
		int q_trunc = q_min + gridHeight / 2;
		for (int q = q_min; q <q_max; q++)
        {
			int low = -q - x_offset +  2 * Mathf.Max (0, q - q_trunc); 
			int high = Mathf.Min(Mathf.Max(q, -x_offset), x_offset - q + 1);
			if (q == 0) {
				high++;
			}
			//Debug.Log (string.Format ("{2}: {0} -> {1}", low, high, q));
            for (int r = low; r <  high; r++)
            {                
                yield return new Vector3(-q, -r, +q + r);
            }
        }
    }

    void Start()
    {

        hexes.Clear();
        foreach (HexPos hex in GetComponentsInChildren<HexPos>())
        {
            if (!hexes.Contains(hex))
            {
                hexes.Add(hex);
            }
        }
        Generate();
        SetupInitialField();
    }

    void OnEnable()
    {
        PlacementRule.OnPossiblePlacement += PlacementRule_OnPossiblePlacement;
    }

    void OnDisable()
    {
        PlacementRule.OnPossiblePlacement -= PlacementRule_OnPossiblePlacement;
    }

    void OnDestroy()
    {
        PlacementRule.OnPossiblePlacement -= PlacementRule_OnPossiblePlacement;
    }

    private void PlacementRule_OnPossiblePlacement(List<HexPos> positions, HexPos anchor)
    {
        for (int i=0, l=positions.Count; i< l; i++)
        {
            positions[i].acceptingTile = true;
        }
    }

    public void Generate()
    {
		if (gridWidth % 2 == 0) {
			gridWidth++;
		}
		if (gridHeight % 2 == 1) {
			gridHeight++;
		}

        int i = 0;
        foreach (Vector3 cubePosition in GenerateCubeCoordinates())
        {
            Vector3 localPosition = GetPositionInLocalPlane(cubePosition);
            SetHex(i, cubePosition, localPosition);
            i++;
        }

        while (i < hexes.Count)
        {
            hexes[i].enabled = false;
            i++;
        }
    }

    void SetupInitialField()
    {
        for (int i=0; i<startPositions.Count; i++)
        {
            Tile tile = Instantiate(tilePrefab);
            tile.map = this;
            tile.SetType(startPositionTypes[i]);
            
            HexPos pos = GetHexPos(startPositions[i]);
            pos.occupant = tile;
        }
    }

    void SetHex(int i, Vector3 cubePosition, Vector3 localPosition)
    {
        HexPos hex;
        if (i < hexes.Count)
        {
            hex = hexes[i];
        } else
        {
            hex = (HexPos) Instantiate(hexPosPrefab, transform);
            hexes.Add(hex);
        }
        hexes[i].cubePos = cubePosition;
        hexes[i].transform.localPosition = localPosition;
		hexes [i].enabled = true;
    }

    public HexPos GetClosest(Vector3 worldPosition, out float bestDiff)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        bestDiff = -1;
        HexPos bestHex = null;
        foreach (HexPos hex in hexes)
        {
            if (!hex.enabled)
            {
                continue;
            }
            float diff = Vector3.Distance(localPosition, hex.transform.localPosition);

            if (bestHex == null || diff < bestDiff)
            {
                bestDiff = diff;
                bestHex = hex;
            }
        }
        return bestHex;
    }

	public HexPos GetHexPos(Vector3 cubePosition) {

		for (int i=0; i < hexes.Count; i++) {
			if (hexes [i].cubePos == cubePosition) {
				return hexes [i];
			}
		}
		return null;
	}
		
	public IEnumerable<Tile> PlacedTiles() {
		for (int i=0; i < hexes.Count; i++) {
			if (hexes [i].enabled && hexes[i].occupant != null) {
				yield return hexes [i].occupant;
			}
		}		
	}

	public IEnumerable<HexPos> OccupiedPositions() {
		for (int i=0; i < hexes.Count; i++) {
			if (hexes [i].enabled && !hexes[i].isFree) {
				yield return hexes [i];
			}
		}			
	}

	public IEnumerable<HexPos> FreePositions() {
		for (int i=0; i < hexes.Count; i++) {
			if (hexes [i].enabled && hexes[i].isFree) {
				yield return hexes [i];
			}
		}		
	}

}
