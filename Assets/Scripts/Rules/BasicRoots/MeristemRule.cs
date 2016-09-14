using UnityEngine;
using System.Collections.Generic;

public class MeristemRule : PlacementRule {

    [SerializeField, Range(1, 10)]
    int minDistToOther = 2;

    protected override void Rule(HexCubMap map, Tile tile)
    {
        List<HexPos> candidates = RootNeighboursAtMeristemDistance(map, minDistToOther);
        Emit(candidates);
    }
}
