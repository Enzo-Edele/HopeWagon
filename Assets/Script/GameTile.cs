using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    public GameTile[] neighbor = new GameTile[4];
    public GameTile nextOnPath;
    public Vector2Int tileCoordinate { get; private set; }

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer railSpriteRenderer;
    [SerializeField] SpriteRenderer buildingSpriteRenderer;
    [SerializeField] SpriteRenderer stationSpriteRenderer;

    [SerializeField] Transform rail;

    int distance = int.MaxValue;

    public bool hasPath => distance != int.MaxValue;
    public TileDirection pathDirection { get; private set; }

    int nbRailNeighbor;
    bool hasRail;
    public bool HasRail
    {
        get { return hasRail; }
        set
        {
            if (value != hasRail)
            {
                hasRail = value;
                for (int i = 0; i < neighbor.Length; i++)
                    if (neighbor[i]) neighbor[i].UpdateRailNeighbor(value);
                UpdateSpriteRail();
                //Debug.Log("change rail");
            }
        }
    }

    bool hasIndustry;
    public bool HasIndustry
    {
        get { return hasIndustry; }
        set
        {
            if (value != hasIndustry)
            {
                hasIndustry = value;
                if (value) buildingSpriteRenderer.sprite = GameManager.Instance.industrySprite;
                else buildingSpriteRenderer.sprite = null;
            }
        }
    }

    public Station station;
    bool hasStation;
    public bool HasStation
    {
        get { return hasStation; }
        set
        {
            if (value != hasStation)
            {
                hasStation = value;
                HasRail = value;
                if (hasStation)
                {
                    station = Instantiate(GameManager.Instance.station, this.transform).GetComponent<Station>();
                    station.SetTile(this);
                }
                else
                    Destroy(station);
                if (value) stationSpriteRenderer.sprite = GameManager.Instance.stationSprite;
                else stationSpriteRenderer.sprite = null;
            }
        }
    }

    public void SetCoordinate(Vector2Int newCoord)
    {
        tileCoordinate = newCoord;
    }

    public GameTile GetNeighbor(TileDirection direction)
    {
        return neighbor[(int)direction];
    }

    public void MakeEastWestNeighbor(GameTile east, GameTile west)
    {
        east.neighbor[3] = west;
        west.neighbor[1] = east;
    }

    public void MakeNorthSouthNeighbor(GameTile north, GameTile south)
    {
        north.neighbor[2] = south;
        south.neighbor[0] = north;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
    }

    GameTile GrowPathToAllRail(GameTile neighbor)
    {
        if (neighbor == null || !neighbor.hasRail || neighbor.hasPath)
            return null;
        neighbor.distance = distance + 1;
        return neighbor;
    }
    public GameTile GrowPathToAllRailNorth() => GrowPathToAllRail(neighbor[0]);
    public GameTile GrowPathToAllRailEast() => GrowPathToAllRail(neighbor[1]);
    public GameTile GrowPathToAllRailSouth() => GrowPathToAllRail(neighbor[2]);
    public GameTile GrowPathToAllRailWest() => GrowPathToAllRail(neighbor[3]);

    GameTile GrowPathToPathfinding(GameTile neighbor, TileDirection direction)
    {
        if (neighbor == null || !neighbor.hasRail || neighbor.hasPath)
            return null;
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.pathDirection = direction;
        return neighbor;
    }
    public GameTile GrowPathToPathfindingNorth() => GrowPathToPathfinding(neighbor[0], TileDirection.south);
    public GameTile GrowPathToPathfindingEast() => GrowPathToPathfinding(neighbor[1], TileDirection.west);
    public GameTile GrowPathToPathfindingSouth() => GrowPathToPathfinding(neighbor[2], TileDirection.north);
    public GameTile GrowPathToPathfindingWest() => GrowPathToPathfinding(neighbor[3], TileDirection.east);

    //use for debbug only
    public void ShowPath()
    {
        if (distance != int.MaxValue)
            Paint(Color.blue);
    }
    public void HidePath()
    {
        Paint(Color.white);
    }

    public void UpdateRailNeighbor(bool hasRail)
    {
        if (hasRail)
            nbRailNeighbor++;
        else
            nbRailNeighbor--;

        if (HasRail)
            UpdateSpriteRail();
    }

    void UpdateSpriteRail()
    {
        float rotation;
        switch (nbRailNeighbor)
        {
            case 1:
                {
                    railSpriteRenderer.sprite = GameManager.Instance.railSprites[4];
                    rotation = 180f;
                    for (int i = 0; i < neighbor.Length; i++)
                    {
                        if (neighbor[i] && neighbor[i].hasRail)
                            rail.rotation = Quaternion.Euler(0f, 0f, rotation);
                        rotation -= 90f;
                    }
                    break;
                }
            case 2:
                {
                    rotation = 0f;
                    for (int i = 0; i < neighbor.Length; i++)
                    {
                        if (neighbor[i] && neighbor[(i + 2) % 4] &&
                        i < 2 && neighbor[i].hasRail && neighbor[(i + 2) % 4].hasRail)
                        {
                            railSpriteRenderer.sprite = GameManager.Instance.railSprites[0];
                            rail.rotation = Quaternion.Euler(0f, 0f, rotation);
                        }
                        if (neighbor[i] && neighbor[(i + 1) % 4] &&
                        neighbor[i].hasRail && neighbor[(i + 1) % 4].hasRail)
                        {
                            railSpriteRenderer.sprite = GameManager.Instance.railSprites[1];
                            rail.rotation = Quaternion.Euler(0f, 0f, rotation);
                        }
                        rotation -= 90f;
                    }
                    break;
                }
            case 3:
                {
                    rotation = 0f;
                    railSpriteRenderer.sprite = GameManager.Instance.railSprites[2];
                    for (int i = 0; i < neighbor.Length; i++)
                    {
                        if ((neighbor[i] && !neighbor[i].hasRail) || neighbor[i] == null)
                        {
                            rail.rotation = Quaternion.Euler(0f, 0f, rotation);
                        }
                        rotation -= 90f;
                    }
                    break;
                }
            case 4:
                {
                    rail.rotation = Quaternion.Euler(0f, 0f, 0f);
                    railSpriteRenderer.sprite = GameManager.Instance.railSprites[3];
                    break;
                }
            default:
                railSpriteRenderer.sprite = GameManager.Instance.railSprites[5];
                break;
        }
        if (!HasRail)
            railSpriteRenderer.sprite = null;
    }

    public void Paint(Color color)
    {
        spriteRenderer.color = color;
    }
}
