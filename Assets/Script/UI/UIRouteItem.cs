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

    public UIManagerInGame inGameUI;

    TrainRoute owner;

    public void SetDisplay(TrainRoute trainRoute)
    {
        owner = trainRoute;

        ClearRessourceDisplay();
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
        SetStopButton();
        //set wagon
        //set display
    }
    public void UpdateDisplay()
    {
        stationDepartName.text = owner.currentStartPoint.name;
        stationArrivalName.text = owner.currentDestination.name;
        ClearRessourceDisplay();
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

    public void ClearRessourceDisplay()
    {
        carriedRessources.Clear();
        for (int i = RessourceDisplay.childCount; i > 0; i--)
            Destroy(RessourceDisplay.GetChild(i - 1).gameObject);
    }

    public void SetStopButton()
    {
        if (owner.toStop)
            stopButton.color = Color.red;
        else
            stopButton.color = Color.white;
    }

    public void StopRoute()
    {
        owner.toStop = !owner.toStop;
        SetStopButton();
    }
}

//ref in trainroute and update if needed (maybe same pour station)
