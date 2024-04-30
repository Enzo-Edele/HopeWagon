using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStationItem : MonoBehaviour
{
    [SerializeField] TMP_Text stationName;
    [SerializeField] GameObject stationExportImport;
    [SerializeField] TMP_Dropdown trainDropdown;
    [SerializeField] TMP_Text trainDestination;
    [SerializeField] GameObject trainRessources;
    [SerializeField] TMP_Dropdown destinationDropdown;
    [SerializeField] GameObject destinationImport;
    [SerializeField] GameObject imagePrefab;
    [SerializeField] Sprite tradeIco;
    List<string> destinationsNames = new List<string>();

    public UIManagerInGame inGameUI;

    Station departStation;
    Station selectedDestination;

    public void Initialize(Station itemOwner, List<bool>stationExports,
                           /*List<TrainRoute> trainRoutes, List<string>destinationTrains, List<List<int>> trainsRessources,*/ 
                           List<string>destinationArray , List<List<bool>> destinationsImports
    ) {
        departStation = itemOwner;
        stationName.text = departStation.nameStation;

        //stationExportImport //activate the right icon
        for (int j = 1; j < stationExportImport.transform.childCount; j++) {
            Destroy(stationExportImport.transform.GetChild(j).gameObject);
        }
        for (int i = 0; i < itemOwner.canExport.Count; i++) {
            if (itemOwner.canExport[i]) {
                Image icon = Instantiate(imagePrefab , stationExportImport.transform).GetComponent<Image>();
                icon.sprite = GameManager.Instance.ressourceTypes[i].sprite;
            }
        }
        //add trainRoutes to dropDown
        //stock the destination of each train
        //stock the ressource of each train
        destinationsNames = destinationArray;

        List<string> matchingDestinations = new List<string>();
        bool match;
        int stationToMark = 0;
        for (int i = destinationsNames.Count - 1; i > -1; i--) {
            match = false;
            for (int j = 0; j < itemOwner.canExport.Count; j++) {
                if (GridBoard.Instance.GetStation(destinationsNames[i]).canImport[j] && itemOwner.canExport[j]) {
                    matchingDestinations.Add(destinationsNames[i]);
                    match = true;
                }
            }
            if (match)
            {
                destinationsNames.RemoveAt(i);
                stationToMark++;
            }
        }
        for (int i = 0; i < matchingDestinations.Count; i++)
        {
            destinationsNames.Insert(0, matchingDestinations[i]);
        }

        destinationDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> destinationItems = new List<TMP_Dropdown.OptionData>();
        for(int i = 0; i < destinationsNames.Count; i++)
        {
            string name = destinationsNames[i];
            var ico = tradeIco;
            if (i < stationToMark)
            {
                var option = new TMP_Dropdown.OptionData(name, ico);
                destinationItems.Add(option);
            }
            else
            {
                var option = new TMP_Dropdown.OptionData(name);
                destinationItems.Add(option);
            }
        }
        destinationDropdown.AddOptions(destinationItems);
        if (destinationsNames.Count > 0)
        {
            int menuIndex = destinationDropdown.value;
            string nameDestination = destinationDropdown.options[menuIndex].text;  //check if still [error when only one station on network]
            for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
                if (nameDestination == GridBoard.Instance.stationList[i].nameStation)
                    selectedDestination = GridBoard.Instance.stationList[i];
            for (int j = 1; j < destinationImport.transform.childCount; j++)
            {
                Destroy(destinationImport.transform.GetChild(j).gameObject);
            }
            destinationDropdown.captionText.text = selectedDestination.name;

            for (int i = 0; i < selectedDestination.canImport.Count; i++)
            {
                if (selectedDestination.canImport[i])
                {
                    Image icon = Instantiate(imagePrefab, destinationImport.transform).GetComponent<Image>();
                    icon.sprite = GameManager.Instance.ressourceTypes[i].sprite;
                }
            }
        }
    }

    public void SelectDestination()
    {
        int menuIndex = destinationDropdown.value;
        string nameDestination = destinationDropdown.options[menuIndex].text;
        for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
            if (nameDestination == GridBoard.Instance.stationList[i].nameStation)
                selectedDestination = GridBoard.Instance.stationList[i];
        //update ressource
        for (int j = 1; j < destinationImport.transform.childCount; j++)
        {
            Destroy(destinationImport.transform.GetChild(j).gameObject);
        }
        for (int i = 0; i < selectedDestination.canImport.Count; i++) {
            if (selectedDestination.canImport[i]) {
                Image icon = Instantiate(imagePrefab, destinationImport.transform).GetComponent<Image>();
                icon.sprite = GameManager.Instance.ressourceTypes[i].sprite;
            }
        }
    }

    public void DeployTrainMultiple()
    {
        if (selectedDestination)
        {
            GameManager.Instance.gameUI.OpenRouteCreator(true);
            GameManager.Instance.gameUI.SetDestinationListCreator(stationName.text, destinationsNames);
        }
    }
    public void DeployTrain()
    {
        departStation.CreateRoute(selectedDestination);
    }

    public void FocusStation()
    {
        GameManager.Instance.cameraController.LockCamOnPos(departStation.cameraFocusPos);
        GameManager.Instance.cameraController.LockCamHeight(5.0f);
    }
}
