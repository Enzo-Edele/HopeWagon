using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class UIGridEditor : MonoBehaviour
{
    public GridBoard grid;
    [SerializeField] GameObject editMenu;

    bool applyStation, applyFactory;
    bool applyCaptor, applyPlate, applyDrill, applyRefinery, applyChip;

    bool isActive = false;

    enum OptionalToggle
    {
        Ignore, Yes, No
    }
    OptionalToggle railMode;

    enum ColorToggle
    {
        Ignore, Green, Black
    }
    ColorToggle colorMode;

    bool isDrag;
    TileDirection dragDirection;
    GameTile previousTile;

    public void ActivateEditMode()
    {
        isActive = !isActive;
        editMenu.SetActive(isActive);
    }

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleInput();
                return;
            }
            if (Input.GetMouseButton(0))
            {
                HandleInputDrag();
                //GameTile tile = grid.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
                //mettre un objet tile selector qui s'attache a la tile select
                return;
            }
            /*if (Input.GetKeyDown(KeyCode.T))
            {
                GameTile currentTile = grid.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
                if (currentTile.station != null)
                    currentTile.station.DeployTrain(currentTile.station.destinationList[0]);
            }*/
            //other input here
        }
        previousTile = null;
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
    void HandleInput()
    {
        GameTile currentTile = grid.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (currentTile) {
            EditTile(currentTile);
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
    void EditTiles(GameTile currentTile)
    {
        if (railMode == OptionalToggle.Yes)
        {
            currentTile.HasRail = true;
        }
        if (railMode == OptionalToggle.No)
        {
            currentTile.HasRail = false;
        }
        if(colorMode != ColorToggle.Ignore)
        {
            if (colorMode == ColorToggle.Green)
                currentTile.Paint(GameManager.Instance.colorArrayTile[0]);
            else if(colorMode == ColorToggle.Black)
                currentTile.Paint(GameManager.Instance.colorArrayTile[3]);
        }
        if (isDrag)
        {
            GameTile otherTile = null; //get tile using direction inverse
            if (otherTile)
            {
                if (railMode == OptionalToggle.Yes)
                    return; //utiliser si on veut faire un outgoing direction
            }
        }
    }
    void EditTile(GameTile tile) {
        if (tile) {
            if (applyStation) {
                tile.HasStation = !tile.HasStation;
                return;
            }
            if (applyFactory) {
                tile.HasIndustry = false;
                return;
            }
            if (applyCaptor) {
                tile.HasIndustry = true;
                tile.industry.SetIndustryType(GameManager.Instance.industryTypes[0]);
            }
            if (applyPlate) {
                tile.HasIndustry = true;
                tile.industry.SetIndustryType(GameManager.Instance.industryTypes[1]);
            }
            if (applyDrill) {
                tile.HasIndustry = true;
                tile.industry.SetIndustryType(GameManager.Instance.industryTypes[2]);
            }
            if (applyRefinery) {
                tile.HasIndustry = true;
                tile.industry.SetIndustryType(GameManager.Instance.industryTypes[3]);
            }
            if (applyChip)
            {
                tile.HasIndustry = true;
                tile.industry.SetIndustryType(GameManager.Instance.industryTypes[4]);
            }
        }
    }

    public void SetRailMode(int mode)
    {
        railMode = (OptionalToggle)mode;
    }
    public void SetApplyStation(bool toggle)
    {
        applyStation = toggle;
    }

    public void SetApplyFactory(bool toggle)
    {
        applyFactory = toggle;
    }
    //unifier la fonction pour set le type d'industrie
    public void SetApplyCaptor(bool toggle)
    {
        applyCaptor = toggle;
    }
    public void SetApplyPlate(bool toggle)
    {
        applyPlate = toggle;
    }
    public void SetApplyDrill(bool toggle)
    {
        applyDrill = toggle;
    }
    public void SetApplyRefinery(bool toggle)
    {
        applyRefinery = toggle;
    }
    public void SetApplyChip(bool toggle)
    {
        applyChip = toggle;
    }

    public void SetColorMode(int mode)
    {
        colorMode = (ColorToggle)mode;
    }
}
