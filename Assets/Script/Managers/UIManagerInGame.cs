using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UIManagerInGame : MonoBehaviour
{
    GameTile currentTileSelected;
    GameTile currentTileHover;

    [SerializeField] GridBoard grid;
    //bool isDrag = false;
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
    [SerializeField] List<TMP_Dropdown> newRouteDestDropdown = new List<TMP_Dropdown>(); 
    [SerializeField] List<Station> newRoutePath = new List<Station>();
    [SerializeField] GameObject dropdownDestPrefab;
    int destNumber;

    [SerializeField] GameObject routeMenuIndividual;
    [SerializeField] UIRouteItem RouteItemIndividual;

    [SerializeField] GameObject routeMenuList;
    [SerializeField] RectTransform routeList;
    [SerializeField] UIRouteItem routeItemPrefab;
    bool routeMenuIsexpanded = false;

    [SerializeField] GameObject buildMenu;
    [SerializeField] List<Image> dozerButton = new List<Image>();

    GameObject lastActiveMenu;

    [SerializeField] GameObject contractPannel;
    [SerializeField] List<UIContratItem> contractsDisplay;

    [SerializeField] GameObject playerRessourceMenu;
    [SerializeField] TMP_Text railQty;
    [SerializeField] TMP_Text stationQty;
    [SerializeField] TMP_Text trainQty;
    [SerializeField] TMP_Text buildModeText;

    [SerializeField] GameObject ressourceItemPrefab;
    [SerializeField] GameObject selectTileMenu;
    //[SerializeField] TMP_Text selectedTileName;
    [SerializeField] TMP_Text pollutionLevel;
    [SerializeField] TMP_Text pollutionLevelMax;
    [SerializeField] TMP_Text pollutionLevelMin;
    [SerializeField] TMP_Text selectedTileContent;
    [SerializeField] Transform selectedTileRessourceContent;
    //[SerializeField] List<UIRessourceItem> selectedTileRessourceItem = new List<UIRessourceItem>();

    //tuto variable
    [SerializeField] GameObject tutoPannel;

    [SerializeField] GameObject stationMenuIndividualTuto;
    [SerializeField] UIStationItem StationItemIndividualTuto;

    [SerializeField] GameObject tutoRessourceOutliner;
    [SerializeField] GameObject tutoContractOutliner;

    [SerializeField] List<Vector2> stationCoordinates = new List<Vector2>();
    [SerializeField] List<Vector2> railCoordinates = new List<Vector2>();

    enum ActionMode
    {
        ignore, build, station, train, route, hidden
    }
    ActionMode actionMode;

    enum BuildMode
    {
        ignore, buildRail, destroyRail, buildStation, destroyStation, destroy
    }
    BuildMode buildMode;

    //add destriy station

    public List<GameObject> prefabPreview = new List<GameObject>();

    public void ActivateInGameUI(bool nState)
    {
        playerRessourceMenu.SetActive(nState);
        contractPannel.SetActive(nState);
        GameManager.Instance.pauseMenu.ActivateTimer(nState);
        if (nState)
            ChangeActionMode(0);
        else
            ChangeActionMode(5);
    }

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
        if (currentTileSelected && !(GameManager.gameState == GameManager.GameState.tuto)) {
            DoSelection(currentTileSelected);
        }
        else if (currentTileSelected) {
            DoSelectionTuto(currentTileSelected);
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

            if(!GameManager.Instance.Debbug)
                selectTileMenu.SetActive(false);
            else
                selectTileMenu.SetActive(true);
            if (tile.HasStation && (buildMode != BuildMode.destroyStation || buildMode != BuildMode.destroy))
            {
                OpenStationMenuIndividual(true);
                List<List<bool>> destinationsimports = new List<List<bool>>();
                StationItemIndividual.Initialize(tile.station, tile.station.canExport, tile.station.destinationNameList, destinationsimports);
                selectTileMenu.SetActive(true);
            }
            else if (buildMode == BuildMode.ignore && actionMode != ActionMode.route)
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
        else if(actionMode != ActionMode.route)
        {
            selectTileMenu.SetActive(false);
            ChangeActionMode(0);
        }
    }
    void DoSelectionTuto(GameTile tile)
    {
        if (tile)
        {
            selectTileMenu.SetActive(false);
            if (tile.HasStation && (buildMode != BuildMode.destroyStation || buildMode != BuildMode.destroy))
            {
                OpenStationMenuIndividualTuto(true);
                List<List<bool>> destinationsimports = new List<List<bool>>();
                StationItemIndividualTuto.Initialize(tile.station, tile.station.canExport, tile.station.destinationNameList, destinationsimports);
                selectTileMenu.SetActive(true);
            }
            else if (buildMode == BuildMode.ignore)
            {
                ChangeActionMode(0);
            }
            tile.UpdateUI(this);

            Vector2 coordCheck = tile.tileCoordinate;

            //check if coordinate matches
            if (buildMode == BuildMode.buildRail)
            {
                for (int i = 0; i < railCoordinates.Count; i++)
                {
                    if (coordCheck == railCoordinates[i])
                    {
                        tile.HasRail = true;
                        TutoManager.Instance.RailPlaced();
                    }
                }
            }
            if (buildMode == BuildMode.buildStation)
            {
                for (int i = 0; i < stationCoordinates.Count; i++)
                {
                    if (coordCheck == stationCoordinates[i])
                    {
                        tile.HasStation = true;
                        TutoManager.Instance.StationPlaced();
                    }
                }
            }
            GameManager.Instance.selectedTile = tile;
        }
        else
        {
            selectTileMenu.SetActive(false);
            ChangeActionMode(0);
        }
    }
    public void SpawnTutoStation() {
        for(int i = 0; i < stationCoordinates.Count; i++) {
            TutoManager.Instance.PlaceStationIndicator(stationCoordinates[i]);
        }
    }
    public void SpawnTutoRail() {
        for (int i = 0; i < railCoordinates.Count; i++) {
            TutoManager.Instance.PlaceRailIndicator(railCoordinates[i]);
        }
    }

    public void Unselect()
    {
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
            //else
            //isDrag = false;
            if (!(GameManager.gameState == GameManager.GameState.tuto))
                EditTiles(currentTile);
            else
                EditTilesTuto(currentTile);
            previousTile = currentTile;
        }
        else
            previousTile = null;
    }
    //use for action using drag
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
    void EditTilesTuto(GameTile currentTile)
    {
        Vector2 coordCheck = currentTile.tileCoordinate;
        if (buildMode == BuildMode.buildRail && !currentTile.HasRail)
        {
            for (int i = 0; i < railCoordinates.Count; i++)
            {
                if (coordCheck == railCoordinates[i])
                {
                    currentTile.HasRail = true;
                    TutoManager.Instance.RailPlaced();
                }
            }
        }
    }
    void ValidateDrag(GameTile currentGameTile)
    {
        for (dragDirection = TileDirection.north; dragDirection <= TileDirection.west; dragDirection++)
        {
            if (previousTile.GetNeighbor(dragDirection) == currentGameTile)
            {
                //isDrag = true;
                return;
            }
        }
        //isDrag = false;
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
                //OpenBuildMenu(false);
                //OpenStationMenuList(false);
                //OpenStationMenuIndividual(false);
                //OpenRouteMenuList(false);
                break;
            case ActionMode.build:
                //OpenActionMenu(false);
                OpenBuildMenu(true);
                //OpenStationMenuList(false);
                //OpenStationMenuIndividual(false);
                //OpenRouteMenuList(false);
                break;
            case ActionMode.station:
                //OpenActionMenu(false);
                //OpenBuildMenu(false);
                OpenStationMenuList(true);
                //OpenStationMenuIndividual(false);
                //OpenRouteMenuList(false);
                break;
            case ActionMode.train:
                //OpenActionMenu(false);
                //OpenBuildMenu(false);
                //OpenStationMenuList(false);
                //OpenStationMenuIndividual(false);
                OpenRouteMenuList(true);
                break;
            case ActionMode.hidden:
                OpenActionMenu(false);
                OpenBuildMenu(false);
                OpenStationMenuList(false);
                OpenStationMenuIndividual(false);
                OpenRouteMenuList(false);
                OpenRouteMenuIndividualList(false);
                break;
            case ActionMode.route:
                OpenRouteMenuIndividualList(true);
                break;
                
        }
    }

    public void OpenActionMenu(bool newState)
    {
        actionMenu.SetActive(newState);
        if (newState) {
            OpenBuildMenu(false);
            OpenStationMenuIndividual(false);
            OpenStationMenuIndividualTuto(false);
            OpenStationMenuList(false);
            OpenRouteCreator(false);
            OpenRouteMenuIndividualList(false);
            OpenRouteMenuList(false);
        }
    }
    void OpenBuildMenu(bool newState)
    {
        buildMenu.SetActive(newState);
        if (newState) {
            OpenActionMenu(false);
            OpenStationMenuIndividual(false);
            OpenStationMenuIndividualTuto(false);
            OpenStationMenuList(false);
            OpenRouteCreator(false);
            OpenRouteMenuIndividualList(false);
            OpenRouteMenuList(false);
        }
        if (!newState) {
            ChangeBuildMode(0);
        }
    }
    void OpenStationMenuIndividual(bool newState)
    {
        stationMenuIndividual.SetActive(newState);
        if (newState)
        {
            OpenActionMenu(false);
            OpenBuildMenu(false);
            OpenStationMenuIndividualTuto(false);
            OpenStationMenuList(false);
            OpenRouteCreator(false);
            OpenRouteMenuIndividualList(false);
            OpenRouteMenuList(false);
        }
    }
    void OpenStationMenuIndividualTuto(bool newState)
    {
        stationMenuIndividualTuto.SetActive(newState);
        if (newState)
        {
            OpenActionMenu(false);
            OpenBuildMenu(false);
            OpenStationMenuIndividual(false);
            OpenStationMenuList(false);
            OpenRouteCreator(false);
            OpenRouteMenuIndividualList(false);
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
            OpenStationMenuIndividualTuto(false);
            OpenRouteCreator(false);
            OpenRouteMenuIndividualList(false);
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
            OpenStationMenuIndividualTuto(false);
            OpenStationMenuList(false);
            OpenRouteMenuIndividualList(false);
            OpenRouteMenuList(false);
        }
    }
    public void OpenRouteMenuIndividualList(bool newState)
    {
        routeMenuIndividual.SetActive(newState);
        if (newState)
        {
            FillRouteList();
            OpenActionMenu(false);
            OpenBuildMenu(false);
            OpenStationMenuIndividual(false);
            OpenStationMenuIndividualTuto(false);
            OpenStationMenuList(false);
            OpenRouteCreator(false);
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
            OpenStationMenuIndividualTuto(false);
            OpenStationMenuList(false);
            OpenRouteCreator(false);
            OpenRouteMenuIndividualList(false);
        }
    }
    public void OpenTutoMenu(bool newState)
    {
        tutoPannel.SetActive(newState);
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
        destNumber = 0;
        newRouteStartDropdown.ClearOptions();
        newRouteStartDropdown.options.Add(new TMP_Dropdown.OptionData() { text = startStationName });
        newRouteStartDropdown.RefreshShownValue();

        newRoutePath.Clear();
        for (int i = routeCreatorDestinations.childCount - 1; i > 1; i--) 
            Destroy(routeCreatorDestinations.GetChild(i).gameObject);
        for(int i = newRouteDestDropdown.Count - 1; i > 0; i--) 
            newRouteDestDropdown.RemoveAt(i);

        for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
            if (listDestination[0] == GridBoard.Instance.stationList[i].nameStation)
                newRoutePath.Add(GridBoard.Instance.stationList[i]);
        newRouteDestDropdown[0].ClearOptions();
        for (int i = 0; i < listDestination.Count; i++) 
            newRouteDestDropdown[0].options.Add(new TMP_Dropdown.OptionData() { text = listDestination[i] });
        newRouteDestDropdown[0].RefreshShownValue();
    }
    public void AddDestination()
    {
        //instanciate dropdown list
        TMP_Dropdown nRouteDropdown = Instantiate(dropdownDestPrefab, routeCreatorDestinations).GetComponent<TMP_Dropdown>();

        //fill with possible destination
        nRouteDropdown.options.Clear();
        for(int i = 0; i < newRoutePath[destNumber].destinationNameList.Count; i++) {
            nRouteDropdown.options.Add(new TMP_Dropdown.OptionData() { text = newRoutePath[destNumber].destinationNameList[i] });
        }

        //add to dest list
        for (int i = 0; i < GridBoard.Instance.stationList.Count; i++)
            if (newRoutePath[destNumber].destinationNameList[0] == GridBoard.Instance.stationList[i].nameStation)
                newRoutePath.Add(GridBoard.Instance.stationList[i]);
        newRouteDestDropdown.Add(nRouteDropdown);

        //enable minus if list > 0
        destNumber++;
    }
    public void MinusDestination()
    {
        //find in list //to do if doing an advanced minus
        //delete
        if (destNumber > 0)
        {
            Destroy(routeCreatorDestinations.GetChild(destNumber + 1).gameObject);
            newRouteDestDropdown.RemoveAt(destNumber);
            newRoutePath.RemoveAt(destNumber);
            destNumber--;
        }
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
            //change next dropdown
            int nextIDs = destNumber - destID;
            for (int i = 0; i < nextIDs ; i++)
                MinusDestination();
            for (int i = 0; i < nextIDs; i++)
                AddDestination();
        }
        else
            Debug.LogError("Route edding destination failled dest index not found");
    }
    public void DeployTrain() {
        if (currentTileSelected.HasStation)
            currentTileSelected.station.CreateRoute(newRoutePath[0]);
    }
    public void DeployTrainMultiple()
    {
        if (currentTileSelected.HasStation)
            currentTileSelected.station.CreateRouteMultiple(newRoutePath);
    }

    public void SetIndividualRoute(TrainRoute route)
    {
        RouteItemIndividual.SetDisplay(route);
    }

    void FillRouteList()
    {
        for (int i = 0; i < routeList.childCount; i++)
            Destroy(routeList.GetChild(i).gameObject);
        List<TrainRoute> listRoute = GameManager.Instance.gridBoard.routeList;
        for (int i = 0; i < listRoute.Count; i++)
        {
            UIRouteItem item = Instantiate(routeItemPrefab);
            item.inGameUI = this;
            item.SetDisplay(listRoute[i]);
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
    public void UpdateTileInfo(string name, string content, float pollution, int pollutionMax, int pollutionMin)
    {
        //selectedTileName.text = name;
        selectedTileContent.text = content;
        pollutionLevel.text = "Pollution : " + pollution.ToString("0.00");
        pollutionLevelMax.text = "Pollution Max : " + pollutionMax;
        pollutionLevelMin.text = "Pollution Min : " + pollutionMin;
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

    public void ActivateOutlinerRessource(bool nState)
    {
        tutoRessourceOutliner.SetActive(nState);
    }
    public void ActivateOutlinerContract(bool nState)
    {
        tutoContractOutliner.SetActive(nState);
    }
}
