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
    [SerializeField] int inStock;
    [SerializeField] public List<int> stockRessources;

    [SerializeField] List<bool> canExport = new List<bool>();
    [SerializeField] List<bool> canImport = new List<bool>();

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
    //a terme autoriser le joueur a lock et unlock des ressources
    public void CheckImportExport()
    {
        canImport.Clear();
        canExport.Clear();
        for (int i = 0; i < GameManager.Instance.ressourceSample.Count; i++) {
            canExport.Add(false);
            canImport.Add(false);
        }
        for (int i = 0; i < linkedIndustries.Count; i++) {
            for(int j = 0; j < linkedIndustries[i].canExport.Count; j++) {
                canImport[linkedIndustries[i].canExport[j]] = true;
            }
            for (int k = 0; k < linkedIndustries[i].canImport.Count; k++) {
                canExport[linkedIndustries[i].canImport[k]] = true;
            }
        }
    }

    private void Awake()
    {
        stockRessources = new List<int>();
        for (int i = 0; i < GameManager.Instance.ressourceSample.Count; i++) {
            stockRessources.Add(0);
            canExport.Add(false);
            canImport.Add(false);
        }
    }

    private void Start()
    {
        name = GameManager.Instance.GiveStationName();
        nameStation = name;
        nameDisplay.text = nameStation;
        destinationList = GridBoard.Instance.GetStationInNetwork(tile);
        for (int i = 0; i < destinationList.Count; i++)
            destinationNameList.Add(destinationList[i].name);
        GridBoard.Instance.stationList.Add(this);      //add (check if it exist) a method to properly remove station
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
            int indexRessource = -1;
            //maybe repenser pour ne pas faire masse de for
            //take ressource
            for(int i = 0; i < linkedIndustries.Count; i++) {
                //loop through all importable
                for(int j = 0; j < linkedIndustries[i].canExport.Count; j++) {
                    //check if matches industry exportable
                    indexRessource = linkedIndustries[i].canExport[j];
                    if (canImport[indexRessource]) {
                        linkedIndustries[i].SetStockRessource(ChangeStorageRessource(linkedIndustries[i].stockRessources[indexRessource], indexRessource), indexRessource); 
                        //verifier sécurité pour empêcher dupli
                    }
                }
            }
            //send ressource
            for (int i = 0; i < linkedIndustries.Count; i++) {
                //loop through all exportable
                for (int j = 0; j < linkedIndustries[i].canImport.Count; j++) {
                    //check if matches industry importable
                    indexRessource = linkedIndustries[i].canImport[j];
                    if (canExport[indexRessource]) {
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

        return leftover;
    }
    public void SetStock(int newStock)
    {
        inStock = newStock;
        if (inStock < 0)
            inStock = 0;
        else if (inStock > storage)
            inStock = storage;
    }

    public int ChangeStorageRessource(int changeValue, int valueIndex)
    {
        int leftover = 0;
        //au besoin récupérer les id depuis l'index de la liste
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
        //Update UI texte

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

        //Update UI string
    }

    public int Unload(int toUnload)
    {
        int leftover = ChangeStorage(toUnload);

        return leftover;
    }
}
