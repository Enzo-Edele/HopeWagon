using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameTileStruct
{
    public Vector2Int tileCoordinate; 
    [SerializeField] public Vector3 tilePosition; 
    [SerializeField] int distance;
    [SerializeField] public TileDirection pathDirection { get; private set; }
    [SerializeField] public Vector3 exitPoint { get; private set; }

    public GameTileStruct(Vector2Int coord, Vector3 pos, int dist, TileDirection direction, Vector3 exit)
    {
        tileCoordinate = coord;
        tilePosition = pos;
        distance = dist;
        pathDirection = direction;
        exitPoint = exit;
    }
}
