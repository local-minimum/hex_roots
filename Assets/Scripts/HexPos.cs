using UnityEngine;
using System.Collections;

public class HexPos : MonoBehaviour {

    public Vector3 cubePos;
    static float viewScale = 1f;

    void OnDrawGizmosSelected()
    {
        if (enabled)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, viewScale);
        }
    }
}
