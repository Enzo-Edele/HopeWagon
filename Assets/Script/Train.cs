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
    int wagonCap;

    public List<bool> RouteRessources;

    public int storage = 12;
    public List<int> stockRessources;

    //make a factory

    [SerializeField] TMP_Text stockDisplay;

    private void Awake()
    {
        stockRessources = new List<int>();
        RouteRessources = new List<bool>();
        for (int i = 0; i < GameManager.Instance.ressourceSample.Count; i++)
        {
            RouteRessources.Add(false);
            stockRessources.Add(0);
        }
    }
    public void Spawn(GameTileCopy tile) {
        currentTile = tile;
        nextTile = tile.nextOnPath;
        progress = 0f;
        PrepareDepart();
    }

    void PrepareDepart()
    {
        positionTile = currentTile.tilePosition;
        positionNextTile = currentTile.exitPoint;
        direction = currentTile.pathDirection; 
        directionChange = DirectionChange.None;
        directionAngleTile = directionAangleNextTile = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;
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
                /*for (int i = 0; i < path.Count; i++) //faire une copy factory
                {
                    //Destroy(path[i].gameObject); //déplacer dans le last wagon
                }*/
                //give destination as var
                for (int i = 0; i < RouteRessources.Count; i++)
                {
                    if (RouteRessources[i])
                    {
                        stockRessources[i] = GridBoard.Instance.GetTile(currentTile.tileCoordinate).station.UnloadRessources(stockRessources[i], i);
                        Debug.Log("Unloading");
                    }
                }
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
        return true;
    }
    void PrepareNextTile()
    {
        //spawn wagon
        //IMPORTANT FIX pour éviter que le premier wagon chevauche la locomotive instancier a 0.5f de progress aprés le premier
        if(wagonQueue.Count > 0)
        {
            Wagon wagon = Instantiate(wagonQueue.Dequeue()).GetComponent<Wagon>();
            wagon.SetPath(path); 
            wagon.Spawn(path[0]);
            if (wagonQueue.Count <= 0) {
                wagon.isLast = true;
            }
        }

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
            //cas de défaut turn around serait une erreur
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
    //on créer une copie des tile pour conserver les infos du bon pathfind
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
    public int LoadRessources(int qty, int index)
    {
        int leftover = 0;
        Debug.Assert(qty >= 0, "WARNING : can't load negative value on train");
        stockRessources[index] += qty;
        RouteRessources[index] = true;

        if (stockRessources[index] > storage) {
            leftover = stockRessources[index] - storage;
            stockRessources[index] = storage;
        }

        Debug.Log(stockRessources[index] + " loaded out of " + qty + " remaining " + leftover);

        stockDisplay.text = "" + stockRessources[index];

        return leftover;
    }
}
