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
    public int tileCount;
    public List<Station> stationList = new List<Station>(); 
    public List<TrainRoute> routeList = new List<TrainRoute>(); 

    List<Network> networkList = new List<Network>();
    public int networkNumber; //fonction pour réatribuer les numéros quand liste remove
    public bool stateWorldUI = true;

    public GameObject network;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    public static GridBoard Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Initialize(Vector2Int size)
    {
        tileCount = size.x * size.y;
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
                tile.index = i;
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
        /*if (Input.GetKeyDown(KeyCode.N))
            foreach (GameTile tile in tiles)
                tile.ShowNetwork();
        if (Input.GetKeyDown(KeyCode.B))
            foreach (GameTile tile in tiles)
                tile.HideNetwork();
        if (Input.GetKeyDown(KeyCode.M))
            PaintStation();*/
        if (Input.GetKeyDown(KeyCode.R))
            ShowHideWorldUI();
        /*if (Input.GetKeyDown(KeyCode.F))
            PaintPollution();*/
    }

    public void PaintAllTile(Color color)
    {
        foreach (GameTile tile in tiles)
        {
            tile.Paint(color);
        }
    }
    public void PaintStation()
    {
        foreach (GameTile tile in tiles)
        {
            if(tile.HasStation)
                tile.Paint(Color.red);
        }
    }
    public void ShowHideWorldUI()
    {
        stateWorldUI = !stateWorldUI;
        foreach (GameTile tile in tiles)
        {
            tile.ShowHideUI(stateWorldUI);
        }
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

        /*foreach (GameTile tile in tiles)
        {
            tile.HidePath();
            if (!tile.HasRail)
                tile.Paint(GameManager.Instance.colorArrayTile[1]);
        }*/

        foreach (GameTile tile in tiles)
        {
            /*if (tile.hasPath) //debbug
                tile.ShowPath();
            if(tile.HasRail) //else
                tile.Paint(GameManager.Instance.colorArrayTile[1]);*/
            if (tile != startStation && tile.hasPath && tile.HasStation) {
                arrivalStation.Add(tile.station);
                tile.station.AddDestination(startStation.station);
                //tile.Paint(Color.green);
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
            //if(tile.HasRail)
                //tile.HidePath();
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
            //tilePath.ShowPath();
            tilePath = tilePath.nextOnPath;
            path.Enqueue(tilePath);
        }
        return path;

    }

    public void UpdateNetworks()
    {
        for(int i = 0; i < networkList.Count; i++)
        {
            networkList[i].ReInitialize(i + 1);
        }
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
    public GameTile GetTile(int index)
    {
        return (tiles[index]);
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
    public void AddStation(Station station)
    {
        for (int i = 0; i < stationList.Count; i++)
            if (stationList[i] == station)
                return;
        stationList.Add(station);
    }
    public void RemoveStation(Station station)
    {
        for (int i = 0; i < stationList.Count; i++)
            if (station == stationList[i])
                stationList.RemoveAt(i);
    }
    public Station GetStation(string stationName)
    {
        for (int i = 0; i < stationList.Count; i++)
            if (stationName == stationList[i].name)
                return stationList[i];
        return null;
    }
    public void RefreshStation()
    {
        for(int i = 0; i < networkList.Count; i++)
        {
            for(int j = 0; j < networkList[i].networkStationList.Count; j++)
            {
                networkList[i].networkStationList[j].destinationList.Clear();
                networkList[i].networkStationList[j].destinationNameList.Clear();
            }
            networkList[i].RelinkStation();
        }
    }
    public void AddRoute(TrainRoute trainRoute)
    {
        for (int i = 0; i < routeList.Count; i++)
            if (routeList[i] == trainRoute)
                return;
        routeList.Add(trainRoute);
    }
    public void RemoveRoute(TrainRoute trainRoute)
    {
        for (int i = 0; i < routeList.Count; i++)
            if (trainRoute == routeList[i])
                routeList.RemoveAt(i);
    }
    public void ClearRoute()
    {
        for (int i = 0; i < routeList.Count; i++)
            Destroy(routeList[i].gameObject);
        routeList.Clear();
    }

    public void RefreshPollutionMax()
    {
        foreach (GameTile tile in tiles)
        {
            if(tile.HasIndustry)
                tile.SetMaxPollution((int)tile.industry.pollutionLevel);
            if(tile.HasPollutedIndustry)
                tile.SetMaxPollution((int)tile.pollutedIndustry.pollutionLevel);
            if (tile.HasCleaner)
                tile.LowerMinPollution((int)tile.cleaner.pollutionLevel);
        }
    }
    void PaintPollution()
    {
        foreach(GameTile tile in tiles)
        {
            tile.PaintPollution();
        }
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(size.x);
        writer.Write(size.y);

        for (int i = 0; i < tiles.Length; i++)
            tiles[i].Save(writer);
        
        writer.Write(routeList.Count);
        for(int i = 0; i < routeList.Count; i++)
        {
            //save number of dest
            writer.Write(routeList[i].destinationArray.Count);
            //for dest save name;
            for (int j = 0; j < routeList[i].destinationArray.Count; j++)
                writer.Write(routeList[i].destinationArray[j].nameStation);
            //if loop or back and forth
        }

        //savecameraPos
    }
    public void Load(BinaryReader reader, int header)
    {
        for(int i = 0; i < routeList.Count; i++)
        {
            routeList[i].LoadingStopRoute();
        }
        ClearRoute();

        int x = 20, z = 15;
        x = reader.ReadInt32();
        z = reader.ReadInt32();
        
        for (int i = 0; i < tiles.Length; i++)
            tiles[i].Load(reader, header);
        RefreshStation();
        if (header >= 3)
        {
            int routeListCount = reader.ReadInt32();
            for (int i = 0; i < routeListCount; i++)
            {
                int destinationCount = reader.ReadInt32();
                Station departure = GetStation(reader.ReadString());
                List<Station> destArray = new List<Station>();
                for (int j = 0; j < destinationCount - 1; j++) { }
                destArray.Add(GetStation(reader.ReadString()));
                //if loop or back and forth

                //create route
                departure.CreateRoute(destArray[0]);
            }
        }
        //setCameraPos
    }
}
