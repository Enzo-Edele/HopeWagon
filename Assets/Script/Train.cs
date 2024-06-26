using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Train : MonoBehaviour
{
    //float speed = 0.2f; to use later to modify progress factor
    [SerializeField] List<GameTileCopy> path = new List<GameTileCopy>();
    GameTileCopy currentTile, nextTile;
    Vector3 positionTile, positionNextTile;
    TileDirection direction;
    DirectionChange directionChange;
    float directionAngleTile, directionAangleNextTile;
    float progress, progressFactor;

    [SerializeField] Transform model;

    //locomotive
    Queue<GameObject> wagonQueue = new Queue<GameObject>();
    TrainRoute trainRoute;

    public List<bool> RouteRessources;

    public int storage = 12;
    public List<int> stockRessources;

    //make a factory

    [SerializeField] TMP_Text stockDisplay;

    //bool first = false;
    bool spawnWagon = false;

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
    public void Spawn(Station depart, Station destination, TrainRoute route) {
        List<GameTileCopy> pathToCopy = new List<GameTileCopy>();

        Queue<GameTile> pathFind = GridBoard.Instance.Pathfinding(destination.tile, depart.tile);
        GameTile tileToCopy = pathFind.Dequeue();
        pathToCopy.Add(Instantiate(GameManager.Instance.tileCopy, gameObject.transform).GetComponent<GameTileCopy>());
        pathToCopy[0].SetUpTileCopy(tileToCopy.tileCoordinate, tileToCopy.transform.position, tileToCopy.distance, tileToCopy.pathDirection, tileToCopy.exitPoint);
        int i = 0;
        while (pathFind.Count > 0)
        {
            i++;
            GameTile tileTC = pathFind.Dequeue();
            pathToCopy.Add(Instantiate(GameManager.Instance.tileCopy, gameObject.transform).GetComponent<GameTileCopy>()); //referenc� la gametilecopy dans une factory
            pathToCopy[i].SetUpTileCopy(tileTC.tileCoordinate, tileTC.transform.position, tileTC.distance, tileTC.pathDirection, tileTC.exitPoint);
            pathToCopy[i - 1].SetUpTileCopyNext(pathToCopy[i]);
        }

        SetPath(pathToCopy);

        currentTile = pathToCopy[0];
        nextTile = pathToCopy[0].nextOnPath;
        progress = 0f;
        trainRoute = route;
        PrepareDepart();
    }

    void PrepareDepart() {
        positionTile = currentTile.tilePosition;
        positionNextTile = currentTile.exitPoint;
        direction = currentTile.pathDirection; 
        directionChange = DirectionChange.None;
        directionAngleTile = directionAangleNextTile = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;
    }

    private void OnMouseDown()
    {
        GameManager.Instance.gameUI.ChangeActionMode(4);
        GameManager.Instance.gameUI.SetIndividualRoute(trainRoute);
    }

    void Update()
    {
        GameUpdate();
    }

    public bool GameUpdate()
    {
        progress += Time.deltaTime * progressFactor;
        while(progress >= 1f) {
            if (nextTile == null) {
                trainRoute.UnloadRessource(currentTile);
                /*for (int i = 0; i < RouteRessources.Count; i++)
                {
                    if (RouteRessources[i])
                    {
                        GameManager.Instance.playerData.AddContratProgress(i, stockRessources[i]);
                        stockRessources[i] = GridBoard.Instance.GetTile(currentTile.tileCoordinate).station.UnloadRessources(stockRessources[i], i);
                    }
                }*/
                Destroy(gameObject); //make a factory and replace with a reclaim method
                return false;
            }
            
            progress = (progress - 1f) / progressFactor;
            PrepareNextTile();
            progress *= progressFactor;
        }
        if(directionChange == DirectionChange.None)
        {
            transform.localPosition = Vector3.LerpUnclamped(positionTile, positionNextTile, progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(directionAngleTile, directionAangleNextTile, progress);
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }

        //spawn wagon   //marche mal si virage dans spawn
        if (spawnWagon && progress > 0.5f && wagonQueue.Count > 0)
        {
            Wagon wagon = Instantiate(wagonQueue.Dequeue(), new Vector3(0, -10, 0), Quaternion.identity).GetComponent<Wagon>();
            wagon.SetPath(path);
            wagon.Spawn(path[0], trainRoute);
            if (wagonQueue.Count <= 0)
            {
                wagon.isLast = true;
            }
            spawnWagon = false;
            trainRoute.AddWagon(wagon);
        }

        return true;
    }
    void PrepareNextTile()
    {
        if (wagonQueue.Count > 0)
            spawnWagon = true;

        currentTile = nextTile;
        nextTile = nextTile.nextOnPath;
        positionTile = positionNextTile;
        if(nextTile == null)
        {
            PrepareArrival();
            return;
        }
        positionNextTile = currentTile.exitPoint;
        directionChange = direction.GetDirectionChangeTo(currentTile.pathDirection);
        direction = currentTile.pathDirection;
        directionAngleTile = directionAangleNextTile;
        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.Right: PrepareRight(); break;
            case DirectionChange.Left: PrepareLeft(); break;
            //cas de d�faut turn around serait une erreur
        }
    }
    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAangleNextTile = direction.GetAngle();
        model.localPosition = Vector3.zero;
        progressFactor = 1f;
    }
    void PrepareRight()
    {
        directionAangleNextTile = directionAngleTile + 90f;
        model.localPosition = new Vector3(-0.5f, 0f);
        transform.localPosition = positionTile + direction.GetHalfVector();
        progressFactor = 1f / (Mathf.PI * 0.25f);
    }
    void PrepareLeft()
    {
        directionAangleNextTile = directionAngleTile - 90f;
        model.localPosition = new Vector3(0.5f, 0f);
        transform.localPosition = positionTile + direction.GetHalfVector();
        progressFactor = 1f / (Mathf.PI * 0.25f);
    }
    //turn around not use here
    void PrepareArrival()
    {
        positionNextTile = currentTile.tilePosition;
        directionChange = DirectionChange.None;
        directionAangleNextTile = direction.GetAngle();
        model.localPosition = Vector3.zero;
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;
    }
    //on cr�er une copie des tile pour conserver les infos du bon pathfind
    public void SetPath(List<GameTileCopy> newPath)
    {
        path = newPath;
    }
    public void SetWagons(List<GameObject> wagonLoad)
    {
        for(int i = 0; i < wagonLoad.Count; i++)
        {
            wagonQueue.Enqueue(wagonLoad[i]);
        }
    }

    public void LoadingStopRoute()
    {
        Destroy(gameObject);
    }
}
