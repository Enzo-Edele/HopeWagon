using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Station : MonoBehaviour
{
    public string nameStation = "Montparnasse";
    [SerializeField] TMP_Text nameDisplay;
    public GameTile tile { get; private set; }
    //the interraction with station will be using the gameUI and it's assigned tile
    //fonction to check and add valid stations as destination

    public List<Station> destinationList = new List<Station>(); //[SerializeField]
    public List<string> destinationNameList = new List<string>();
    //value that index last used path destinationIndex

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

    private void Start()
    {
        name = GameManager.Instance.GiveStationName();
        nameStation = name;
        nameDisplay.text = nameStation;
        destinationList = GridBoard.Instance.GetStationInNetwork(tile);
        for (int i = 0; i < destinationList.Count; i++)
            destinationNameList.Add(destinationList[i].name);
        GridBoard.Instance.stationList.Add(this);
    }

    public void AddDestination(Station station)
    {
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
    }
}
