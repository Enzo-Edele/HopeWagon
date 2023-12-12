using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour
{
    [SerializeField] int indexNetwork;
    [SerializeField] string nameNetwork;
    public Color colorNetwork;

    public List<Station> networkStationList = new List<Station>();

    //touche pour display le network

    public void Initialize()
    {
        indexNetwork = GameManager.Instance.gridBoard.networkNumber++;
        nameNetwork = "Network" + indexNetwork;
        name = "Network" + indexNetwork;
        colorNetwork = GameManager.Instance.colorArrayNetwork[indexNetwork % GameManager.Instance.colorArrayNetwork.Length];
    }
    public void ReInitialize(int i)
    {
        indexNetwork = i;
        nameNetwork = "Network" + indexNetwork;
        name = "Network" + indexNetwork;
        colorNetwork = GameManager.Instance.colorArrayNetwork[indexNetwork % GameManager.Instance.colorArrayNetwork.Length];
    }

    public void ClaimTile(GameTile tile)
    {
        tile.Network = this;
    }
    public void AddStation(Station station)
    {
        for(int i = 0; i < networkStationList.Count; i++)
            if (networkStationList[i] == station)
                return;
        networkStationList.Add(station);
    }
    public void RemoveStation(Station station)
    {
        for(int i = 0; i < networkStationList.Count; i++)
            if (station == networkStationList[i])
                networkStationList.RemoveAt(i);
    }
    public void MergeNetwork(List<Station> stationsToAbsorb)
    {
        for(int i = 0; i < networkStationList.Count; i++)
            for (int j = 0; j < stationsToAbsorb.Count; j++)
                networkStationList[i].AddDestination(stationsToAbsorb[j]);
        for (int i = 0; i < stationsToAbsorb.Count; i++)
            networkStationList.Add(stationsToAbsorb[i]);
    }

    public void RelinkStation()
    {
        for (int i = 0; i < networkStationList.Count; i++) {
            for (int j = i + 1; j < networkStationList.Count; j++) {
                networkStationList[i].AddDestination(networkStationList[j]);
                networkStationList[j].AddDestination(networkStationList[i]);
            }
        }
    }

    public void Delete()
    {
        GameManager.Instance.gridBoard.networkNumber--;
        GameManager.Instance.gridBoard.RemoveNetwork(this);
        Destroy(gameObject);
    }
    //CAS SPLIT
    //quand suppresion de rail prendre les voisins avec rail ET faire pathfinding depuis un vérifier si les autre ont hasPath == true
    //  SINON créer un network depuis le rail orphelin;
}
