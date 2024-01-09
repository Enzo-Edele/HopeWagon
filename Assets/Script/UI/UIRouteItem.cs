using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRouteItem : MonoBehaviour
{
    [SerializeField] TMP_Text stationDepartName;
    [SerializeField] TMP_Text stationArrivalName;
    [SerializeField] Transform RessourceDisplay;
    [SerializeField] GameObject routeRessourceItem;
    List<RouteRessourceItem> carriedRessources = new List<RouteRessourceItem>();
    //wagon
    //display
    [SerializeField] Image stopButton;

    public InGameUI inGameUI;

    TrainRoute owner;

    public void SetDisplay(TrainRoute trainRoute)
    {
        owner = trainRoute;
        owner.displayUI = this;
        //set name x2
        stationDepartName.text = owner.currentStartPoint.name;
        stationArrivalName.text = owner.currentDestination.name;
        //set ressource
        int j = 0;
        for(int i = 0; i < owner.RouteRessources.Count; i++)
        {
            if (owner.RouteRessources[i])
            {
                carriedRessources.Add(Instantiate(routeRessourceItem, RessourceDisplay.transform).GetComponent<RouteRessourceItem>());
                carriedRessources[j].Set(i, owner.stockRessources[i]);
                j++;
            }
        }
        //set wagon
        //set display
    }
    public void UpdateDisplay()
    {
        stationDepartName.text = owner.currentStartPoint.name;
        stationArrivalName.text = owner.currentDestination.name;
        carriedRessources.Clear();
        for (int i = 0; i < RessourceDisplay.childCount; i++)
            Destroy(RessourceDisplay.GetChild(i).gameObject);
        int j = 0;
        for (int i = 0; i < owner.RouteRessources.Count; i++)
        {
            if (owner.RouteRessources[i])
            {
                carriedRessources.Add(Instantiate(routeRessourceItem, RessourceDisplay.transform).GetComponent<RouteRessourceItem>());
                carriedRessources[j].Set(i, owner.stockRessources[i]);
                j++;
            }
        }
    }

    public void StopRoute()
    {
        owner.toStop = !owner.toStop;
        if (owner.toStop)
            stopButton.color = Color.red;
        else
            stopButton.color = Color.white;
    }
}

//ref in trainroute and update if needed (maybe same pour station)
