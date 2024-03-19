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

    //parent said copy tile heto object

    public List<bool> RouteRessources;
    public List<int> stockRessources;
    int storage = 12;   //to update when implementing wagon

    public bool toStop = false;

    Train train;
    List<Wagon> wagonArray = new List<Wagon>();

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
            DeployTrain();
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
        DeployTrain();

        /*adapt for multiple station
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
        }*/
    }
    public void InitializeMultiple(List<List<GameTileCopy>> newPath, Station depart, List<Station> destination)
    {
        destinationArray.Add(depart);
        for(int j = 0; j < destination.Count; j++)
            destinationArray.Add(destination[j]);

        currentStartPoint = depart;
        currentDestination = destination[0];
        DeployTrain();
    }

    public void DeployTrain()
    {
        if (GameManager.Instance.playerData.trainStock > 0)
        {
            if (toStop)
            {
                stopRoute();
                return;
            }
            train = Instantiate(GameManager.Instance.trainPrefab, new Vector3(0, -10, 0), Quaternion.identity).GetComponent<Train>();
            //train.SetPath(path);
            //train.Spawn(path[pathIndex], this);
            train.Spawn(destinationArray[0], destinationArray[1], this);
            currentStartPoint.LoadTrain(currentDestination, this);
            train.SetWagons(GameManager.Instance.wagonTemplate);
            timerTrain = 0;
            GameManager.Instance.playerData.ChangeTrainStock(-1);
        }
        else
            timerTrain = timeTrain;
    }
    public void SetNextPath() //to update with for loop
    {
        /*List<GameTileCopy> memPath = new List<GameTileCopy>();
        memPath = path;
        path = pathReverse;
        pathReverse = memPath;*/

        Station memStation;
        for(int i = 0; i < destinationArray.Count - 1; i++)
        {
            memStation = destinationArray[i];
            destinationArray[i] = destinationArray[i + 1];
            destinationArray[i + 1] = memStation;
        }

        currentStartPoint = destinationArray[0];
        currentDestination = destinationArray[1];

        DeployTrain();
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

    public void AddWagon(Wagon wagon)
    {
        for (int i = 0; i < wagonArray.Count; i++)
            if (wagonArray[i] == wagon)
                return;
        wagonArray.Add(wagon);
    }
    public void RemoveWagon(Wagon wagon)
    {
        for (int i = 0; i < wagonArray.Count; i++)
            if (wagon == wagonArray[i])
                wagonArray.RemoveAt(i);
    }
    public void ClearWagon()
    {
        for (int i = 0; i < wagonArray.Count; i++)
            Destroy(wagonArray[i].gameObject);
        wagonArray.Clear();
    }

    void stopRoute() {
        GameManager.Instance.gridBoard.RemoveRoute(this);
        Destroy(gameObject);
    }
    public void LoadingStopRoute() {
        if(train)
            train.LoadingStopRoute();
        for (int i = 0; i < wagonArray.Count; i++)
            Destroy(wagonArray[i].gameObject);
        Destroy(gameObject);
    }
}
