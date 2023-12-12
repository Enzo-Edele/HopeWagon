using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class InGameUI : MonoBehaviour
{
    GameTile currentTileSelected;
    GameTile currentTileHover;
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
    [SerializeField] TMP_Text buildModeText;

    [SerializeField] List<TMP_Text> contractsText;

    enum BuildMode
    {
        ignore, buildRail, destroyRail, buildStation, destroyStation
    }
    BuildMode buildMode;

    //MapEdit

    bool buildingRail;
    bool destroyingRail;
    bool buildingStation;
    //add destriy station

    public List<GameObject> prefabPreview = new List<GameObject>();
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
            if(buildMode == BuildMode.buildRail || buildMode == BuildMode.buildStation)
                UpdatePreview();
        }
    }

    void HandleInput()
    {
        currentTileSelected = GridBoard.Instance.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (currentTileSelected)
        {
            DoSelection(currentTileSelected);
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
                if(tile.station.destinationNameList.Count > 1) { 
                    int menuIndex = destinationDropdown.value;
                    string nameDestination = destinationDropdown.options[menuIndex].text;  //error when only one station on network
                    for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
                        if (nameDestination == GridBoard.Instance.stationList[i].nameStation)
                            destination = GridBoard.Instance.stationList[i];
                    destinationDropdown.captionText.text = destination.name;
                }
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
        currentTileSelected = GridBoard.Instance.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (currentTileSelected)
        {
            DoSelectionBIS(currentTileSelected);
        }

        stationMenu.SetActive(false);
        buildMenu.SetActive(true);
        currentTileSelected = null;
        selectTileMenu.SetActive(false);
        ChangeBuildMode(0);
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
        if (buildMode == BuildMode.buildRail) {
            currentTile.HasRail = true;
        }
        if (buildMode == BuildMode.destroyRail) {
            currentTile.HasRail = false;
        }
        if (buildMode == BuildMode.buildStation) {
            currentTile.HasStation = true;
        }
        if (buildMode == BuildMode.destroyStation) {
            currentTile.HasStation = false;
        }
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
            currentTileSelected.station.DeployTrain(destination);
            GameManager.Instance.playerData.ChangeTrainStock(-1);
        }
    }

    void UpdatePreview()
    {
        currentTileHover = GridBoard.Instance.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
        Vector3 pos = new Vector3();
        if (currentTileHover != null)
            pos = currentTileHover.transform.position;
        if (buildMode == BuildMode.buildRail) {
            prefabPreview[0].transform.position = pos;
        }
        if (buildMode == BuildMode.buildStation) {
            prefabPreview[1].transform.position = pos;
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

    public void ChangeBuildMode(int newMode)
    {
        if((int)buildMode == newMode)
            buildMode = (BuildMode)0;
        else
            buildMode = (BuildMode)newMode;
        switch (buildMode)
        {
            case BuildMode.ignore:
                buildModeText.text = "Selection";
                prefabPreview[0].SetActive(false);
                prefabPreview[1].SetActive(false);
                break;
            case BuildMode.buildRail:
                buildModeText.text = "Build Rail";
                prefabPreview[0].SetActive(true);
                prefabPreview[1].SetActive(false);
                break;
            case BuildMode.destroyRail:
                buildModeText.text = "Destroy Rail";
                prefabPreview[0].SetActive(false);
                prefabPreview[1].SetActive(false);
                break;
            case BuildMode.buildStation:
                buildModeText.text = "Build Station";
                prefabPreview[0].SetActive(false);
                prefabPreview[1].SetActive(true);
                break;
            case BuildMode.destroyStation:
                buildModeText.text = "Destroy Station";
                prefabPreview[0].SetActive(false);
                prefabPreview[1].SetActive(false);
                break;
        }
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
