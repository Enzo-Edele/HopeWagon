using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class UIGridEditor : MonoBehaviour
{
    public GridBoard grid;

    bool applyStation, applyFactory;

    enum OptionalToggle
    {
        Ignore, Yes, No
    }

    OptionalToggle railMode, factoryMode;

    bool isDrag;
    TileDirection dragDirection;
    GameTile previousTile;

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
            if (Input.GetKeyDown(KeyCode.T))
            {
                GameTile currentTile = grid.GetTile(Camera.main.ScreenPointToRay(Input.mousePosition));
                if (currentTile.station != null)
                    currentTile.station.DeployTrain(currentTile.station.destinations[0]);
            }
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
        EditTile(currentTile);
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
    void EditTile(GameTile tile)
    {
        if (tile)
        {
            if (applyStation)
            {
                tile.HasStation = !tile.HasStation;
                return;
            }
            if (applyFactory)
            {
                tile.HasIndustry = !tile.HasIndustry;
                return;
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
}
