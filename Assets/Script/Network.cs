using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour
{
    Color networkColor;

    List<Station> networkStationList = new List<Station>();

    public void AddStation(Station station)
    {
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
}
