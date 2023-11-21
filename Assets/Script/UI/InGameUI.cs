using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public GridBoard grid;

    GameTile currentTile;
    [SerializeField] GameObject stationMenu;
    [SerializeField] TMP_Text stationStartText;
    [SerializeField] TMP_Dropdown destinationList;

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
        GameTile currentTile = grid.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
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
                destinationList.options.Clear();
                foreach(string option in tile.station.destinationsName) {
                    destinationList.options.Add(new TMP_Dropdown.OptionData() { text = option });
                }
            }
            else {
                stationMenu.SetActive(false);
            }
        }
    }
}
