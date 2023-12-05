using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GridBoard : MonoBehaviour
{
    Vector2Int size;

    [SerializeField] int offset = 1;

    [SerializeField] Transform ground;

    [SerializeField] GameTile tilePrefab = default;

    [SerializeField] GameTile[] tiles;
    public List<Station> stationList = new List<Station>(); //[SerializeField]

    //créer une classe network 
    //creation au moment ou on pose une station et add case de la station
    //fonction add 
    //  on ajoute le nouveau et si station on l'ajoute a une liste de station du network
    //et oncheck si voisin sans network ou network diff (pour chaque voisin)
    //      voisin vide : on l'ajoute et répéte opération
    //      network voisin : on copie le voisin, sa liste de station
    //          pour chaque station de n1 on ajoute toutes les dest n2 et ajoute les dest de n2 a celle de n1
    //          pour chaque station de n2 on ajoute toutes les dest de n1
    //          on supprime n2
    //          on ajoute les voisin de rail vide au network
    //fonction merge
    //
    List<Network> networkList = new List<Network>();

    public GameObject network;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    public static GridBoard Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            foreach (GameTile tile in tiles)
                tile.ShowNetwork();
        if (Input.GetKeyDown(KeyCode.B))
            foreach (GameTile tile in tiles)
                tile.HideNetwork();
    }
    //Get all station connected to startStation
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
            if (!tile.HasRail)
                tile.Paint(new Color(26/255f, 180/255f, 27/255f));
        }

        foreach (GameTile tile in tiles)
        {
            if (tile.hasPath)
                tile.ShowPath();
            else if(tile.HasRail)
            tile.Paint(new Color(26/255f, 180/255f, 27/255f));
            if (tile != startStation && tile.hasPath && tile.HasStation) {
                arrivalStation.Add(tile.station);
                tile.station.AddDestination(startStation.station);
                tile.Paint(Color.green);
            }
        }
        return arrivalStation;
    }

    //créer une copie des gametile chemin qui conserve leur valeur de pathfind
    public Queue<GameTile> Pathfinding(GameTile destination, GameTile start)
    {
        Queue<GameTile> path = new Queue<GameTile>();
        foreach (GameTile tile in tiles)
        {
            tile.ClearPath();
            if(tile.HasRail)
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
            tilePath.ShowPath();
            tilePath = tilePath.nextOnPath;
            path.Enqueue(tilePath);
        }
        return path;

    }

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
        position = transform.InverseTransformPoint(position);
        Vector2Int coordinates = new Vector2Int((int)position.x / offset, (int)position.z / offset);
        return GetTile(coordinates);
    }
    public GameTile GetTile(Vector2Int coordinates)
    {
        return tiles[coordinates.y * size.x + coordinates.x];
    }
    public GameTile GetTile(GameTile tile)
    {
        return tile;
    }
    public Network AddNetwork(Network network)
    {
        for (int i = 0; i < networkList.Count; i++)
            if (networkList[i] == network)
                return null;
        networkList.Add(network);
        return network;
    }
    public void RemoveNetwork(Network network)
    {
        for (int i = 0; i < networkList.Count; i++)
            if (network == networkList[i])
                networkList.RemoveAt(i);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(size.x);
        writer.Write(size.y);

        for (int i = 0; i < tiles.Length; i++)
            tiles[i].Save(writer);
    }
    public void Load(BinaryReader reader, int header)
    {
        int x = 20, z = 15;
        x = reader.ReadInt32();
        z = reader.ReadInt32();

        for (int i = 0; i < tiles.Length; i++)
            tiles[i].Load(reader, header);
    }
}
