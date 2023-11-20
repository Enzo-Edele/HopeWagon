using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoard : MonoBehaviour
{
    [SerializeField] Vector2Int size;

    [SerializeField] int offset = 1;

    [SerializeField] Transform ground;

    [SerializeField] GameTile tilePrefab = default;

    GameTile[] tiles;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    public void Initialize(Vector2Int size)
    {
        this.size = size;
        ground.localScale = new Vector3(size.x, 0.05f ,size.y);

        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        tiles = new GameTile[size.x * size.y];
        for (int i = 0, z = 0; z < size.y; z++) {
            for (int x = 0; x < size.x; x++, i++) {
                GameTile tile = tiles[i] = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0, z - offset.y);
                tile.name = "Tile : " + x + ", " + z;
                tile.SetCoordinate(new Vector2Int(x, z));
                if (x > 0) {
                    tile.MakeEastWestNeighbor(tile, tiles[i - 1]);
                }
                if (z > 0) {
                    tile.MakeNorthSouthNeighbor(tile, tiles[i - size.x]);
                }
            }
        }
    }

    public List<Station> GetStationInNetwork(GameTile startStation)
    {
        foreach (GameTile tile in tiles)
        {
            tile.ClearPath();
        }
        List<Station> arrivalStation = new List<Station>();
        searchFrontier.Enqueue(startStation);
        while (searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            if (tile != null)
            {
                searchFrontier.Enqueue(tile.GrowPathToAllRailNorth());
                searchFrontier.Enqueue(tile.GrowPathToAllRailEast());
                searchFrontier.Enqueue(tile.GrowPathToAllRailSouth());
                searchFrontier.Enqueue(tile.GrowPathToAllRailWest());
            }
        }

        foreach (GameTile tile in tiles)
        {
            tile.HidePath();
        }

        foreach (GameTile tile in tiles)
        {
            if (tile.hasPath)
            {
                tile.ShowPath();
            }
            if (tile != startStation && tile.hasPath && tile.HasStation)
            {
                arrivalStation.Add(tile.station);
                tile.station.AddDestination(startStation.station);
                tile.Paint(Color.green);
            }
        }
        //remplacer par UI
        string debbug;
        debbug = "Stattion : ";
        for (int i = 0; i < arrivalStation.Count; i++)
            debbug += arrivalStation[i].name + ", ";
        Debug.Log(debbug);
        //
        return arrivalStation;
    }

    public Queue<GameTile> Pathfinding(GameTile destination, GameTile start)
    {
        Queue<GameTile> path = new Queue<GameTile>();
        foreach (GameTile tile in tiles)
        {
            tile.ClearPath();
            tile.HidePath();
        }
        searchFrontier.Enqueue(destination);
        destination.BecomeDestination();
        while (searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            if (tile != null)
            {
                searchFrontier.Enqueue(tile.GrowPathToPathfindingNorth());
                searchFrontier.Enqueue(tile.GrowPathToPathfindingEast());
                searchFrontier.Enqueue(tile.GrowPathToPathfindingSouth());
                searchFrontier.Enqueue(tile.GrowPathToPathfindingWest());
            }
        }
        path.Enqueue(start);
        GameTile tilePath = start;
        while (tilePath != destination)
        {
            tilePath = tilePath.nextOnPath;
            path.Enqueue(tilePath);
        }
        while (path.Count > 0)
        {
            path.Dequeue().ShowPath();
        }
        return path;

    }
    //pathfinding returning bool
    //add the destinations tile(s) in list searchfrontier
    //verif searchfrontier > 0

    //tant que searchfrontier > 0
    //dequeu
    //si tile pas null : call fonction in tile to check if tile is good
    //faire les tiles et les isalternative
    //for each tile with no path do nothing
    //showpath for tile with path
    //return true

    //faire une version qui renvoie les stations atteintes
    //add la station 
    //grow path depuis la station
    //renvoyer les stations atteintes

    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            //Debug.Log("coord hit : " + x + ", " + y);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
                return tiles[x + y * size.x];
        }
        return null;
    }
    public GameTile GetTile(Vector3 position)
    {
        //position = transform.InverseTransformPoint(position);
        Vector2Int coordinates = new Vector2Int((int)position.x / offset, (int)position.z / offset);
        return GetTile(coordinates);
    }
    public GameTile GetTile(Vector2Int coordinates)
    {
        return tiles[coordinates.x * coordinates.y];
    }
    public GameTile GetTile(GameTile tile)
    {
        return tile;
    }
}
