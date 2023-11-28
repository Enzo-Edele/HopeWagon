using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Station : MonoBehaviour
{
    public string nameStation = "Name_Station";
    public GameTile tile { get; private set; }

    [SerializeField] List<Station> destinationList = new List<Station>();
    public List<string> destinationNameList = new List<string>();

    [SerializeField] List<Industry> linkedIndustries = new List<Industry>();
    int storage = 50;
    [SerializeField] int inStock;

    int requestTime = 2;
    float requestTimer;

    //toScrap
    [SerializeField] TMP_Text nameDisplay;
    [SerializeField] TMP_Text stockDisplay;

    public void SetTile(GameTile newTile)
    {
        tile = newTile;
        if(tile.Network != null)
            tile.Network.AddStation(this);
        else {
            Network newNetwork = GridBoard.Instance.AddNetwork(Instantiate(GridBoard.Instance.network).GetComponent<Network>());
            newNetwork.Initialize();
            newNetwork.AddStation(this);
            newNetwork.ClaimTile(tile);
        }
    }

    public void AddDestination(Station station)
    {
        for (int i = 0; i < destinationList.Count; i++)
            if (destinationList[i] == station)
                return;
        destinationList.Add(station);
        destinationNameList.Add(station.name);
    }
    public void RemoveDestination(Station station)
    {
        for (int i = 0; i < destinationList.Count; i++)
        {
            if (destinationList[i] == station)
            {
                destinationList.RemoveAt(i);
                destinationNameList.RemoveAt(i);
            }
        }
    }

    public void AddIndustry(Industry industry)
    {
        for (int i = 0; i < linkedIndustries.Count; i++)
            if (linkedIndustries[i] == industry)
                return;
        linkedIndustries.Add(industry);
    }
    public void RemoveIndustry(Industry industry)
    {
        for (int i = 0; i < linkedIndustries.Count; i++)
            if (industry == linkedIndustries[i])
                linkedIndustries.RemoveAt(i);
    }

    private void Start()
    {
        name = GameManager.Instance.GiveStationName();
        nameStation = name;
        nameDisplay.text = nameStation;
        destinationList = GridBoard.Instance.GetStationInNetwork(tile);
        for (int i = 0; i < destinationList.Count; i++)
            destinationNameList.Add(destinationList[i].name);
        GridBoard.Instance.stationList.Add(this);
        inStock = 0;
        stockDisplay.text = "" + inStock;
    }

    public void DeployTrain(Station destination)
    {
        List<GameTileCopy> path = new List<GameTileCopy>();
        Queue<GameTile> pathToCopy = GridBoard.Instance.Pathfinding(destination.tile, tile);

        GameTile tileToCopy = pathToCopy.Dequeue();
        path.Add(Instantiate(GameManager.Instance.tileCopy).GetComponent<GameTileCopy>());
        path[0].SetUpTileCopy(tileToCopy.tileCoordinate, tileToCopy.transform.position, tileToCopy.distance, tileToCopy.pathDirection, tileToCopy.exitPoint);
        int i = 0;
        while(pathToCopy.Count > 0)
        {
            i++;
            GameTile tileTC = pathToCopy.Dequeue();
            path.Add(Instantiate(GameManager.Instance.tileCopy).GetComponent<GameTileCopy>()); //referencé la gametilecopy dans une factory
                                                                                               //et virer le get component TRANSFORM ???
            path[i].SetUpTileCopy(tileTC.tileCoordinate, tileTC.transform.position, tileTC.distance, tileTC.pathDirection, tileTC.exitPoint);
            path[i - 1].SetUpTileCopyNext(path[i]);
        }
        Train train = Instantiate(GameManager.Instance.trainPrefab).GetComponent<Train>();
        train.SetPath(path);
        train.Spawn(path[0]); //donner la premiére tile
        train.SetWagons(GameManager.Instance.wagonTemplate);
        SetStock(train.Load(inStock));
    }

    private void Update() {
        if(requestTimer < 0) {
            requestTimer = requestTime;
            if(inStock < storage) { 
                for(int i = 0; i < linkedIndustries.Count; i++) {
                    linkedIndustries[i].SetStock(ChangeStorage(linkedIndustries[i].stock)); //ajouté des sécurité pour éviter dupli
                }
            }
        }
        else
        {
            requestTimer -= Time.deltaTime;
        }
    }

    int ChangeStorage(int changeValue)
    {
        int leftover = 0;

        inStock += changeValue;
        if (inStock < 0)
        {
            leftover = inStock;
            inStock = 0;
        }
        else if (inStock > storage)
        {
            leftover = inStock - storage;
            inStock = storage;
        }
        stockDisplay.text = "" + inStock;

        return leftover;
    }
    public void SetStock(int newStock)
    {
        inStock = newStock;
        if (inStock < 0)
            inStock = 0;
        else if (inStock > storage)
            inStock = storage;
        stockDisplay.text = "" + inStock;
    }

    public int Unload(int toUnload)
    {
        int leftover = ChangeStorage(toUnload);

        return leftover;
    }
}
