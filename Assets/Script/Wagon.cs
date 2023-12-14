using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wagon : MonoBehaviour
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

    //array of ressources

    //make a factory

    public bool isLast = false; //replace by a properly manage train
    TrainRoute transportPath;

    public void Spawn(GameTileCopy tile, TrainRoute route)
    {
        currentTile = tile;
        nextTile = tile.nextOnPath;
        progress = 0f;
        transportPath = route;
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
        while (progress >= 1f)
        {
            if (nextTile == null)
            {
                if (isLast) {
                    //for (int i = 0; i < path.Count; i++)//faire une copy factory
                    //Destroy(path[i].gameObject);
                    transportPath.SetNextPath();
                    GameManager.Instance.playerData.ChangeTrainStock(1);
                }

                Destroy(gameObject); //make a factory and replace with a reclaim method
                return false;
            }
            progress = (progress - 1f) / progressFactor;
            PrepareNextTile();
            progress *= progressFactor;
        }
        if (directionChange == DirectionChange.None)
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
        currentTile = nextTile;
        nextTile = nextTile.nextOnPath;
        positionTile = positionNextTile;
        if (nextTile == null)
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
        //currentTile.HidePath();
        //nextTile.ShowPath();
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
    //on créer une copie des tile pour conserver les infos du bon pathfind (passer par une fake class qui prend seulement le nécesaire)
    public void SetPath(List<GameTileCopy> newPath)
    {
        path = newPath;
    }
}
