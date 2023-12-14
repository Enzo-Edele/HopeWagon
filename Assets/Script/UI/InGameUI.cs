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

    [SerializeField] GameObject actionMenu;

    [SerializeField] GameObject stationMenuIndividual;
    [SerializeField] UIStationItem StationItemIndividual;

    [SerializeField] GameObject stationMenuList;
    [SerializeField] RectTransform stationList;
    [SerializeField] UIStationItem stationItemPrefab;
    bool stationMenuIsexpanded = false;

    [SerializeField] GameObject buildMenu;

    GameObject lastActiveMenu;

    [SerializeField] List<TMP_Text> contractsText;

    [SerializeField] GameObject playerRessourceMenu;
    [SerializeField] TMP_Text railQty;
    [SerializeField] TMP_Text stationQty;
    [SerializeField] TMP_Text trainQty;
    [SerializeField] TMP_Text buildModeText;

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

    enum ActionMode
    {
        ignore, build, station, train
    }
    ActionMode actionMode;

    enum BuildMode
    {
        ignore, buildRail, destroyRail, buildStation, destroyStation
    }
    BuildMode buildMode;

    //add destriy station

    public List<GameObject> prefabPreview = new List<GameObject>();

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

            if (Input.GetKeyDown(KeyCode.T))
                OpenStationMenuList(true);
            if (Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftAlt))
                OpenStationMenuList(false);
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
            if(tile.station != null && buildMode != BuildMode.destroyStation) {
                OpenStationMenuIndividual(true);
                List<List<bool>> destinationsimports = new List<List<bool>>();
                StationItemIndividual.Initialize(tile.station, tile.station.canExport, tile.station.destinationNameList, destinationsimports);
            }
            else if(buildMode == BuildMode.ignore){
                OpenActionMenu(true);
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
        /*currentTileSelected = GridBoard.Instance.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (currentTileSelected)
        {
            DoSelectionBIS(currentTileSelected);
        }*/

        OpenActionMenu(true);
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

    public void ChangeActionMode(int newMode)
    {
        if ((int)actionMode == newMode)
            actionMode = (ActionMode)0;
        else
            actionMode = (ActionMode)newMode;
        switch (actionMode)
        {
            case ActionMode.ignore:
                OpenActionMenu(true);
                OpenBuildMenu(false);
                OpenStationMenuList(false);
                OpenStationMenuIndividual(false);
                break;
            case ActionMode.build:
                OpenActionMenu(false);
                OpenBuildMenu(true);
                OpenStationMenuList(false);
                OpenStationMenuIndividual(false);
                break;
            case ActionMode.station:
                OpenActionMenu(false);
                OpenBuildMenu(false);
                OpenStationMenuList(true);
                OpenStationMenuIndividual(false);
                break;
            case ActionMode.train:
                OpenActionMenu(false);
                OpenBuildMenu(false);
                OpenStationMenuList(false);
                OpenStationMenuIndividual(false);
                break;
        }
    }

    public void OpenActionMenu(bool newState)
    {
        actionMenu.SetActive(newState);
        if (newState) {
            OpenBuildMenu(false);
            OpenStationMenuIndividual(false);
            OpenStationMenuList(false);
        }
    }
    void OpenBuildMenu(bool newState)
    {
        buildMenu.SetActive(newState);
        if (newState) {
            OpenActionMenu(false);
            OpenStationMenuIndividual(false);
            OpenStationMenuList(false);
        }
        if (!newState) {
            ChangeBuildMode(0);
        }
    }
    void OpenStationMenuIndividual(bool newState)
    {
        stationMenuIndividual.SetActive(newState);
        if (newState) {
            OpenActionMenu(false);
            OpenBuildMenu(false);
            OpenStationMenuList(false);
        }
    }
    public void OpenStationMenuList(bool newState)
    {
        stationMenuList.SetActive(newState);
        FillStationListList();
        if (newState) {
            OpenActionMenu(false);
            OpenBuildMenu(false);
            OpenStationMenuIndividual(false);
        }
    }
    void FillStationListList()
    {
        for(int i = 0; i < stationList.childCount; i++)
            Destroy(stationList.GetChild(i).gameObject);
        List<Station> list = GameManager.Instance.gridBoard.stationList;
        for (int i = 0; i < list.Count; i++)
        {
            UIStationItem item = Instantiate(stationItemPrefab);
            item.inGameUI = this;
            List<List<bool>> stationsImports = new List<List<bool>>();
            for (int j = 0; j < list[i].destinationList.Count; j++)
                stationsImports.Add(list[i].destinationList[j].canImport);
            item.Initialize(list[i], list[i].canExport, list[i].destinationNameList, stationsImports);
            item.transform.SetParent(stationList, false);
        }
    }
    public void ResizeStationMenu()
    {
        stationMenuIsexpanded = !stationMenuIsexpanded;
        if (stationMenuIsexpanded)
            stationMenuList.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 700);
        else
            stationMenuList.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 200);
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

    public void UpdatePlayerData(int rail, int station, int train)
    {
        railQty.text = "" + rail;
        stationQty.text = "" + station;
        trainQty.text = "" + train;
    }

    public void ChangeBuildMode(int newMode)
    {
        if ((int)buildMode == newMode)
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

    //fonction contract
    public void updateContractDisplay(List<Contract> contracts)
    {
        for (int i = 0; i < 3; i++)
        {
            contractsText[i].text = "" + GameManager.Instance.ressourceTypes[contracts[i].requireRessourcesIndex].name + " "
                + contracts[i].accumulated + " / " + contracts[i].required + " -> " + contracts[i].reward + " " + contracts[i].rewardQty;
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

    //toscrap
    void DoSelectionBIS(GameTile tile)
    {
        if (tile) {
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
