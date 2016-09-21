using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexCubMap))]
class HexGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        /*
        if (GUILayout.Button("Generate")) {

            (target as HexCubMap).Generate();
        }
        */        
    }

}