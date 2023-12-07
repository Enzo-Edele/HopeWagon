using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class InGameUI : MonoBehaviour
{
    GameTile currentTile;
    Station destination = null;

    [SerializeField] GridBoard grid;
    bool isDrag;
    TileDirection dragDirection;
    GameTile previousTile;

    //UI Gameobject

    public GameObject ressourceItem;

    //UI Element

    [SerializeField] GameObject stationMenu;
    [SerializeField] TMP_Text stationStartText;
    [SerializeField] TMP_Dropdown destinationDropdown;

    [SerializeField] GameObject buildMenu;

    [SerializeField] GameObject selectTileMenu;
    [SerializeField] TMP_Text selectedTileName;
    [SerializeField] TMP_Text selectedTileContent;
    [SerializeField] List<UIRessourceItem> selectedTileImport = new List<UIRessourceItem>();
    [SerializeField] List<UIRessourceItem> selectedTileExport = new List<UIRessourceItem>();

    [SerializeField] GameObject selectTileMenuBIS;
    [SerializeField] TMP_Text selectedTileNameBIS;
    [SerializeField] TMP_Text selectedTileContentBIS;
    [SerializeField] List<UIRessourceItem> selectedTileImportBIS = new List<UIRessourceItem>();
    [SerializeField] List<UIRessourceItem> selectedTileExportBIS = new List<UIRessourceItem>();

    [SerializeField] GameObject playerRessourceMenu;
    [SerializeField] TMP_Text railQty;
    [SerializeField] TMP_Text stationQty;
    [SerializeField] TMP_Text trainQty;
    [SerializeField] TMP_Text buildMode;

    [SerializeField] List<TMP_Text> contractsText;

    //MapEdit

    bool buildingRail;
    bool destroyingRail;
    bool buildingStation;
    //string buildMode; //use to keep track of build mode

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
                HandleInput();
            if (Input.GetMouseButtonDown(1))
                Unselect();
            if (Input.GetMouseButton(0))
            {
                HandleInputDrag();
                return;
            }

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
                buildMenu.SetActive(false);
                stationStartText.text = tile.station.name;
                destinationDropdown.options.Clear();
                foreach(string option in tile.station.destinationNameList) {
                    destinationDropdown.options.Add(new TMP_Dropdown.OptionData() { text = option });
                }
                int menuIndex = destinationDropdown.value;
                string nameDestination = destinationDropdown.options[menuIndex].text;
                for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
                    if (nameDestination == GridBoard.Instance.stationList[i].nameStation)
                        destination = GridBoard.Instance.stationList[i];
                destinationDropdown.captionText.text = destination.name;
            }
            else {
                stationMenu.SetActive(false);
                buildMenu.SetActive(true);
            }
            selectTileMenu.SetActive(true);
            tile.UpdateUI(this);
            GameManager.Instance.selectedTile = tile;
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
            DoSelectionBIS(currentTile);
        }

        stationMenu.SetActive(false);
        buildMenu.SetActive(true);
        //currentTile = null;
        //selectTileMenu.SetActive(false);
    }

    void HandleInputDrag()
    {
        GameTile currentTile = grid.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (currentTile)
        {
            if (previousTile && previousTile != currentTile)
                ValidateDrag(currentTile);
            else
                isDrag = false;
            EditTiles(currentTile);
            previousTile = currentTile;
        }
        else
            previousTile = null;
    }
    void EditTiles(GameTile currentTile)
    {
        if (buildingRail)
            currentTile.HasRail = true;
        if (destroyingRail)
            currentTile.HasRail = false;
        if (buildingStation)
            currentTile.HasStation = true;
    }
    void ValidateDrag(GameTile currentGameTile)
    {
        for (dragDirection = TileDirection.north; dragDirection <= TileDirection.west; dragDirection++)
        {
            if (previousTile.GetNeighbor(dragDirection) == currentGameTile)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
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
        if (destination) {
            currentTile.station.DeployTrain(destination);
            GameManager.Instance.playerData.ChangeTrainStock(-1);
        }
    }

    //fonction tile info
    public void UpdateTileInfo(string name, string content)
    {
        selectedTileName.text = name;
        selectedTileContent.text = content;
    }
    public void UpdateItemDisplayList(List<int> imports, List<int> exports, List<int> stock) {
        for(int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++)
        {
            selectedTileImport[i].nameRessource.text = GameManager.Instance.ressourceTypes[i].nameRessource;
            selectedTileImport[i].qtyRessource.text = "0";
            selectedTileExport[i].nameRessource.text = GameManager.Instance.ressourceTypes[i].nameRessource;
            selectedTileExport[i].qtyRessource.text = "0";
        }
        for(int i = 0; i < imports.Count; i++) {
            if(stock.Count >= 0)
                selectedTileImport[imports[i]].qtyRessource.text = "" + stock[imports[i]];
        }
        for (int i = 0; i < exports.Count; i++)
        {
            if (stock.Count >= 0)
                selectedTileExport[exports[i]].qtyRessource.text = "" + stock[exports[i]];
        }
    }

    public void UpdatePlayerData(int rail, int station, int train)
    {
        railQty.text = "" + rail;
        stationQty.text = "" + station;
        trainQty.text = "" + train;
    }

    public void BuildRail()
    {
        buildingRail = !buildingRail;
        if (buildingRail)
        {
            buildingStation = false;
            buildMode.text = "Build Rail";
            destroyingRail = false;
        }
        else if (buildingStation)
        {
            buildMode.text = "Build Station";
        }
        else if(!buildingRail)
            buildMode.text = "Selection";
    }
    public void BuildStation()
    {
        buildingStation = !buildingStation;
        if (buildingStation)
        {
            buildingRail = false;
            buildMode.text = "Build Station";
            destroyingRail = false;
        }
        else if (buildingRail)
        {
            buildMode.text = "Build Rail";
        }
        else if (!buildingStation)
            buildMode.text = "Selection";
    }
    public void DestroyRail()
    {
        destroyingRail = !destroyingRail;
        if (destroyingRail)
        {
            buildingRail = false;
            buildMode.text = "Destroy Rail";
            buildingStation = false;
        }
        else if (!destroyingRail)
            buildMode.text = "Selection";
    }

    public void updateContractDisplay(List<Contract> contracts)
    {
        for(int i = 0; i < 3; i++) {
            contractsText[i].text = "" + GameManager.Instance.ressourceTypes[contracts[i].requireRessourcesIndex].name + " " 
                + contracts[i].accumulated + " / " + contracts[i].required + " -> " + contracts[i].reward + " " + contracts[i].rewardQty;
        }
    }

    //toscrap
    void DoSelectionBIS(GameTile tile)
    {
        if (tile) {
            if (tile.station != null) {
                stationMenu.SetActive(true);
                stationStartText.text = tile.station.name;
                destinationDropdown.options.Clear();
                foreach (string option in tile.station.destinationNameList) {
                    destinationDropdown.options.Add(new TMP_Dropdown.OptionData() { text = option });
                }
            }
            else {
                stationMenu.SetActive(false);
            }
            selectTileMenuBIS.SetActive(true);
            tile.UpdateUIBIS(this);
            GameManager.Instance.selectedTileBIS = tile;
        }
        else
            selectTileMenuBIS.SetActive(false);
    }
    public void UpdateTileInfoBIS(string name, string content)
    {
        selectedTileNameBIS.text = name;
        selectedTileContentBIS.text = content;
    }
    public void UpdateItemDisplayListBIS(List<int> imports, List<int> exports, List<int> stock)
    {
        for (int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++) {
            selectedTileImportBIS[i].nameRessource.text = GameManager.Instance.ressourceTypes[i].nameRessource;
            selectedTileImportBIS[i].qtyRessource.text = "0";
            selectedTileExportBIS[i].nameRessource.text = GameManager.Instance.ressourceTypes[i].nameRessource;
            selectedTileExportBIS[i].qtyRessource.text = "0";
        }
        for (int i = 0; i < imports.Count; i++) {
            if (stock.Count >= 0)
                selectedTileImportBIS[imports[i]].qtyRessource.text = "" + stock[imports[i]];
        }
        for (int i = 0; i < exports.Count; i++) {
            if (stock.Count >= 0)
                selectedTileExportBIS[exports[i]].qtyRessource.text = "" + stock[exports[i]];
        }
    }
}
