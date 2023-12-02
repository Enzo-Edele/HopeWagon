using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class InGameUI : MonoBehaviour
{
    GameTile currentTile;
    Station destination = null;

    public GameObject ressourceItem;

    [SerializeField] GameObject stationMenu;
    [SerializeField] TMP_Text stationStartText;
    [SerializeField] TMP_Dropdown destinationDropdown;

    [SerializeField] GameObject selectTileMenu;
    [SerializeField] TMP_Text selectedTileName;
    [SerializeField] TMP_Text selectedTileContent;
    [SerializeField] List<UIRessourceItem> selectedTileImport = new List<UIRessourceItem>();
    [SerializeField] List<UIRessourceItem> selectedTileExport = new List<UIRessourceItem>();

    //Unit

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
                HandleInput();
            if (Input.GetMouseButtonDown(1))
                Unselect();
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
            selectTileMenu.SetActive(true);
            tile.UpdateUI(this);
        }
        else
            selectTileMenu.SetActive(false);
    }
    void Unselect()
    {
        //to remove debug code
        currentTile = GridBoard.Instance.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (currentTile)
        {
            //update panel info 2
        }
        //hide current tile info panel
    }
    public void SelectDestination()
    {
        int menuIndex = destinationDropdown.value;
        string nameDestination = destinationDropdown.options[menuIndex].text;
        for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
            if (nameDestination == GridBoard.Instance.stationList[i].nameStation)
                destination = GridBoard.Instance.stationList[i];
    }
    public void DeployTrain()
    {
        if (destination)
            currentTile.station.DeployTrain(destination);
    }

    //fonction tile info
    public void UpdateTileInfo(string name, string content)
    {
        selectedTileName.text = name;
        selectedTileContent.text = content;
    }
    public void CreateItemDisplayList(List<int> imports, List<int> exports, List<int> stock) {
        for(int i = 0; i < GameManager.Instance.ressourceSample.Count; i++)
        {
            selectedTileImport[i].qtyRessource.text = "";
            selectedTileExport[i].qtyRessource.text = "";
        }
        for(int i = 0; i < imports.Count; i++) {
            selectedTileImport[imports[i]].nameRessource.text = GameManager.Instance.ressourceSample[imports[i]].nameRessource;
            selectedTileImport[imports[i]].nameRessource.text = "" + stock[imports[i]];
        }
        for (int i = 0; i < exports.Count; i++)
        {
            selectedTileExport[exports[i]].nameRessource.text = GameManager.Instance.ressourceSample[exports[i]].nameRessource;
            selectedTileExport[exports[i]].nameRessource.text = "" + stock[exports[i]];
        }
    }
}
