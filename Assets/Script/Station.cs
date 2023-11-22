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
    }

    private void Start()
    {
        name = GameManager.Instance.GiveStationName();
        nameStation = name;
        nameDisplay.text = nameStation;
        destinationList = GameManager.Instance.gridBoard.GetStationInNetwork(tile);
        for (int i = 0; i < destinationList.Count; i++)
            destinationNameList.Add(destinationList[i].name);
        GameManager.Instance.gridBoard.stationList.Add(this);
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
        Queue<GameTile> path = new Queue<GameTile>();
        path = GameManager.Instance.gridBoard.Pathfinding(destination.tile, tile);
        Train train = Instantiate(GameManager.Instance.trainPrefab).GetComponent<Train>();
        train.SetPath(path);
        train.Spawn();
    }
}
