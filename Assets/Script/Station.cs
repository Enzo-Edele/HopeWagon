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
    int storage = 50; //pas besoin de passer en liste tant que les gares on un stockage égal pour tout
    [SerializeField] public List<int> stockRessources;

    [SerializeField] List<bool> canExport = new List<bool>();
    [SerializeField] List<bool> canImport = new List<bool>();

    int requestTime = 2;
    float requestTimer;

    //toScrap
    [SerializeField] TMP_Text nameDisplay;

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
        destinationList = GridBoard.Instance.GetStationInNetwork(tile);
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
    //a terme autoriser le joueur a lock et unlock des ressources
    public void CheckImportExport()
    {
        canImport.Clear();
        canExport.Clear();
        for (int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++) {
            canExport.Add(false);
            canImport.Add(false);
        }
        for (int i = 0; i < linkedIndustries.Count; i++) {
            for(int j = 0; j < linkedIndustries[i].canExport.Count; j++) {
                canExport[linkedIndustries[i].canExport[j]] = true;
            }
            for (int k = 0; k < linkedIndustries[i].canImport.Count; k++) {
                canImport[linkedIndustries[i].canImport[k]] = true;
            }
        }
    }

    private void Awake()
    {
        name = GameManager.Instance.GiveStationName();
        nameStation = name;
        nameDisplay.text = nameStation;

        GridBoard.Instance.stationList.Add(this); //add (check if it exist) a method to properly remove station
        stockRessources = new List<int>();
        for (int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++)
        {
            stockRessources.Add(0);
            canExport.Add(false);
            canImport.Add(false);
        }
    }

    private void Start()
    {
        for (int i = 0; i < destinationList.Count; i++)
            destinationNameList.Add(destinationList[i].name);
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
        train.Spawn(path[0]); //donne la premiére tile
        train.SetWagons(GameManager.Instance.wagonTemplate);
        
        List<int> toLoad = new List<int>();
        for(int j = 0; j < GameManager.Instance.ressourceTypes.Count; j++) {
            if (canExport[j] && canExport[j] == destination.canImport[j])
                toLoad.Add(j);
        }
        for (int j = 0; j < toLoad.Count; j++)
        {
            SetStockRessource(train.LoadRessources(stockRessources[toLoad[j]], toLoad[j]), toLoad[j]);
        }
    }

    private void Update() {
        if(requestTimer < 0) {
            requestTimer = requestTime;
            int indexRessource = -1;
            //take ressource
            for(int i = 0; i < linkedIndustries.Count; i++) {
                for(int j = 0; j < linkedIndustries[i].canExport.Count; j++) {
                    indexRessource = linkedIndustries[i].canExport[j];
                    if (canExport[indexRessource]) {
                        linkedIndustries[i].SetStockRessource(ChangeStorageRessource(linkedIndustries[i].stockRessources[indexRessource], indexRessource), indexRessource); 
                        //verifier sécurité pour empêcher dupli
                    }
                }
            }
            //send ressource
            for (int i = 0; i < linkedIndustries.Count; i++) {
                for (int j = 0; j < linkedIndustries[i].canImport.Count; j++) {
                    indexRessource = linkedIndustries[i].canImport[j];
                    if (canImport[indexRessource]) {
                        SetStockRessource(linkedIndustries[i].ChangeStorageRessource(stockRessources[indexRessource], indexRessource), indexRessource);
                        //verifier sécurité pour empêcher dupli
                    }
                }
            }
        }
        else
        {
            requestTimer -= Time.deltaTime;
        }
    }

    public int ChangeStorageRessource(int changeValue, int valueIndex)
    {
        int leftover = 0;
        stockRessources[valueIndex] += changeValue;
        if (stockRessources[valueIndex] < 0)
        {
            leftover = stockRessources[valueIndex];
            stockRessources[valueIndex] = 0;
        }
        else if (stockRessources[valueIndex] > storage)
        {
            leftover = stockRessources[valueIndex] - storage;
            stockRessources[valueIndex] = storage;
        }

        return leftover;
    }
    public void SetStockRessource(int newStock, int valueIndex)
    {
        //ajouter sécurité pour voir si ressource utilisé
        stockRessources[valueIndex] = newStock;
        if (stockRessources[valueIndex] < 0)
            stockRessources[valueIndex] = 0;
        else if (stockRessources[valueIndex] > storage)
            stockRessources[valueIndex] = storage;

        //vérifier si leftover ???
    }

    public int UnloadRessources(int qtyUnload, int ressourceIndex)
    {
        int leftover = ChangeStorageRessource(qtyUnload, ressourceIndex);
        //Debug.Log(stockRessources[ressourceIndex] + " loaded out of " + qtyUnload + " of type " + ressourceIndex);
        return leftover;
    }
}
