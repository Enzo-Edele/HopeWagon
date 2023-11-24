using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTileCopy : MonoBehaviour
{
    public Vector2Int tileCoordinate { get; private set; }
    [SerializeField] public Vector3 tilePosition { get; private set; }
    [SerializeField] public GameTileCopy nextOnPath;
    [SerializeField] int distance = int.MaxValue;
    [SerializeField] public TileDirection pathDirection { get; private set; }
    [SerializeField]public Vector3 exitPoint { get; private set; }

    public void SetUpTileCopy(Vector2Int coordinate, Vector3 pos, int dist, TileDirection dir, Vector3 exit)
    {
        tileCoordinate = coordinate;
        tilePosition = pos;
        distance = dist;
        pathDirection = dir;
        exitPoint = exit;
    }
    public void SetUpTileCopyNext(GameTileCopy next)
    {
        nextOnPath = next;
    }
}
