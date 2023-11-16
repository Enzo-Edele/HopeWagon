using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public string nameStation = "Montparnasse";
    public GameTile tile { get; private set; }
    //the interraction with station will be using the gameUI and it's assigned tile
    //fonction to check and add valid stations as destination

    public List<Station> destinations = new List<Station>();

    public void SetTile(GameTile newTile)
    {
        tile = newTile;
    }

    private void Start()
    {
        name = GameManager.Instance.StationNameGenerator[GameManager.Instance.nameIndex];
        GameManager.Instance.nameIndex += 1;
        if (GameManager.Instance.nameIndex > GameManager.Instance.StationNameGenerator.Length)
        {
            GameManager.Instance.nameIndex = 0;
            GameManager.Instance.NameLooped += 1;
        }
        destinations = GameManager.Instance.gridBoard.GetStationInNetwork(tile);
    }

    public void AddDestination(Station station)
    {
        destinations.Add(station);
    }

    public void RemoveDestination(Station station)
    {
        for (int i = 0; i < destinations.Count; i++)
        {
            if (destinations[i] == station)
            {
                destinations.RemoveAt(i);
            }
        }
    }

    public void DeployTrain(/*Train train, */Station destination)
    {
        Queue<GameTile> path = new Queue<GameTile>();
        path = GameManager.Instance.gridBoard.Pathfinding(destination.tile, tile);
    }
}
