using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileDirection
{
    north, east, south, west
}
public enum DirectionChange
{
    None, Right, Left, TurneAroundError
}

public static class TileDirectionExtension
{
    static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f , 90f, 0f),
        Quaternion.Euler(0f , 180f, 0f),
        Quaternion.Euler(0f , 270f, 0f)
    };
    public static Quaternion GetRotation(this TileDirection direction)
    {
        return rotations[(int)direction];
    }
    static Vector3[] halfVectors = {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };
    public static Vector3 GetHalfVector(this TileDirection direction)
    {
        return halfVectors[(int)direction];
    }
    public static float GetAngle(this TileDirection direction)
    {
        return (float)direction * 90f;
    }

    public static TileDirection GetOppositeDirection(TileDirection direction)
    {
        int index = (int)direction;
        index = (index + 2) % 4;
        return (TileDirection)index;
    }

    public static DirectionChange GetDirectionChangeTo(this TileDirection current, TileDirection next)
    {
        if (current == next)
            return DirectionChange.None;
        else if (current + 1 == next || current - 3 == next)
            return DirectionChange.Right;
        else if (current - 1 == next || current + 3 == next)
            return DirectionChange.Left;
        Debug.LogError("Train did a 180 no scope");
        return DirectionChange.TurneAroundError;
    }

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

    public static TileDirection GetDirection(int i)
    {
        if (i == 0)
            return TileDirection.north;
        else if (i == 1)
            return TileDirection.east;
        else if (i == 2)
            return TileDirection.south;
        else
            return TileDirection.west;
    }
}
