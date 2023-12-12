using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    [SerializeField] int nbRailNeighbor;
    [SerializeField]bool hasRail;
    public bool HasRail
    {
        get { return hasRail; }
        set {
            if (HasStation)
                value = true;
            if (!CanBuildStationRail())
                value = false;
            if (value != hasRail) {
                hasRail = value;
                if (hasRail)
                    GameManager.Instance.playerData.ChangeRailStock(-1);
                if (!hasRail)
                    GameManager.Instance.playerData.ChangeRailStock(0);
                for (int i = 0; i < neighbor.Length; i++)
                    if (neighbor[i]) neighbor[i].UpdateRailNeighbor(value);
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
    [SerializeField] bool hasStation;
    public bool HasStation
    {
        get { return hasStation; }
        set
        {
            if (!CanBuildStationRail())
                value = false;
            if (value != hasStation)
            {
                hasStation = value;
                if (hasStation) {
                    SpawnStation();
                    HasRail = hasStation;   //l'ordre de ces deux lignes a un GROS IMPACT sur le network à revoir
                    GameManager.Instance.playerData.ChangeStationStock(-1);
                }
                else if (!hasStation) {
                    Network.RemoveStation(station);
                    DestroyStation();
                    GameManager.Instance.playerData.ChangeStationStock(0);
                }
            }
        }
    }

    public Industry industry { get; private set; }
    [SerializeField] bool hasIndustry;
    public bool HasIndustry
    {
        get { return hasIndustry; }
        set
        {
            if (value != hasIndustry)
            {
                if (value && CanBuildIndustry()) {
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

    bool CanBuildIndustry()
    {
        return !(hasStation || hasRail);
    }
    bool CanBuildStationRail()
    {
        return !(hasIndustry);
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
            if (tile == neighbor[i])
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
            for (int j = 0; j < 4; j++) {
                if (networkClose[0] != neighbor[j].Network) {
                    if (neighbor[j].Network != null) {
                        networkClose[0].MergeNetwork(neighbor[j].network.networkStationList); //
                    }
                    //Debug.Log(" equal : " + neighbor[j].gameObject.name + " ref : " + networkClose[0].name);
                    neighbor[j].RemoveNetwork();
                    neighbor[j].SetNetworkNeighbor(networkClose[0]); //0 eat the other network
                }
            }
            networkClose[0].RelinkStation();
        }
    }
    public void RemoveNetwork()
    {
        if (Network != null)
        {
            Network.Delete();
        }
    }
    public void SetNetworkNeighbor(Network networkToCheck)
    {
        if(hasRail)
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
                    Network.AddStation(station);
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
                neighbor[i].industry.AddStation(station);
            }
        }
        station.CheckImportExport();
    }
    void DestroyStation()
    {
        Destroy(stationPrefab);
        stationPrefab = null;
        for (int i = 0; i < 4; i++) {
            if (neighbor[i].HasIndustry) {
                neighbor[i].industry.RemoveStation(station);
            }
        }
        //GameManager.Instance.gridBoard.   //créer fonction pour update station list
    }
    void SpawnFactory()
    {
        buildingPrefab = Instantiate(GameManager.Instance.factoryPrefab, transform);
        industry = buildingPrefab.GetComponent<Industry>();
        for (int i = 0; i < 4; i++) {
            if (neighbor[i].HasStation) {
                neighbor[i].station.AddIndustry(industry);
                industry.AddStation(neighbor[i].station);
                neighbor[i].station.CheckImportExport();
            }
        }
    }
    void DestroyFactory()
    {
        Destroy(buildingPrefab); 
        buildingPrefab = null;
        for (int i = 0; i < 4; i++) {
            if (neighbor[i].HasStation) {
                neighbor[i].station.RemoveIndustry(industry);
            }
        }
    }

    //link to UI

    public void UpdateUI(InGameUI ui)
    {
        string content;
        if (hasIndustry)
        {
            content = "Industry";
            ui.UpdateItemDisplayList(industry.canImport, industry.canExport, industry.stockRessources);
        }
        else if (hasStation)
        {
            content = "Station";
            List<int> numberRessources = new List<int>();
            for(int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++) {
                numberRessources.Add(i);
            }
            ui.UpdateItemDisplayList(numberRessources, numberRessources, station.stockRessources);
        }
        else
            content = "Empty";
        ui.UpdateTileInfo(name, content);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(HasRail);
        writer.Write(HasStation);
        if (hasIndustry)
        {
            writer.Write(HasIndustry);
            if (industry.Type != null)
                writer.Write(industry.Type.id); //opti plus tard en byte
            else
            {
                writer.Write(-1);
            }
        }
        else
        {
            writer.Write(HasIndustry);
            writer.Write(-1);
        }
    }

    public void Load(BinaryReader reader, int header)
    {
        ClearTile();
        HasRail = reader.ReadBoolean();
        HasStation = reader.ReadBoolean();
        HasIndustry = reader.ReadBoolean();
        int industryType = reader.ReadInt32();
        if (HasIndustry) {
            if(industryType >= 0) 
                industry.SetIndustryType(GameManager.Instance.industryTypes[industryType]);
        }
        else {
            HasIndustry = false;
        }
    }

    public void ClearTile() {
        HasRail = false;
        HasIndustry = false;
        HasStation = false;
    }

    //use for advance option and debbug
    public void ShowNetwork()
    {
        if (railMat != null && Network != null)
            railMat.color = Network.colorNetwork;
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

    //to scrap
    public void UpdateUIBIS(InGameUI ui)
    {
        string content;
        if (hasIndustry)
        {
            content = "Industry";
            //industry.canExport    //industry.canImport
            ui.UpdateItemDisplayListBIS(industry.canImport, industry.canExport, industry.stockRessources);
        }
        else if (hasStation)
        {
            content = "Station";
            //station.stockRessources
            List<int> numberRessources = new List<int>();
            for (int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++)
            {
                numberRessources.Add(i);
            }
            ui.UpdateItemDisplayListBIS(numberRessources, numberRessources, station.stockRessources);
        }
        else
            content = "Empty";
        ui.UpdateTileInfoBIS(name, content);
    }
}
