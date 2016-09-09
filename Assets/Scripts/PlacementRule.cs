using UnityEngine;
using System.Collections.Generic;

public abstract class PlacementRule : MonoBehaviour {

	Vector3 PermissableCubePosition (HexCubMap map);

	static public bool isMeristemPosition(HexCubMap map, Vector3 pos) {
		Tile tile = map.GetTile (pos);
		if (tile) {
			return tile.tileType == TileType.Meristem;
		} else {
			return false;
		}
	}

	public bool sharedNeighbour(Vector3	pos1, Vector3 pos2) {
		return false;
	}
}
