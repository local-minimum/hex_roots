using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ProximityRule : PlacementRule {

    [SerializeField, Range(1, 10)] int maxDistance;

    protected override void ApplyOn(HexCubMap map, Tile tile)
    {
        Dictionary<HexPos, List<HexPos>> positions = PlacementRule.GetMeristemProximateList(map, Occupation.Any, 1, maxDistance);
        foreach (KeyValuePair<HexPos, List<HexPos>> data in positions)
        {
            Emit(data.Value.Where(e => e.isFree).ToList(), data.Key);
        }
    }
}
