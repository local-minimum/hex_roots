using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor {

    public override void OnInspectorGUI()
    {
        Tile me = target as Tile;

        EditorGUILayout.HelpBox(
            string.Format("Type:\t{1}\nStatus:\t{0}", me.Status, me.tileType),
            MessageType.Info);

        base.OnInspectorGUI();

    }
}
