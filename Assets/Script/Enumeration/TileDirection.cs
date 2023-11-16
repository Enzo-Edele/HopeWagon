using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileDirection
{
    north, east, south, west
}

public static class TileDirectionExtension
{
    public static TileDirection CoorDirection(Vector2Int first, Vector2Int second)
    {
        int xDiff = second.x - first.x;
        int yDiff = second.y - first.y;

        if (xDiff == 1)
            return TileDirection.east;
        if (xDiff == -1)
            return TileDirection.west;
        if (yDiff == 1)
            return TileDirection.north;
        if (yDiff == -1)
            return TileDirection.south;

        Debug.LogWarning("default direction return not neighbour cell compared !");
        return TileDirection.north;
    }
}
