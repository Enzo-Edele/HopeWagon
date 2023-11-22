using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    //float speed = 0.2f; to use later to modify progress factor
    int wagonCap;
    Queue<GameTile> path = new Queue<GameTile>();
    GameTile currentTile, nextTile, orientationTile;
    Vector3 positionTile, positionNextTile;
    TileDirection direction;
    DirectionChange directionChange;
    float directionAngleTile, directionAangleNextTile;
    float progress, progressFactor;

    [SerializeField] Transform model;

    //locomotive
    //list of wagon

    //make a factory

    void Start() {
        //Spawn();
    }

    public void Spawn() {
        currentTile = path.Dequeue();
        nextTile = path.Dequeue();
        orientationTile = path.Dequeue();
        progress = 0f;
        PrepareDepart();
    }

    void PrepareDepart()
    {
        positionTile = currentTile.transform.position;
        direction = nextTile.GetNeighborDirection(orientationTile); //
        positionNextTile = nextTile.transform.localPosition + direction.GetHalfVector();
        directionChange = DirectionChange.None;
        directionAngleTile = directionAangleNextTile = direction.GetAngle();
        transform.localRotation = direction.GetRotation();
        transform.position = currentTile.transform.position; //added
        progressFactor = 1f;

        nextTile.Paint(Color.blue);
    }

    void Update()
    {
        GameUpdate();
    }

    public bool GameUpdate()
    {
        progress += Time.deltaTime * progressFactor;
        while(progress >= 1f) {
            if(nextTile == null) {
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
        currentTile = nextTile;
        nextTile = orientationTile;
        if (!(path.Count > 0))
            orientationTile = null;
        else
            orientationTile = path.Dequeue();
        positionTile = positionNextTile;
        if (orientationTile == null)
        {
            PrepareArrival();
            return;
        }
        direction = nextTile.GetNeighborDirection(orientationTile);
        directionChange = direction.GetDirectionChangeTo(nextTile.GetNeighborDirection(orientationTile));
        positionNextTile = nextTile.transform.localPosition + direction.GetHalfVector();
        directionAngleTile = directionAangleNextTile;
        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.Right: PrepareRight(); break;
            case DirectionChange.Left: PrepareLeft(); break;
            //cas de défaut turn around serait une erreur
        }
        currentTile.HidePath();
        nextTile.ShowPath();
    }
    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAangleNextTile = direction.GetAngle();
        model.localPosition = Vector3.zero;
        progressFactor = 1f;
        Debug.Log("prep forward");
    }
    void PrepareRight()
    {
        directionAangleNextTile = directionAngleTile + 90f;
        model.localPosition = new Vector3(-0.5f, 0f);
        transform.localPosition = positionTile + direction.GetHalfVector();
        progressFactor = 1f / (Mathf.PI * 0.25f);
        Debug.Log("prep right");
    }
    void PrepareLeft()
    {
        directionAangleNextTile = directionAngleTile - 90f;
        model.localPosition = new Vector3(0.5f, 0f);
        transform.localPosition = positionTile + direction.GetHalfVector();
        progressFactor = 1f / (Mathf.PI * 0.25f);
        Debug.Log("prep left");
    }
    //turn around not use here
    void PrepareArrival()
    {
        positionNextTile = currentTile.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAangleNextTile = direction.GetAngle();
        model.localPosition = Vector3.zero;
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f;
    }
    //on créer une copie des tile pour conserver les infos du bon pathfind (passer par une fake class qui prend seulement le nécesaire)
    public void SetPath(Queue<GameTile> newPath)
    {
        path = newPath;
    }
}
