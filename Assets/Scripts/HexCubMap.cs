using UnityEngine;
using System.Collections.Generic;

public class HexCubMap : MonoBehaviour {

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

    public float tileDistance(Vector3 cubePositionA, Vector3 cubePositionB)
    {
        Vector3 delta = cubePositionA - cubePositionB;
        return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
    }

    IEnumerable<Vector3> GenerateCubeCoordinates()
    {        
        int x_offset = (gridWidth - gridWidth % 2) / 2;
        int q_min = -x_offset / 2;
        int q_max = q_min + Mathf.Max((gridHeight - gridHeight % 2) / 2 + gridHeight % 2, (gridWidth - gridWidth % 2) / 2 + gridWidth % 2);

		for (int q = q_min; q <q_max; q++)
        {
			int high = Mathf.Min(Mathf.Max(q, -x_offset), x_offset - q + 1);
			if (q == 0) {
				high++;
			}
			Debug.Log (string.Format ("{2}: {0} -> {1}", -q - x_offset, high, q));
            for (int r = -q - x_offset; r <  high; r++)
            {                
                yield return new Vector3(-q, -r, +q + r);
            }
        }
    }

    void Start()
    {
        foreach (HexPos hex in GetComponentsInChildren<HexPos>())
        {
            if (!hexes.Contains(hex))
            {
                hexes.Add(hex);
            }
        }
        Generate();
    }

    public void Generate()
    {
		if (gridWidth % 2 == 0) {
			gridWidth++;
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

}
