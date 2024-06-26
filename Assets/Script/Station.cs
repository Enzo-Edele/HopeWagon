using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Station : MonoBehaviour
{
    public string nameStation = "Name_Station";
    public GameTile tile { get; private set; }

    public List<Station> destinationList = new List<Station>();
    public List<string> destinationNameList = new List<string>();

    [SerializeField] List<Industry> linkedIndustries = new List<Industry>();
    [SerializeField] List<PollutedIndustry> linkedPollutedIndustry = new List<PollutedIndustry>();
    [SerializeField] List<PollutionCleaner> linkedPollutionCleaners = new List<PollutionCleaner>();
    int storage = 50; //pas besoin de passer en liste tant que les gares on un stockage �gal pour tout
    [SerializeField] public List<int> stockRessources; //public get private set

    public List<bool> canExport = new List<bool>(); //public get private set
    public List<bool> canImport = new List<bool>(); //public get private set

    List<TrainRoute> RouteList = new List<TrainRoute>();

    int requestTime = 2;
    float requestTimer;

    [SerializeField] TMP_Text nameDisplay;

    public Vector2 cameraFocusPos;

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
        for(int i = 0;i < destinationList.Count; i++)
        {
            destinationNameList.Add(destinationList[i].name);
        }
        Vector3 tilePos = tile.transform.position;
        cameraFocusPos = new Vector2(tilePos.x, tilePos.z - 2);
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
    public void AddPollutedIndustry(PollutedIndustry industry)
    {
        for (int i = 0; i < linkedPollutedIndustry.Count; i++)
            if (linkedPollutedIndustry[i] == industry)
                return;
        linkedPollutedIndustry.Add(industry);
    }
    public void RemovePollutedIndustry(PollutedIndustry industry)
    {
        for (int i = 0; i < linkedPollutedIndustry.Count; i++)
            if (industry == linkedPollutedIndustry[i])
                linkedPollutedIndustry.RemoveAt(i);
    }
    public void AddCleaner(PollutionCleaner cleaner)
    {
        for (int i = 0; i < linkedPollutionCleaners.Count; i++)
            if (linkedPollutionCleaners[i] == cleaner)
                return;
        linkedPollutionCleaners.Add(cleaner);
    }
    public void RemoveCleaner(PollutionCleaner cleaner)
    {
        for (int i = 0; i < linkedPollutionCleaners.Count; i++)
            if (cleaner == linkedPollutionCleaners[i])
                linkedPollutionCleaners.RemoveAt(i);
    }

    public void ChangeName(string nName)
    {
        name = nName;
        nameStation = nName;
        nameDisplay.text = nName;
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
            for(int j = 0; j < linkedIndustries[i].exportID.Count; j++) {
                canExport[linkedIndustries[i].exportID[j]] = true;
            }
            for (int k = 0; k < linkedIndustries[i].importID.Count; k++) {
                canImport[linkedIndustries[i].importID[k]] = true;
            }
        }
        for (int i = 0; i < linkedPollutedIndustry.Count; i++)
        {
            for (int j = 0; j < linkedPollutedIndustry[i].exportID.Count; j++) {
                canExport[linkedPollutedIndustry[i].exportID[j]] = true;
            }
            for (int k = 0; k < linkedPollutedIndustry[i].importID.Count; k++) {
                canImport[linkedPollutedIndustry[i].importID[k]] = true;
            }
        }
    }

    private void Awake()
    {
        name = GameManager.Instance.GiveStationName();
        nameStation = name;
        nameDisplay.text = nameStation;

        GridBoard.Instance.AddStation(this);
        stockRessources = new List<int>();
        for (int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++)
        {
            stockRessources.Add(0);
            canExport.Add(false);
            canImport.Add(false);
        }
    }

    public void CreateRoute(Station destination)
    {
        List<GameTileCopy> path = new List<GameTileCopy>();
        //do for all dest of path
        Queue<GameTile> pathToCopy = GridBoard.Instance.Pathfinding(destination.tile, tile);

        TrainRoute route = Instantiate(GameManager.Instance.routePrefab).GetComponent<TrainRoute>(); 

        GameTile tileToCopy = pathToCopy.Dequeue();
        path.Add(Instantiate(GameManager.Instance.tileCopy, route.gameObject.transform).GetComponent<GameTileCopy>());
        path[0].SetUpTileCopy(tileToCopy.tileCoordinate, tileToCopy.transform.position, tileToCopy.distance, tileToCopy.pathDirection, tileToCopy.exitPoint);
        int i = 0;
        while(pathToCopy.Count > 0)
        {
            i++;
            GameTile tileTC = pathToCopy.Dequeue();
            path.Add(Instantiate(GameManager.Instance.tileCopy, route.gameObject.transform).GetComponent<GameTileCopy>()); //referenc� la gametilecopy dans une factory
                                                                                               //et virer le get component TRANSFORM ???
            path[i].SetUpTileCopy(tileTC.tileCoordinate, tileTC.transform.position, tileTC.distance, tileTC.pathDirection, tileTC.exitPoint);
            path[i - 1].SetUpTileCopyNext(path[i]);
        }

        route.Initialize(path, this, destination);
        GameManager.Instance.gridBoard.AddRoute(route);
    }
    public void CreateRouteMultiple(List<Station> destinationsList)
    {
        List<List<GameTileCopy>> pathArray = new List<List<GameTileCopy>>();
        TrainRoute route = Instantiate(GameManager.Instance.routePrefab).GetComponent<TrainRoute>();

        for (int i = 0; i < destinationList.Count; i++)
        {
            List<GameTileCopy> path = new List<GameTileCopy>();
            Queue<GameTile> pathToCopy = GridBoard.Instance.Pathfinding(destinationsList[0].tile, tile);

            GameTile tileToCopy = pathToCopy.Dequeue();
            path.Add(Instantiate(GameManager.Instance.tileCopy, route.gameObject.transform).GetComponent<GameTileCopy>());
            path[0].SetUpTileCopy(tileToCopy.tileCoordinate, tileToCopy.transform.position, tileToCopy.distance, tileToCopy.pathDirection, tileToCopy.exitPoint);
            int j = 0;
            while (pathToCopy.Count > 0)
            {
                j++;
                GameTile tileTC = pathToCopy.Dequeue();
                path.Add(Instantiate(GameManager.Instance.tileCopy, route.gameObject.transform).GetComponent<GameTileCopy>()); //referenc� la gametilecopy dans une factory
                                                                                                                               //et virer le get component TRANSFORM ???
                path[j].SetUpTileCopy(tileTC.tileCoordinate, tileTC.transform.position, tileTC.distance, tileTC.pathDirection, tileTC.exitPoint);
                path[j - 1].SetUpTileCopyNext(path[j]);
            }
            pathArray.Add(path);
        }
        route.InitializeMultiple(pathArray, this, destinationsList);
        GameManager.Instance.gridBoard.AddRoute(route);
    }

    public void LoadTrain(Station destination, TrainRoute route)
    {
        List<int> toLoad = new List<int>();
        for (int j = 0; j < GameManager.Instance.ressourceTypes.Count; j++)
        {
            if (canExport[j] && canExport[j] == destination.canImport[j])
                toLoad.Add(j);
        }
        for (int j = 0; j < toLoad.Count; j++)
        {
            SetStockRessource(route.LoadRessources(stockRessources[toLoad[j]], toLoad[j]), toLoad[j]);
        }

        for(int i = 0; i < linkedIndustries.Count; i++)
        {
            linkedIndustries[i].SendRessourcesToStation();
        }

    }

    private void Update() {
        if(requestTimer < 0) {
            requestTimer = requestTime;
            for(int i = 0; i < linkedIndustries.Count; i++) {
                //linkedIndustries[i].SendRessourcesToStation();
            }
            //SendRessourcesToIndustry();
        }
        else
        {
            requestTimer -= Time.deltaTime;
        }
    }

    public void SendRessourcesToIndustry()
    {
        int indexRessource = -1;
        for (int i = 0; i < linkedIndustries.Count; i++)
        {
            for (int j = 0; j < linkedIndustries[i].importID.Count; j++)
            {
                indexRessource = linkedIndustries[i].importID[j];
                if (canImport[indexRessource])
                {
                    SetStockRessource(linkedIndustries[i].ChangeStorageRessource(stockRessources[indexRessource], indexRessource), indexRessource);
                    //verifier s�curit� pour emp�cher dupli
                }
            }
        }
        for (int i = 0; i < linkedPollutionCleaners.Count; i++)
        {
            for (int j = 0; j < linkedPollutionCleaners[i].importID.Count; j++)
            {
                indexRessource = linkedPollutionCleaners[i].importID[j];
                if (canImport[indexRessource])
                {
                    SetStockRessource(linkedPollutionCleaners[i].ChangeStorageRessource(stockRessources[indexRessource], indexRessource), indexRessource);
                    //verifier s�curit� pour emp�cher dupli
                }
            }
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
        //ajouter s�curit� pour voir si ressource utilis�
        stockRessources[valueIndex] = newStock;
        if (stockRessources[valueIndex] < 0)
            stockRessources[valueIndex] = 0;
        else if (stockRessources[valueIndex] > storage)
            stockRessources[valueIndex] = storage;

        //v�rifier si leftover ???
    }

    public int UnloadRessources(int qtyUnload, int ressourceIndex)
    {
        int leftover = ChangeStorageRessource(qtyUnload, ressourceIndex);
        SendRessourcesToIndustry();
        
        return leftover;
    }
    public int UnloadRessourcesPolluted(int qtyUnload, int ressourceIndex)
    {
        int leftover = ChangeStorageRessource(qtyUnload, ressourceIndex);
        return leftover;
    }
    public bool BuildIndustry()
    {
        for (int i = 0; i < linkedPollutedIndustry.Count; i++) {
            for (int j = 0; j < canImport.Count; j++) {
                if (linkedPollutedIndustry[i] != null && canImport[j] && canImport[j] == linkedPollutedIndustry[i].canImport[j])
                {
                    stockRessources[j] = linkedPollutedIndustry[i].AddRessource(j, stockRessources[j]);
                }
            }
            if (linkedPollutedIndustry[i].CanDepollute()) {
                linkedPollutedIndustry[i].Depollute();
                return true;
            }
        }
        return false;
    }
    public bool CheckFillPolluted(int id)
    {
        bool idFilled = true;
        if (linkedPollutedIndustry.Count == 0)
            idFilled = false;
        for(int i = 0; i < linkedPollutedIndustry.Count; i++)
        {
            if (!linkedPollutedIndustry[i].FillRessource(id))
                idFilled = false;
        }
        return idFilled;
    }
}
