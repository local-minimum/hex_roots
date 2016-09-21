using UnityEngine;
using System.Collections.Generic;

public class HexPos : MonoBehaviour {

    public Vector3 cubePos;
    static float viewScale = 1f;

	private Tile _occupant;

    public Tile occupant
    {
        get
        {
            return _occupant;
        }

        set
        {
            if (_occupant != null)
            {
                _occupant.transform.SetParent(null);
                _occupant.ReActivate();
            }
            
            value.transform.SetParent(transform);
            value.transform.localPosition = Vector3.zero;
            _occupant = value;
        }
    }
        
	public bool isFree {
		get {
			return occupant == null;
		}
	}

    private int _water = 10;

    public void AddWater(int amount)
    {
        _water += amount;
    }

    public int ConsumeWater(int request)
    {
        request = Mathf.Min(_water, request);
        _water -= request;
        return request;
    }

    private int _minerals;

    public int ConsumeMineral(int request)
    {
        request = Mathf.Min(_minerals, request);
        _minerals -= request;
        return request;
    }

    public void AddMineral(int amount)
    {
        _minerals += amount;
    }

    int mineralsRnd = 3;
    int mineralsN = 4;

    int SeedMinerals()
    {
        float v = 0;
        for (int i = 0; i < mineralsN; i++)
        {
            v += Random.value * mineralsRnd;
        }
        return Mathf.RoundToInt(v / mineralsN);
    }

    void Start()
    {
        name = string.Format("HexPos ({0:0.}, {1:0.}, {2:0.})", cubePos.x, cubePos.y, cubePos.z);
        _minerals = SeedMinerals();
        SetupRotationComponents();
    }    

    void Update()
    {
        UpdateRotationRender();
    }

    void OnDrawGizmosSelected()
    {
        if (enabled)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, viewScale);
        }
    }

    static float cornerAngleDelta = 2 * Mathf.PI / 6;
    [SerializeField]
    float cornerRadius = 1;

    Vector3 GetCorner(int corner) {

        return new Vector3(
            cornerRadius * Mathf.Cos(cornerAngleDelta * corner),
            cornerRadius * Mathf.Sin(cornerAngleDelta * corner),
            0);
    }

    [SerializeField]
    float rotationFrequency = 3f;

    [SerializeField, Range(0, 1)]
    float rotationCircumference = 0.8f;

    float rotationStart = 0f;

    LineRenderer lRend;

    [SerializeField]
    public bool acceptingTile = false;

    void SetupRotationComponents()
    {
        lRend = GetComponent<LineRenderer>();
        if (lRend == null)
        {
            lRend = gameObject.AddComponent<LineRenderer>();
        }
    }

    void UpdateRotationRender()
    {
        if (!acceptingTile)
        {
            lRend.enabled = false;
            return;
        }

        Vector3[] positions = GetRotationRenderPoints();
        lRend.SetVertexCount(positions.Length);           
        lRend.SetPositions(positions);
        
        if (!lRend.enabled)
        {
            lRend.enabled = true;
        }
    }

    static float twoPi = 2 * Mathf.PI;
    Vector3[] GetRotationRenderPoints()
    {
        rotationStart += twoPi * Time.deltaTime / rotationFrequency;
        rotationStart %= twoPi;

        float rotationEnd = rotationStart + twoPi * rotationCircumference;

        int start = Mathf.FloorToInt(rotationStart * 6 / twoPi);
        int stop = Mathf.CeilToInt(rotationEnd * 6 / twoPi);

        rotationEnd %= twoPi;

        Vector3 vLeft = GetCorner(start);

        List<Vector3> positions = new List<Vector3>();

        for (int left = start; left < stop; left++)
        {
            int right = left + 1;
            Vector3 vRight = GetCorner(right % 6);

            if (left == start)
            {
                positions.Add(Vector3.Lerp(vLeft, vRight, rotationStart * 6 / twoPi % 1));
            }
            else
            { 
                positions.Add(vLeft);
            }

            if (right == stop)
            {                
                positions.Add(Vector3.Lerp(vLeft, vRight, rotationEnd * 6 / twoPi % 1));
            }
            else
            {
                positions.Add(vRight);
            }

            vLeft = vRight;
        }

        return positions.ToArray();
    }

}
