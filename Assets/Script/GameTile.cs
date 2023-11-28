using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    public GameTile[] neighbor = new GameTile[4];
    public Vector2Int tileCoordinate { get; private set; }
    public int distance = int.MaxValue;
    public bool hasPath => distance != int.MaxValue;
    public Vector3 exitPoint { get; private set; }
    public TileDirection pathDirection { get; private set; }
    public GameTile nextOnPath;

    [SerializeField] GameObject ground;
    [SerializeField] Material groundMat;
    GameObject rail;
    GameObject buildingPrefab;
    GameObject stationPrefab;

    Transform railTransform;
    Material railMat;

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
                if (HasStation)
                    hasRail = true;
                UpdateRail();
                CheckNeighborNetwork();
            }
        }
    }
    [SerializeField] Network network;
    public Network Network { 
        get { return network; }
        set { 
            if (value != network) {
                network = value;
                if(network != null) {
                    SetNetworkNeighbor(network);
                }
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
                if(value && CanBuild()) {
                    hasStation = value;
                    HasRail = hasStation;
                    SpawnStation();
                }
                else if (!value) {
                    hasStation = value;
                    //HasRail = hasStation;
                    network.RemoveStation(station);
                    DestroyStation();
                }
            }
        }
    }

    public Industry industry { get; private set; }
    bool hasIndustry;
    public bool HasIndustry
    {
        get { return hasIndustry; }
        set
        {
            if (value != hasIndustry)
            {
                if (value && CanBuild()) {
                    hasIndustry = value;
                    SpawnFactory();
                }
                else if (!value) {
                    hasIndustry = value;
                    DestroyFactory();
                }
            }
        }
    }

    //couvrir le cas spéciaux des rails et stations
    bool CanBuild()
    {
        return !(hasStation || hasIndustry);
    }
    private void Start()
    {
        groundMat = ground.GetComponent<MeshRenderer>().material;
    }
    public void SetCoordinate(Vector2Int newCoord)
    {
        tileCoordinate = newCoord;
    }

    public GameTile GetNeighbor(TileDirection direction)
    {
        return neighbor[(int)direction];
    }

    public TileDirection GetNeighborDirection(GameTile tile)
    {
        for(int i = 0; i < 4; i++)
        {
            if (tile == neighbor[i])  //maybe checker le nom si toutes les cases sont "égales"
                return (TileDirection)i;
        }
        return 0;
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
        return neighbor.hasRail == true ? neighbor : null;
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
        neighbor.exitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.pathDirection = direction;
        return neighbor.hasRail == true ? neighbor : null;
    }
    public GameTile GrowPathToPathfindingNorth() => GrowPathToPathfinding(neighbor[0], TileDirection.south);
    public GameTile GrowPathToPathfindingEast() => GrowPathToPathfinding(neighbor[1], TileDirection.west);
    public GameTile GrowPathToPathfindingSouth() => GrowPathToPathfinding(neighbor[2], TileDirection.north);
    public GameTile GrowPathToPathfindingWest() => GrowPathToPathfinding(neighbor[3], TileDirection.east);

    //ca delete pas les old network
    public void CheckNeighborNetwork()
    {
        bool hasNetworkNeighbor = false;
        List<Network> networkClose = new List<Network>();
        for (int i = 0; i < 4; i++) { 
            if (neighbor[i] != null) { 
                if (neighbor[i].Network != null) {
                    networkClose.Add(neighbor[i].Network);
                    hasNetworkNeighbor = true;
                }
            }
        }
        bool doMerge = false;
        for(int i = 1; i < networkClose.Count; i++) {
            if(networkClose[i] != networkClose[0]) {
                doMerge = true;
            }
        }
        if(hasNetworkNeighbor && !doMerge) {
            AddNetwork(networkClose[0]);
            SetNetworkNeighbor(networkClose[0]);
            networkClose[0].RelinkStation();
        }
        if (doMerge) {
            for (int i = 1; i < 4; i++) {
                if (networkClose[0] != neighbor[i].network) {
                    //if(neighbor[i].network != null)
                        //networkClose[0].MergeNetwork(neighbor[i].network.networkStationList);
                    neighbor[i].RemoveNetwork();
                    neighbor[i].SetNetworkNeighbor(networkClose[0]);
                }
            }
            networkClose[0].RelinkStation();
        }
    }

    public void SetNetworkNeighbor(Network networkToCheck)
    {
        for (int i = 0; i < 4; i++)
            if(neighbor[i] != null)
                if(neighbor[i].HasRail)
                    neighbor[i].AddNetwork(networkToCheck);
    }
    public void AddNetwork(Network networkToCheck)
    {
        if(Network != networkToCheck) { 
            Network = networkToCheck;
            if (hasStation)
                    network.AddStation(station);
        }
        else if(network != networkToCheck) {
            //merge both network
        }
    }
    public void RemoveNetwork()
    {
        if (Network != null)
        {
            Debug.Log("remove network");
            network.Delete();
        }
    }

    public void UpdateRailNeighbor(bool hasRail)
    {
        if (hasRail)
            nbRailNeighbor++;
        else
            nbRailNeighbor--;

        if (HasRail)
            UpdateRail();
        //update network
    }

    void UpdateRail()
    {
        Destroy(rail);
        rail = null;
        float rotation;
        switch (nbRailNeighbor)
        {
            case 1:
                {
                    rail = Instantiate(GameManager.Instance.railPrefabs[4], transform);
                    railTransform = rail.transform;
                    rotation = 180f;
                    for (int i = 0; i < neighbor.Length; i++)
                    {
                        if (neighbor[i] && neighbor[i].hasRail)
                            railTransform.rotation = Quaternion.Euler(0f, rotation, 0f);
                        rotation += 90f;
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
                            rail = Instantiate(GameManager.Instance.railPrefabs[0], transform);
                            railTransform = rail.transform;
                            railTransform.rotation = Quaternion.Euler(0f, rotation, 0f);
                        }
                        if (neighbor[i] && neighbor[(i + 1) % 4] &&
                        neighbor[i].hasRail && neighbor[(i + 1) % 4].hasRail)
                        {
                            rail = Instantiate(GameManager.Instance.railPrefabs[1], transform);
                            railTransform = rail.transform;
                            railTransform.rotation = Quaternion.Euler(0f, rotation, 0f);
                        }
                        rotation += 90f;
                    }
                    break;
                }
            case 3:
                {
                    rotation = 0f;
                    rail = Instantiate(GameManager.Instance.railPrefabs[2], transform);
                    railTransform = rail.transform;
                    for (int i = 0; i < neighbor.Length; i++)
                    {
                        if ((neighbor[i] && !neighbor[i].hasRail) || neighbor[i] == null)
                        {
                            railTransform.rotation = Quaternion.Euler(0f, rotation, 0f);
                        }
                        rotation += 90f;
                    }
                    break;
                }
            case 4:
                {
                    rail = Instantiate(GameManager.Instance.railPrefabs[3], transform);
                    railTransform = rail.transform;
                    railTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                }
            default:
                rail = Instantiate(GameManager.Instance.railPrefabs[5], transform);
                railTransform = rail.transform;
                break;
        }
        railMat = rail.GetComponentInChildren<Renderer>().material;
        if (!HasRail)
        {
            Destroy(rail);
            rail = null;
        }
    }

    void SpawnStation()
    {
        stationPrefab = Instantiate(GameManager.Instance.stationPrefab, transform);
        station = stationPrefab.GetComponent<Station>();
        station.SetTile(this);
        for(int i = 0; i < 4; i++) {
            if (neighbor[i].HasIndustry) {
                station.AddIndustry(neighbor[i].industry);
            }
        }
    }
    void DestroyStation()
    {
        Destroy(stationPrefab);
        stationPrefab = null;
    }
    void SpawnFactory()
    {
        buildingPrefab = Instantiate(GameManager.Instance.factoryPrefab, transform);
        industry = buildingPrefab.GetComponent<Industry>();
        for (int i = 0; i < 4; i++) {
            if (neighbor[i].HasStation) {
                neighbor[i].station.AddIndustry(industry);
            }
        }
    }
    void DestroyFactory()
    {
        Destroy(buildingPrefab); 
        buildingPrefab = null;
    }

    public void ShowNetwork()
    {
        if (railMat != null && network != null)
            railMat.color = network.colorNetwork;
    }
    public void HideNetwork()
    {
        if(railMat != null)
            railMat.color = new Color(70 / 255f, 35 / 255f, 27 / 255f);
    }
    public void Paint(Color color)
    {
        if (groundMat) groundMat.color = color;
    }
    //use for debbug
    public void ShowPath()
    {
        if (distance != int.MaxValue)
            Paint(Color.blue);
    }
    public void HidePath()
    {
        Paint(Color.white);
    }
}
