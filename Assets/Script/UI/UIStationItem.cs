using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public InGameUI inGameUI;

    Station departStation;
    Station selectedDestination;

    public void Initialize(Station itemOwner, List<bool>stationExports,
                           /*List<TrainRoute> trainRoutes, List<string>destinationTrains, List<List<int>> trainsRessources,*/ 
                           List<string>destinationArray , List<List<bool>> destinationsImports
    ) {
        departStation = itemOwner;
        stationName.text = departStation.nameStation;

        //stationExportImport //activate the right icon
        //add trainRoutes to dropDown
        //stock the destination of each train
        //stock the ressource of each train

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
    }

    public void DeployTrain()
    {
        if (selectedDestination)
            departStation.CreateRoute(selectedDestination);
    }
}
