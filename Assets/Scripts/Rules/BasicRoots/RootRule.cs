using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RootRule : PlacementRule {

    protected override void ApplyOn(HexCubMap map, Tile tile)
    {
        List<HexPos> meristems = GetMeristems(map).ToList();
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
