using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class InGameUI : MonoBehaviour
{
    GameTile currentTile;
    [SerializeField] GameObject stationMenu;
    [SerializeField] TMP_Text stationStartText;
    [SerializeField] TMP_Dropdown destinationDropdown;

    //Unit

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
                HandleInput();
            /*else if (selectedUnit)
            {
                if (Input.GetMouseButtonDown(1))
                    //DoMove();
                else
                    //DoPathFinding();
            }*/
        }
    }

    void HandleInput()
    {
        currentTile = GridBoard.Instance.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (currentTile)
        {
            DoSelection(currentTile);
        }
    }
    void DoSelection(GameTile tile)
    {
        if (tile)
        {
            if(tile.station != null) {
                stationMenu.SetActive(true);
                stationStartText.text = tile.station.name;
                destinationDropdown.options.Clear();
                foreach(string option in tile.station.destinationNameList) {
                    destinationDropdown.options.Add(new TMP_Dropdown.OptionData() { text = option });
                }
            }
            else {
                stationMenu.SetActive(false);
            }
        }
    }
    public void SelectDestination()
    {
        int menuIndex = destinationDropdown.value;
        string nameDestination = destinationDropdown.options[menuIndex].text;
        Station destination = null;
        for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
            if (nameDestination == GridBoard.Instance.stationList[i].nameStation)
                destination = GridBoard.Instance.stationList[i];
        if(destination)
            currentTile.station.DeployTrain(destination);
    }
}
