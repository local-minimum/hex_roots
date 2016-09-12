using UnityEngine;
using System.Collections.Generic;

public abstract class PlacementRule : MonoBehaviour {

	static public bool isMeristemPosition(HexCubMap map, Vector3 pos) {
		Tile tile = map.GetTile (pos).occupant;
		if (tile) {
			return tile.tileType == TileType.Meristem;
		} else {
			return false;
		}
	}

	protected List<HexPos> PermissableHexPositions(
		HexCubMap map,
		int maxDistance, 
		System.Func<HexPos, HexPos, HexCubMap, bool> evalFunc)
	{
		HexPos[] free = (HexPos[]) map.FreePositions ();
		HexPos[] occupied = (HexPos[]) map.OccupiedPositions ();
		List<HexPos> permissable = new List<HexPos> ();

		for (int i = 0; i < free.Length; i++) {
			for (int j = 0; j < occupied.Length; i++) {
				if (evalFunc (free [i], occupied [j], map)) {
					permissable.Add (free [i]);
					break;
				}
			}
		}
		return permissable;
	}

	public static List<List<HexPos>> distanceUntil(HexPos start, HexPos end, HexCubMap map){
		
		HashSet<HexPos> visited = new HashSet<HexPos>();
		List<List<HexPos>> fringes = new List<List<HexPos>>();
		fringes.Add(new List<HexPos>() {start});

		int k = 0;
		while (true) {
			fringes.Add (new List<HexPos> ());
			k++;
			for (int i=0, l=fringes[k - 1].Count; i<l; i++) {
				foreach (HexPos neighbour in map.GetNeighbours(fringes[k - 1][i].cubePos)) {
					if (visited.Contains(neighbour)) {
						continue;
					}
					visited.Add (neighbour);
					fringes [k].Add (neighbour);
					if (neighbour == end) {
						return fringes;
					}
				}
			}
			if (fringes [k].Count == 0) {
				fringes.RemoveAt (k);
				return fringes;
			}
		}
	}

	//TODO: add reverse traversal from distanceuntil end towards start.
	//this is the shortest cube pos path

	public static bool nextNeighbours(HexPos a, HexPos b, HexCubMap map) {
		if (proximate (a, b, map)) {

		}
		return false;
	}

	public static bool proximate(HexPos a, HexPos b, HexCubMap map) {
		return distance (a.cubePos, b.cubePos) == 2;
	}

	public static bool proximate(Vector3 a, Vector3 b) {
		return distance (a, b) == 2;
	}

	public static int distance(Vector3 a, Vector3 b) {
		return Mathf.RoundToInt((Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2);
	}
}
