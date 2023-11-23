using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileCopy : MonoBehaviour
{
    public GameTile nextOnPath;
    public Vector2Int tileCoordinate { get; private set; }
    int distance = int.MaxValue;

    public bool hasPath => distance != int.MaxValue;
    public TileDirection pathDirection { get; private set; }
}
