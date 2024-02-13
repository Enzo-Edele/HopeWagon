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
        destinationDropdown.options.Clear();
        for(int i = 0; i < destinationArray.Count; i++)
        {
            destinationDropdown.options.Add(new TMP_Dropdown.OptionData() { text = destinationArray[i] });
        }
        if (destinationArray.Count > 0)
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
            for (int i = 0; i < selectedDestination.canImport.Count; i++)
            {
                if (selectedDestination.canImport[i])
                {
                    Image icon = Instantiate(imagePrefab, destinationImport.transform).GetComponent<Image>();
                    icon.sprite = GameManager.Instance.ressourceTypes[i].sprite;
                }
            }
            destinationDropdown.captionText.text = selectedDestination.name;
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

    public void DeployTrain()
    {
        /*if (selectedDestination)
        {
            GameManager.Instance.gameUI.OpenRouteCreator(true);
            GameManager.Instance.gameUI.SetDestinationListCreator(stationName.text, destinationsNames);
        }*/
        departStation.CreateRoute(selectedDestination);
    }
}
