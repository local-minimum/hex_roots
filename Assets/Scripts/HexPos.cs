using UnityEngine;
using System.Collections;

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
            if (value != null)
            {
                value.transform.SetParent(null);
                value.ReActivate();
            }

            value.transform.SetParent(transform);
            value.transform.localPosition = Vector3.zero;
            _occupant = value;
            value.Place();
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

    public int minerals;

    int mineralsRnd = 3;
    int mineralsN = 4;

    void Start()
    {
        minerals = SeedMinerals();
    }

    int SeedMinerals()
    {
        float v = 0;
        for (int i = 0; i < mineralsN; i++)
        {
            v += Random.value * mineralsRnd;
        }
        return Mathf.RoundToInt(v / mineralsN);
    }

    void OnDrawGizmosSelected()
    {
        if (enabled)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, viewScale);
        }
    }
}
