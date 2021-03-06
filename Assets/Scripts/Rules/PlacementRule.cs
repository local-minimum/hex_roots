﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum Occupation { Occupied, Free, Any, LastFree};
public enum Neighbour { A, B, C, D, E, F }

public delegate void PossiblePlacements(List<HexPos> positions, HexPos anchor);


public abstract class PlacementRule : MonoBehaviour {

    public static event PossiblePlacements OnPossiblePlacement;

    [SerializeField]
    protected TileType ruleFor;

    public TileType RuleFor
    {
        get
        {
            return ruleFor;
        }
    }

    protected void Emit(List<HexPos> positions)
    {
        if (OnPossiblePlacement != null)
        {
            OnPossiblePlacement(positions, null);
        }
    }


    protected void Emit(List<HexPos> positions, HexPos anchor)
    {
        if (OnPossiblePlacement != null)
        {
            OnPossiblePlacement(positions, anchor);
        }
    }    

    static public bool isMeristemPosition(HexCubMap map, Vector3 pos) {
		Tile tile = map.GetHexPos (pos).occupant;
		if (tile) {
			return tile.tileType == TileType.Meristem;
		} else {
			return false;
		}
	}

    public static IEnumerable<HexPos> GetMeristems(HexCubMap map)
    {
        foreach(HexPos pos in map.OccupiedPositions())
        {
            if (pos.occupant.tileType == TileType.Meristem)
            {
                yield return pos;
            }
        }
    }

    public static List<HexPos> RootNeighboursAtMeristemDistance(HexCubMap map, int requiredDist)
    {        
        List<HexPos> meristems = GetMeristems(map).ToList();
        List<HexPos> valid = new List<HexPos>();
        foreach(HexPos free in map.FreePositions())
        {
            if (BordersRoot(free, map) && minDistance(free, meristems) >= requiredDist)
            {                
                valid.Add(free);
            }
        }
        return valid;
    }
    
    public static bool BordersRoot(HexPos pos, HexCubMap map)
    {
        foreach (HexPos neighbour in GetNeighbours(pos.cubePos, map))
        {
            if (neighbour != null && !neighbour.isFree && neighbour.occupant.tileType == TileType.Root)
            {
                return true;
            }
        }
        return false;
    }

    public static Dictionary<HexPos, List<HexPos>> GetMeristemProximateList(HexCubMap map, Occupation occupation, int min = 0, int max = 2)
    {
        Dictionary<HexPos, List<HexPos>> dict = new Dictionary<HexPos, List<HexPos>>();
        bool isLastFreeMode = occupation == Occupation.LastFree;
        Occupation floodOccupation = isLastFreeMode ? Occupation.Occupied : occupation;

        //Will actually look one extra in all cases in this mode.
        if (isLastFreeMode)
        {
            max--;
        }

        foreach(HexPos meristem in GetMeristems(map))
        {
            dict[meristem] = new List<HexPos>();
            List<List<HexPos>> flood = GetFloodFillUntil(meristem, max, map, floodOccupation);
            for (int i=min; i<max; i++)
            {
                if (isLastFreeMode)
                {
                    for (int j = 0, l = flood[i].Count; j < l; j++) {
                        foreach (HexPos neighbour in GetNeighbours(flood[i][j].cubePos, map))
                        {
                            if (neighbour.isFree && !dict[meristem].Contains(neighbour))
                            {
                                dict[meristem].Add(neighbour);
                            }
                        }
                    }
                } else
                {
                    dict[meristem].AddRange(flood[i]);
                }
            }
        }

        return dict;
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

    public static List<HexPos> GetPath(HexPos start, HexPos end, HexCubMap map, Occupation criteria)
    {
        List<HexPos> path = new List<HexPos>();
        List<List<HexPos>> floodFill = GetFloodFillUntil(start, end, map, criteria);
        int n = floodFill.Count;

        //If no end in sight return empty path
        if (n== 0 || !floodFill[n].Contains(end))
        {
            return path;
        }

        path.Add(end);
        n--;
        while (n > 0)
        {
            HexPos nextPos = FirstNeighbour(path[path.Count - 1], floodFill[n]);
            if (nextPos == null)
            {
                path.Clear();
                return path;
            }
            n--;
        }
        path.Add(start);

        path.Reverse();
        return path;
    }

    public static HexPos FirstNeighbour(HexPos refPos, List<HexPos> candidates)
    {
        for (int i=0, l=candidates.Count; i<l; i++)
        {
            if (distance(refPos.cubePos, candidates[i].cubePos) == 1)
            {
                return candidates[i];
            }
        }
        return null;
    }

	public static List<List<HexPos>> GetFloodFillUntil(HexPos start, HexPos end, HexCubMap map, Occupation criteria){
		
		HashSet<HexPos> visited = new HashSet<HexPos>();
		List<List<HexPos>> fringes = new List<List<HexPos>>();
		fringes.Add(new List<HexPos>() {start});

		int k = 0;
		while (true) {
			fringes.Add (new List<HexPos> ());
			k++;
			for (int i=0, l=fringes[k - 1].Count; i<l; i++) {
				foreach (HexPos neighbour in GetNeighbours(fringes[k - 1][i].cubePos, map)) {
					if (visited.Contains(neighbour)) {
						continue;
					}
					visited.Add (neighbour);
                    if (criteria == Occupation.Any || criteria == Occupation.Free && neighbour.isFree || criteria == Occupation.Occupied && !neighbour.isFree || neighbour == end)
                    {
                        fringes[k].Add(neighbour);
                    }
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

    public static List<List<HexPos>> GetFloodFillUntil(HexPos start, int end, HexCubMap map, Occupation criteria)
    {

        HashSet<HexPos> visited = new HashSet<HexPos>();
        List<List<HexPos>> fringes = new List<List<HexPos>>();
        fringes.Add(new List<HexPos>() { start });

        int k = 0;
        while (k < end)
        {
            fringes.Add(new List<HexPos>());
            k++;
            for (int i = 0, l = fringes[k - 1].Count; i < l; i++)
            {
                foreach (HexPos neighbour in GetNeighbours(fringes[k - 1][i].cubePos, map))
                {
                    if (visited.Contains(neighbour))
                    {
                        continue;
                    }
                    visited.Add(neighbour);
                    if (criteria == Occupation.Any || criteria == Occupation.Free && neighbour.isFree || criteria == Occupation.Occupied && !neighbour.isFree)
                    {
                        fringes[k].Add(neighbour);
                    }
                }
            }
            if (fringes[k].Count == 0)
            {
                fringes.RemoveAt(k);
                return fringes;
            }
        }
        return fringes;
    }

	public static bool proximate(HexPos a, HexPos b, HexCubMap map) {
		return distance (a.cubePos, b.cubePos) == 2;
	}

	public static bool proximate(Vector3 a, Vector3 b) {
		return distance (a, b) == 2;
	}

    public static int minDistance(HexPos pos, List<HexPos> others)
    {
        int dist = -1;
        Vector3 a = pos.cubePos;
        for (int i = 0, l=others.Count; i< l; i++)
        {
            int curDist = distance(a, others[i].cubePos);
            if (dist < 0 || curDist < dist)
            {
                dist = curDist;
            }
        }
        return dist;
    }

	public static int distance(Vector3 a, Vector3 b) {
		return Mathf.RoundToInt((Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2);
	}

    static Vector3[] directions = new Vector3[] {
        new Vector3(+1, -1,  0), new Vector3(+1,  0, -1), new Vector3( 0, +1, -1),
        new Vector3(-1, +1,  0), new Vector3(-1,  0, +1), new Vector3( 0, -1, +1)
    };

    public static Vector3 cubeDirection(Neighbour neighbour)
    {
        return directions[(int)neighbour];
    }

    public static IEnumerable<HexPos> GetNeighbours(Vector3 pos, HexCubMap map)
    {
        foreach (Vector3 dir in directions)
        {
            HexPos hex = map.GetHexPos(dir + pos);
            if (hex != null && hex.enabled)
            {
                yield return hex;
            }
        }
    }

    public static int CountNeighboursOfTypes(Vector3 pos, HexCubMap map, params TileType[] types)
    {
        int n = 0;
        
        foreach (HexPos neighbour in GetNeighbours(pos, map))
        {
            if (neighbour.isFree)
            {
                continue;
            }

            for (int i=0; i<types.Length; i++)
            {
                if (types[i] == neighbour.occupant.tileType)
                {
                    n++;
                    break;
                }
            }    
        }
        return n;
    }

    void OnEnable()
    {
        Tile.OnTileEvent += Tile_OnTileEvent;
    }

    void OnDisable()
    {
        Tile.OnTileEvent -= Tile_OnTileEvent;
    }

    void OnDestroy()
    {
        Tile.OnTileEvent -= Tile_OnTileEvent;
    }

    private void Tile_OnTileEvent(Tile tile, TileEventType eventType)
    {
        if (eventType == TileEventType.HandHovered && tile.tileType == ruleFor)
        {
            ApplyOn(HexCubMap.current, tile);
        }
    }

    protected abstract void ApplyOn(HexCubMap map, Tile tile);
}
