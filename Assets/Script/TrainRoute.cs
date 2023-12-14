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
    public List<GameTileCopy> path = new List<GameTileCopy>();
    public List<GameTileCopy> pathReverse = new List<GameTileCopy>();
    //parent said copy tile heto object
    Train train; //a terme on aura des trains custom pour l'instant prendre un prefab

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
        currentDestination = destination;
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
            train = Instantiate(GameManager.Instance.trainPrefab).GetComponent<Train>();
            train.SetPath(path);
            train.Spawn(path[pathIndex], this);
            destinationArray[0].DeployTrain(currentDestination, train);
            train.SetWagons(GameManager.Instance.wagonTemplate);
            timerTrain = 0;
            GameManager.Instance.playerData.ChangeTrainStock(-1);
        }
        else
            timerTrain = timeTrain;
    }
    public void SetNextPath()
    {
        List<GameTileCopy> memPath = new List<GameTileCopy>();
        memPath = path;
        path = pathReverse;
        pathReverse = memPath;

        Station memStation = destinationArray[0];
        destinationArray[0] = destinationArray[1];
        destinationArray[1] = memStation;
        currentDestination = destinationArray[1];

        DeployTrain(0);
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
