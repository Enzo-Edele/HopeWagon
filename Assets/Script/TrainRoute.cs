using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainRoute : MonoBehaviour
{
    float timerTrain = 0.0f;
    int timeTrain = 2;

    //string routeName;
    public List<Station> destinationArray = new List<Station>();  //on met le départ a 0
    public Station currentDestination; // au start on prend array[1]
    public Station currentStartPoint;
    public List<GameTileCopy> path = new List<GameTileCopy>();
    public List<GameTileCopy> pathReverse = new List<GameTileCopy>();

    public UIRouteItem displayUI;
    //parent said copy tile heto object

    public List<bool> RouteRessources;
    public List<int> stockRessources;
    int storage = 12;   //to update when implementing wagon

    public bool toStop = false;

    Train train; //a terme on aura des trains custom pour l'instant prendre un prefab

    private void Awake()
    {
        stockRessources = new List<int>();
        RouteRessources = new List<bool>();
        for (int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++)
        {
            RouteRessources.Add(false);
            stockRessources.Add(0);
        }
    }
    private void Update()
    {
        if (timerTrain < 0)
            DeployTrain(0);
        else if (timerTrain > 0)
            timerTrain -= Time.deltaTime;
    }

    public void Initialize(List<GameTileCopy> newPath, Station depart, Station destination)
    {
        destinationArray.Add(depart);
        destinationArray.Add(destination);
        //replace by a copy of destPath list

        currentStartPoint = depart;
        currentDestination = destination;
        //add a list of path
        path = newPath;  
        DeployTrain(0);

        //adapt for multiple station
        Queue<GameTile> pathToCopy = GridBoard.Instance.Pathfinding(depart.tile, destination.tile);
        GameTile tileToCopy = pathToCopy.Dequeue();
        pathReverse.Add(Instantiate(GameManager.Instance.tileCopy, gameObject.transform).GetComponent<GameTileCopy>());
        pathReverse[0].SetUpTileCopy(tileToCopy.tileCoordinate, tileToCopy.transform.position, tileToCopy.distance, tileToCopy.pathDirection, tileToCopy.exitPoint);
        int i = 0;
        while (pathToCopy.Count > 0)
        {
            i++;
            GameTile tileTC = pathToCopy.Dequeue();
            pathReverse.Add(Instantiate(GameManager.Instance.tileCopy, gameObject.transform).GetComponent<GameTileCopy>()); //referencé la gametilecopy dans une factory
            pathReverse[i].SetUpTileCopy(tileTC.tileCoordinate, tileTC.transform.position, tileTC.distance, tileTC.pathDirection, tileTC.exitPoint);
            pathReverse[i - 1].SetUpTileCopyNext(pathReverse[i]);
        }
    }

    public void DeployTrain(int pathIndex)
    {
        if (GameManager.Instance.playerData.trainStock > 0)
        {
            if (toStop)
            {
                stopRoute();
                return;
            }
            train = Instantiate(GameManager.Instance.trainPrefab, new Vector3(0, -10, 0), Quaternion.identity).GetComponent<Train>();
            train.SetPath(path);
            train.Spawn(path[pathIndex], this);
            currentStartPoint.LoadTrain(currentDestination, this);
            train.SetWagons(GameManager.Instance.wagonTemplate);
            timerTrain = 0;
            GameManager.Instance.playerData.ChangeTrainStock(-1);
            if(displayUI != null)
                displayUI.UpdateDisplay();
        }
        else
            timerTrain = timeTrain;
    }
    public void SetNextPath() //to update with for loop
    {
        List<GameTileCopy> memPath = new List<GameTileCopy>();
        memPath = path;
        path = pathReverse;
        pathReverse = memPath;

        Station memStation = destinationArray[0];
        destinationArray[0] = destinationArray[1];
        destinationArray[1] = memStation;
        currentStartPoint = destinationArray[0];
        currentDestination = destinationArray[1];

        DeployTrain(0);
    }

    public int LoadRessources(int qty, int index)
    {
        int leftover = 0;
        Debug.Assert(qty >= 0, "WARNING : can't load negative value on train");
        stockRessources[index] += qty;
        RouteRessources[index] = true;

        if (stockRessources[index] > storage)
        {
            leftover = stockRessources[index] - storage;
            stockRessources[index] = storage;
        }
        //Debug.Log(stockRessources[index] + " loaded out of " + qty + " remaining " + leftover);

        return leftover;
    }
    public void UnloadRessource(GameTileCopy copy)
    {
        for (int i = 0; i < RouteRessources.Count; i++)
        {
            if (RouteRessources[i])
            {
                GameManager.Instance.playerData.AddContratProgress(i, stockRessources[i]);
                stockRessources[i] = GridBoard.Instance.GetTile(copy.tileCoordinate).station.UnloadRessources(stockRessources[i], i);
                RouteRessources[i] = false;
            }
        }
    }

    void stopRoute()
    {
        GameManager.Instance.gridBoard.RemoveRoute(this);
        Destroy(displayUI.gameObject);
        Destroy(gameObject);
    }
    public void LoadigStopRoute()
    {
        Destroy(displayUI.gameObject);
        Destroy(gameObject);
    }
    /*  depreciated for now
    public void reversePath()
    {
        List<GameTileCopy> pathReverse = new List<GameTileCopy>();
        for (int i = path.Count - 1; i >= 0; i--)
        {
            pathReverse.Add(path[i]);
        }
        for (int i = 0; i < path.Count - 1; i++)
        {
            pathReverse[i].nextOnPath = pathReverse[i + 1];
        }
        pathReverse[path.Count - 1].nextOnPath = null;
        path = pathReverse;

        //to replace for multiple dest
        Station memStation = destinationArray[0];
        destinationArray[0] = destinationArray[1];
        destinationArray[1] = memStation;
        currentDestination = destinationArray[1];

        DeployTrain(0);
    }*/
}
