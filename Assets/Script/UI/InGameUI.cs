using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    GameTile currentTileSelected;
    GameTile currentTileHover;

    [SerializeField] GridBoard grid;
    bool isDrag = false;
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

    [SerializeField] GameObject routeCreatorMenu;
    [SerializeField] RectTransform routeCreatorDestinations;
    [SerializeField] TMP_Dropdown newRouteStartDropdown;
    [SerializeField] List<TMP_Dropdown> newRouteDestDropdown = new List<TMP_Dropdown>(); //turn to list ???
    [SerializeField] List<Station> newRoutePath = new List<Station>();

    [SerializeField] GameObject routeMenuList;
    [SerializeField] RectTransform routeList;
    [SerializeField] UIRouteItem routeItemPrefab;
    bool routeMenuIsexpanded = false;

    [SerializeField] GameObject buildMenu;
    [SerializeField] List<Image> dozerButton = new List<Image>();

    GameObject lastActiveMenu;

    [SerializeField] List<UIContratItem> contractsDisplay;

    [SerializeField] GameObject playerRessourceMenu;
    [SerializeField] TMP_Text railQty;
    [SerializeField] TMP_Text stationQty;
    [SerializeField] TMP_Text trainQty;
    [SerializeField] TMP_Text buildModeText;

    [SerializeField] GameObject ressourceItemPrefab;
    [SerializeField] GameObject selectTileMenu;
    //[SerializeField] TMP_Text selectedTileName;
    [SerializeField] TMP_Text selectedTileContent;
    [SerializeField] Transform selectedTileRessourceContent;
    //[SerializeField] List<UIRessourceItem> selectedTileRessourceItem = new List<UIRessourceItem>();

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
        ignore, buildRail, destroyRail, buildStation, destroyStation, destroy
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
        if (tile) {
            if (buildMode == BuildMode.destroyRail) {
                tile.HasRail = false;
            }
            if (buildMode == BuildMode.destroyStation) {
                tile.HasStation = false;
            }
            if (buildMode == BuildMode.destroy) {
                tile.HasRail = false;
                tile.HasStation = false;
            }

            selectTileMenu.SetActive(false);
            if (tile.HasStation && (buildMode != BuildMode.destroyStation || buildMode != BuildMode.destroy))
            {
                OpenStationMenuIndividual(true);
                List<List<bool>> destinationsimports = new List<List<bool>>();
                StationItemIndividual.Initialize(tile.station, tile.station.canExport, tile.station.destinationNameList, destinationsimports);
                selectTileMenu.SetActive(true);
            }
            else if (buildMode == BuildMode.ignore)
            {
                ChangeActionMode(0);
            }
            if(tile.HasIndustry)
                selectTileMenu.SetActive(true);
            tile.UpdateUI(this);

            if (buildMode == BuildMode.buildRail) {
                tile.HasRail = true;
            }
            if (buildMode == BuildMode.buildStation) {
                tile.HasStation = true;
            }
            GameManager.Instance.selectedTile = tile;
        }
        else
        {
            selectTileMenu.SetActive(false);
            ChangeActionMode(0);
        }
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
        ChangeActionMode(0);
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
            //currentTile.HasStation = true;
        }
        if (buildMode == BuildMode.destroyStation) {
            //currentTile.HasStation = false;
        }
        if (buildMode == BuildMode.destroy)
        {
            currentTile.HasRail = false;
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
                OpenRouteMenuList(false);
                break;
            case ActionMode.build:
                OpenActionMenu(false);
                OpenBuildMenu(true);
                OpenStationMenuList(false);
                OpenStationMenuIndividual(false);
                OpenRouteMenuList(false);
                break;
            case ActionMode.station:
                OpenActionMenu(false);
                OpenBuildMenu(false);
                OpenStationMenuList(true);
                OpenStationMenuIndividual(false);
                OpenRouteMenuList(false);
                break;
            case ActionMode.train:
                OpenActionMenu(false);
                OpenBuildMenu(false);
                OpenStationMenuList(false);
                OpenStationMenuIndividual(false);
                OpenRouteMenuList(true);
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
            OpenRouteCreator(false);
            OpenRouteMenuList(false);
        }
    }
    void OpenBuildMenu(bool newState)
    {
        buildMenu.SetActive(newState);
        if (newState) {
            OpenActionMenu(false);
            OpenStationMenuIndividual(false);
            OpenStationMenuList(false);
            OpenRouteCreator(false);
            OpenRouteMenuList(false);
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
            OpenRouteCreator(false);
            OpenRouteMenuList(false);
        }
    }
    public void OpenStationMenuList(bool newState)
    {
        stationMenuList.SetActive(newState);
        if (newState) {
            FillStationListList();
            OpenActionMenu(false);
            OpenBuildMenu(false);
            OpenStationMenuIndividual(false);
            OpenRouteCreator(false);
            OpenRouteMenuList(false);
        }
    }
    public void OpenRouteCreator(bool newState) {
        routeCreatorMenu.SetActive(newState);
        if (newState)
        {
            OpenActionMenu(false);
            OpenBuildMenu(false);
            OpenStationMenuIndividual(false);
            OpenStationMenuList(false);
            OpenRouteMenuList(false);
        }
    }
    public void OpenRouteMenuList(bool newState)
    {
        routeMenuList.SetActive(newState);
        if (newState) {
            FillRouteList();
            OpenActionMenu(false);
            OpenBuildMenu(false);
            OpenStationMenuIndividual(false);
            OpenStationMenuList(false);
            OpenRouteCreator(false);
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
    public void SetDestinationListCreator(string startStationName, List<string> listDestination)
    {
        newRouteStartDropdown.captionText.text = startStationName;
        newRoutePath.Clear();
        for (int i = 2; i < 3; i++) //replace 3 by child count-1
        {
            //delete children
        }
        for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
            if (listDestination[0] == GridBoard.Instance.stationList[i].nameStation)
                newRoutePath.Add(GridBoard.Instance.stationList[i]);
        newRouteDestDropdown[0].options.Clear();
        for (int i = 0; i < listDestination.Count; i++)
        {
            newRouteDestDropdown[0].options.Add(new TMP_Dropdown.OptionData() { text = listDestination[i] });
        }
    }
    void AddDestination()
    {
        //instanciate dropdown list
        //fill with possible destination
        //add to dest list

        //enable minus if list > 0
    }
    void MinusDestination()
    {
        //find in list
        //delete
        //disable minus if list < 1
    }
    public void ChangeDestination(TMP_Dropdown destToChange)
    {
        int destID = -1;
        for(int i = 0; i < newRouteDestDropdown.Count; i++)
        {
            if (destToChange == newRouteDestDropdown[i])
                destID = i;
        }
        if (destID >= 0)
        {
            int menuIndex = newRouteDestDropdown[destID].value;
            string nameDestination = newRouteDestDropdown[destID].options[menuIndex].text;
            for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
                if (nameDestination == GridBoard.Instance.stationList[i].nameStation)
                    newRoutePath[destID] = GridBoard.Instance.stationList[i];
        }
    }
    public void DeployTrain()
    {
        if (currentTileSelected.HasStation)
            currentTileSelected.station.CreateRoute(newRoutePath[0]);
    }
    void FillRouteList()
    {
        for (int i = 0; i < routeList.childCount; i++)
            Destroy(routeList.GetChild(i).gameObject);
        List<TrainRoute> list = GameManager.Instance.gridBoard.routeList;
        for (int i = 0; i < list.Count; i++)
        {
            UIRouteItem item = Instantiate(routeItemPrefab);
            item.inGameUI = this;
            item.SetDisplay(list[i]);
            item.transform.SetParent(routeList, false);
        }
    }
    public void ResizeRouteMenu()
    {
        routeMenuIsexpanded = !routeMenuIsexpanded;
        if (routeMenuIsexpanded)
            routeMenuList.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 700);
        else
            routeMenuList.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 200);
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
        for(int i = 0; i < dozerButton.Count; i++)
            dozerButton[i].color = Color.white;
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
            case BuildMode.destroy:
                buildModeText.text = "Destroy";
                prefabPreview[0].SetActive(false);
                prefabPreview[1].SetActive(false);
                for (int i = 0; i < dozerButton.Count; i++)
                    dozerButton[i].color = Color.red;
                break;
        }
    }

    //fonction contract
    public void updateContractDisplay(List<Contract> contracts)
    {
        for (int i = 0; i < 3; i++)
        {
            contractsDisplay[i].Set(GameManager.Instance.ressourceTypes[contracts[i].requireRessourcesIndex].sprite, contracts[i].requiredDisplayText, contracts[i].rewardIcon, contracts[i].rewardDisplayText);
        }
    }

    //fonction tile info
    public void UpdateTileInfo(string name, string content)
    {
        //selectedTileName.text = name;
        selectedTileContent.text = content;
    }
    public void UpdateItemDisplayList(List<int> imports, List<int> exports, List<int> stock) {
        for(int i = 0; i < GameManager.Instance.ressourceTypes.Count; i++)
        {
            //selectedTileImport[i].nameRessource.text = GameManager.Instance.ressourceTypes[i].nameRessource;
            //selectedTileImport[i].qtyRessource.text = "0";
            //selectedTileExport[i].nameRessource.text = GameManager.Instance.ressourceTypes[i].nameRessource;
            //selectedTileExport[i].qtyRessource.text = "0";
        }
        for(int i = 0; i < imports.Count; i++) {
            //if(stock.Count >= 0)
                //selectedTileImport[imports[i]].qtyRessource.text = "" + stock[imports[i]];
        }
        for (int i = 0; i < exports.Count; i++)
        {
            //if (stock.Count >= 0)
                //selectedTileExport[exports[i]].qtyRessource.text = "" + stock[exports[i]];
        }
    }
    public void UpdateItemDisplayListNew(List<bool> imports, List<bool> exports, List<int> stock)
    {
        for(int j = 0; j < selectedTileRessourceContent.childCount; j++)
        {
            Destroy(selectedTileRessourceContent.GetChild(j).gameObject);
        }
        for (int i = 0; i < imports.Count; i++)
        {
            if (imports[i] || exports[i])
            {
                UIRessourceItem item = Instantiate(ressourceItemPrefab, selectedTileRessourceContent).GetComponent<UIRessourceItem>();
                item.SetItem(i, stock[i], exports[i], imports[i]);
            }
        }
    }
    public void UpdateItemDisplayListNew()
    {
        for (int j = 0; j < selectedTileRessourceContent.childCount; j++)
        {
            Destroy(selectedTileRessourceContent.GetChild(j).gameObject);
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
            //selectedTileImportBIS[i].nameRessource.text = GameManager.Instance.ressourceTypes[i].nameRessource;
            selectedTileImportBIS[i].qtyRessource.text = "0";
            //selectedTileExportBIS[i].nameRessource.text = GameManager.Instance.ressourceTypes[i].nameRessource;
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
