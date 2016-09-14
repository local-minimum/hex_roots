using UnityEngine;
using System.Collections.Generic;
using System;

public class RootRule : PlacementRule {

    protected override void Rule(HexCubMap map, Tile tile)
    {
        List<HexPos> meristems = (List<HexPos>) GetMeristems(map);
        List<HexPos> candidates = new List<HexPos>();
        for (int i=0, l=meristems.Count; i< l; i++)
        {
            foreach(HexPos neighbour in GetNeighbours(meristems[i].cubePos, map))
            {
                if (CountNeighboursOfTypes(neighbour.cubePos, map, TileType.Meristem, TileType.Root, TileType.RootShoot) == 1)
                {
                    candidates.Add(neighbour);
                }
            }
        }
        Emit(candidates);
    }
}
